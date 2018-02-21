using ORM.Dao;
using ORM.DataBase;
using ORM.DataContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitchat.Authentication;
using Twitchat.Logic;
using Twitchat.Twitch;

namespace Oxcha
{
    class Program
    {
        static void Main(string[] args)
        {
            WhereClause where1 = Q.Where(Q.And(Q.Eq(Q.Col("s", "f"), Q.Val("v")), Q.Or(Q.LtEq(Q.Val(2.0), Q.Val(1)), Q.Neq(Q.Val(-1), Q.Fn("NOW")))));
            WhereClause where2 = Q.Where("s".Col("f").Eq("v".Val()).And(2.0.Val().LtEq(1.Val()).Or((-1).Val().Neq(Q.Fn("NOW")))));

            Console.WriteLine(where1);
            Console.WriteLine(where2);


            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oxcha").ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Console.WriteLine("Login user: ");
            string user = Console.ReadLine();

            bool newToken = false;
            IAuthenticationResult authResponse;
            
            Dictionary<string, string> UserAccessToken = MainAdmin.GetAccesTokens();
            if (UserAccessToken.ContainsKey(user))
            {
                //TODO: Refresh access token https://dev.twitch.tv/docs/authentication#refreshing-access-tokens
                //TODO: Token validieren
                authResponse = new SuccessfulAuthentication(user, UserAccessToken[user]);

                //TODO: nicht in else, da es auch aufgerufen werden muss wenn das token nicht mehr gültig ist
            } else {
                if (args.Length < 2)
                {
                    Console.WriteLine("Too few arguments!");
                    Console.WriteLine("Usage: {0} <client id> <client secret>", Environment.GetCommandLineArgs()[0]);
                    Environment.Exit(1);
                    return;
                }

                string clientId = args[0];
                string clientSecret = args[1];

                authResponse = TwitchAuthentication.Authenticate(clientId, clientSecret, url => {
                    Console.WriteLine("Log in URL:");
                    Console.WriteLine(url);
                    return Console.ReadLine();
                });
                newToken = true;
            }

            var success = authResponse as SuccessfulAuthentication;
            if (success != null)
            {
                // When new Token add Token to DB
                if (newToken)
                {
                    MainAdmin.AddAccesToken(success.Name, success.Token);
                }
                
                Console.WriteLine("Authentication Success");

                CliClientHandler clientHandler = new CliClientHandler();
                Client client = new Client(success.Name, success.Token, clientHandler);
                client.Connect();

                Console.WriteLine("Channel to join: ");
                string channel = Console.ReadLine();
                CliChannelHandler channelHandler = new CliChannelHandler();
                client.JoinChannel(channel, channelHandler);

                string line = "";
                while(line != "!exit")
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line[0] != '!')
                        {
                            line = Console.ReadLine();
                            client.SendMessage(channel, line);  
                        } else {
                            
                        }
                    }
                }
            }
            else
            {
                var failure = authResponse as FailedAuthentication;
                Console.WriteLine("Authentication Failure: {0}; Reason: {1}", failure.Failure, failure.Reason);

                Console.ReadKey();
            }
        }
    }
}
