using System;
using System.Net;
using System.Text;
using RestSharp;
using Uniars.Shared.Database.Entity;

namespace Uniars.Client.Http
{
    public class ApiClient : RestClient
    {
        public User CurrentUser { get; private set; }

        protected string authString;

        public ApiClient(string baseUri)
            : base(baseUri)
        {
        }

        /// <summary>
        /// Check if the client can connect to server
        /// </summary>
        /// <returns></returns>
        public void TestConnect(Action<IRestResponse> response)
        {
            ApiRequest request = new ApiRequest("health", Method.GET);
            this.ExecuteAsync(request, response);
        }

        /// <summary>
        /// Login to server
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Instance of logged in user</returns>
        public User Login(string username, string password)
        {
            this.authString = GetAuthenticationString(username, password);

            RestRequest request = new RestRequest("auth");
            request.AddHeader("Authorization", authString);

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
        public void LoginAsync(string username, string password, Action<IRestResponse<User>> callback)
        {
            this.authString = GetAuthenticationString(username, password);

            RestRequest request = new RestRequest("auth");
            request.AddHeader("Authorization", authString);

            this.ExecuteAsync<User>(request, response =>
            {
                this.CurrentUser = response.Data;
                callback(response);
            });
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void Logout()
        {
            this.CurrentUser = null;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            request.AddHeader("Authorization", authString);
            return base.Execute<T>(request);
        }

        public override RestRequestAsyncHandle ExecuteAsync<T>(IRestRequest request, Action<IRestResponse<T>, RestRequestAsyncHandle> callback)
        {
            request.AddHeader("Authorization", authString);
            return base.ExecuteAsync<T>(request, callback);
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            request.AddHeader("Authorization", authString);
            return base.Execute(request);
        }

        public override RestRequestAsyncHandle ExecuteAsync(IRestRequest request, Action<IRestResponse, RestRequestAsyncHandle> callback)
        {
            request.AddHeader("Authorization", authString);
            return base.ExecuteAsync(request, callback);
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
