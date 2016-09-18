using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using RestSharp;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.Http
{
    public class ApiClient : RestClient
    {
        public User CurrentUser { get; private set; }

        public ApiClient(string baseUri)
            : base(baseUri)
        {
        }

        /// <summary>
        /// Login to server
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Instance of logged in user</returns>
        public User Login(string username, string password)
        {
            RestRequest request = new RestRequest("auth");
            request.AddHeader("Authorization", GetAuthenticationString(username, password));

            IRestResponse<User> response = this.Execute<User>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthException();
            }

            this.CurrentUser = response.Data;
            return response.Data;
        }

        /// <summary>
        /// Login to server asynchronously
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="callback">Callback containing the boolean status of login and User object</param>
        public void LoginAsync(string username, string password, Action<bool, User> callback)
        {
            RestRequest request = new RestRequest("auth");
            request.AddHeader("Authorization", GetAuthenticationString(username, password));

            this.ExecuteAsync<User>(request, response =>
            {
                this.CurrentUser = response.Data;
                
                callback(response.StatusCode == HttpStatusCode.OK, response.Data);
            });
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void Logout()
        {
            this.CurrentUser = null;
        }

        /// <summary>
        /// Get base64 encoded authentication string
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>base64 encoded string</returns>
        protected string GetAuthenticationString(string username, string password)
        {
            return string.Format("Basic {0}",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, password))
                )
            );
        }
    }
}
