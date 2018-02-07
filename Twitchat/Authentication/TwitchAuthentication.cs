﻿using log4net;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using Twitchat.Logic;
using Twitchat.Resources;

namespace Twitchat.Authentication
{
    public static class TwitchAuthentication
    {
        static ILog logger = LogManager.GetLogger(typeof(TwitchAuthentication));

        public static string GetState()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static IAuthenticationResult Authenticate(string clientId, string clientSecret, Func<string, string> func)
        {
            string state = GetState();
            string url = AuthUri(state, clientId);
            string returnUrl = func(url);
            return AuthRequest(returnUrl, state, clientId, clientSecret);
        }

        public static string AuthUri(string state, string clientId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("https://api.twitch.tv/kraken/oauth2/authorize");
            sb.AppendFormat("?client_id={0}", clientId);
            sb.AppendFormat("&redirect_uri={0}", TwitchResource.RedirectUri);
            sb.Append("&response_type=code");
            sb.AppendFormat("&scope={0}", TwitchResource.Scope);
            sb.AppendFormat("&state={0}", state);

            return sb.ToString();
        }

        public static IAuthenticationResult AuthRequest(string uriQuery, string state, string clientId, string clientSecret)
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(uriQuery);

            if (nvc["state"] == state)
            {
                try
                {
                    string code = nvc["code"];

                    var client = new RestClient("https://api.twitch.tv/kraken");

                    #region Request Acces_Token
                    var request = new RestRequest("oauth2/token", Method.POST);
                    request.AddParameter("client_id", clientId);
                    request.AddParameter("client_secret", clientSecret);
                    request.AddParameter("code", code);
                    request.AddParameter("grant_type", "authorization_code");
                    request.AddParameter("redirect_uri", TwitchResource.RedirectUri);
                    request.AddParameter("state", state);

                    IRestResponse<TwitchResponse> response = client.Execute<TwitchResponse>(request);
                    #endregion
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        #region Request User
                        var requestUser = new RestRequest("user", Method.GET);
                        client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(response.Data.access_token);
                        requestUser.AddHeader("Client-ID", clientId);

                        IRestResponse<TwitchUserResponse> responseUser = client.Execute<TwitchUserResponse>(requestUser);
                        #endregion
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return new SuccessfulAuthentication(responseUser.Data.name, response.Data.access_token);
                        }
                    }
                    return new FailedAuthentication(AuthenticationFailure.HttpError, response.StatusCode.ToString());
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat("Error: {0}", ex);
                }
            }
            else
            {
                return new FailedAuthentication(AuthenticationFailure.InvalidState);
            }

            return new FailedAuthentication(AuthenticationFailure.Unknown);
        }

#pragma warning disable IDE1006
        public class TwitchResponse
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public List<string> scope { get; set; }
        }
#pragma warning restore IDE1006

#pragma warning disable IDE1006
        public class TwitchUserResponse
        {
            public string _id { get; set; }
            public string bio { get; set; }
            public string created_at { get; set; }
            public string display_name { get; set; }
            public string email { get; set; }
            public string email_verified { get; set; }
            public string logo { get; set; }
            public string name { get; set; }
            public string partnered { get; set; }
            public string type { get; set; }
            public string updated_at { get; set; }
        }
#pragma warning restore IDE1006


    }
}
