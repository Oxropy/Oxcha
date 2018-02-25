using ORM.Dao;
using ORM.DataBase;
using static ORM.QueryBuilder.QueryBuilderExtensions;
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
using ORM.QueryBuilder;

namespace Oxcha
{
    class Program
    {
        static void Main(string[] args)
        {
            SelectClause select = Select(("c".Col("t")).As("a"), "co".Col(), QueryBuilderExtensions.Col("cl").As("col"), "Now".Call().As("Date"));
            // SELECT (t.c) AS a, co, (cl) AS col, (Now()) AS Date
            Console.WriteLine(select.GetQuery());

            FromClause from = From("t".Table().LeftOuterJoin("lo".Table(), "x".Col("t").Eq("y".Col("lo")).And("y".Col("t").Neq("x".Col("lo")))));
            // FROM t LEFT OUTER JOIN lo ON ((t.x = lo.y) And (t.y != lo.x))
            Console.WriteLine(from.GetQuery());
            
            WhereClause where = Where("c".Col("t").Eq(Val("v")).And(Val(2.0).LtEq(Val(1)).Or(Val(-1).Neq("NOW".Call()))));
            // WHERE (s.f = 'v') And ((2 <= 1) Or (-1 != NOW()))
            Console.WriteLine(where.GetQuery());


            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oxcha").ToString();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Client client = AuthenticateLogin(args);
            if (client != null)
            {
                EntryHandler(client);
            }
        }

        private static Client AuthenticateLogin(string[] args)
        {
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
            }
            else
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Too few arguments!");
                    Console.WriteLine("Usage: {0} <client id> <client secret>", Environment.GetCommandLineArgs()[0]);
                    Environment.Exit(1);
                    return null;
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

            if (authResponse is SuccessfulAuthentication success)
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

                return client;
            }
            else
            {
                var failure = authResponse as FailedAuthentication;
                Console.WriteLine("Authentication Failure: {0}; Reason: {1}", failure.Failure, failure.Reason);

                Console.ReadKey();
            }
            return null;
        }

        private static void EntryHandler(Client client)
        {
            string channel = string.Empty;

            ShowInfo();

            string line = "";
            while (line != "!exit")
            {
                line = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line[0] != '!')
                    {
                        if (!string.IsNullOrWhiteSpace(channel))
                        {
                            client.SendMessage(channel, line); 
                        } else
                        {
                            ShowInfo();
                        }
                    }
                    else
                    {
                        if (line.StartsWith("!join"))
                        {
                            if (!string.IsNullOrWhiteSpace(channel))
                            {
                                CliChannelHandler channelHandler = new CliChannelHandler();
                                client.JoinChannel(channel, channelHandler); 
                            } else
                            {
                                Console.WriteLine(string.Format("You are joined to '{0}'!", channel));
                            }
                        }
                        else if (line.StartsWith("!leave"))
                        {
                            if (!string.IsNullOrWhiteSpace(channel))
                            {
                                client.LeaveChannel(channel);
                                channel = string.Empty; 
                            }
                        }
                        else if (line.StartsWith("!clear"))
                        {
                            Console.Clear();
                        }
                        else if (line.StartsWith("!help"))
                        {
                            ShowHelp();
                        }
                    }
                }
            }
        }

        private static void ShowInfo()
        {
            Console.WriteLine("You are not connected to a channel.");
            Console.WriteLine("Use '!join' to join one.");
            Console.WriteLine("'!help' for help.");
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("'!exit' : close the program");
            Console.WriteLine("'!join' : join channel when no channel is joined");
            Console.WriteLine("'!leave' : leave channel when connect");
            Console.WriteLine("'!clear' : clears the screen");
        }
    }
}
