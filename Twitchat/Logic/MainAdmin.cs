using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitchat.Authentication;
using Twitchat.GUI.View;
using Twitchat.GUI.ViewModel;
using Twitchat.Logic.DataContract;
using Twitchat.Logic.IO;

namespace Twitchat.Logic
{
    public static class MainAdmin
    {
        #region Eigenschaften und Felder
        public static Dictionary<string, string> TwitchUserTokenDict = new Dictionary<string, string>();
        //TODO: Ordner beim ersten ausführen anlegen
        public static DB DbObject = new DB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Oxcha/Oxcha.db").ToString());
        #endregion

        #region Methoden
        public static void RetrieveAuthenticationUsers(string clientId, string clientSecret)
        {
            TwitchUserTokenDict = GetAccesTokens();
            if (TwitchUserTokenDict.Count == 0)
            {
                AddAuthenticationUser(clientId, clientSecret);
            }
        }

        public static void AddAuthenticationUser(string clientId, string clientSecret)
        {
            var authResponse = TwitchAuthentication.Authenticate(clientId, clientSecret, url => {
                TwitchAuthenticationWindow authentWindow = new TwitchAuthenticationWindow(url);
                authentWindow.ShowDialog();

                return authentWindow.Url;
            });

            if (authResponse is SuccessfulAuthentication success)
            {
                MainAdmin.TwitchUserTokenDict.Add(success.Name, success.Token);
            }
        }

        #region DB Methoden
        #region user_auth_key
        public static bool AddAccesToken(string user, string accessToken)
        {
            try
            {
                DbObject.Run(string.Format("INSERT INTO user_auth_key (user, access_token) VALUES ('{0}', '{1}')", user, accessToken), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteAccesToken(string user, string accessToken)
        {
            try
            {
                DbObject.Run(string.Format("DELETE FROM user_auth_key WHERE user = '{0}' and access_token = '{1}'", user, accessToken), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetAccesTokens()
        {
            try
            {
                return DbObject.Run("SELECT user, access_token FROM user_auth_key", c =>
                {

                    var accesTokenDict = new Dictionary<string, string>();
                    var reader = c.ExecuteReader();

                    while (reader.Read())
                    {
                        accesTokenDict.Add(reader.GetString(0), reader.GetString(1));
                    }

                    return accesTokenDict;
                });
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }
        #endregion

        #region channelautojoin
        public static bool AddChannelAutoJoin(string user, string channel)
        {
            try
            {
                DbObject.Run(string.Format("INSERT INTO channelautojoin (user, channel) values ('{0}', '{1}')", user, channel), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool DeleteChannelAutoJoin(string user, string channel)
        {
            try
            {
                DbObject.Run(string.Format("DELETE FROM channelautojoin WHERE user = '{0}' and channel = '{1}'", user, channel), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region friends
        public static bool AddFriend(string user, string alias = "", string emote = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    DbObject.Run(string.Format("INSERT INTO friends (user) values ('{0}')", user), c => c.ExecuteNonQuery()); 
                } else
                {
                    DbObject.Run(string.Format("INSERT INTO friends (user, alias) values ('{0}', '{1}')", user, alias), c => c.ExecuteNonQuery());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateFriendAlias(string user, string alias)
        {
            try
            {
                DbObject.Run(string.Format("UPDATE friends SET (alias) values ({0} WHERE user = '{1}')", alias, user), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UpdateFriendEmote(string user, string emote)
        {
            try
            {
                DbObject.Run(string.Format("UPDATE friends SET (emote) values ({0} WHERE user = {1})", emote, user), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Friend GetFriendInfo(string user)
        {
            try
            {
                return DbObject.Run(string.Format("SELECT user, alias, emote FROM friends where user = {0}", user), c =>
                {
                    var reader = c.ExecuteReader();

                    return new Friend(user, reader.GetString(1), reader.GetString(2));
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetFriendEmote(string user)
        {
            try
            {
                return DbObject.Run(string.Format("SELECT emote FROM friends where user = {0}", user), c =>
                {
                    var reader = c.ExecuteReader();

                    return reader.GetString(0);
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool DeleteFriend(string user)
        {
            try
            {
                DbObject.Run(string.Format("DELETE FROM friends WHERE user = {0}", user), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
