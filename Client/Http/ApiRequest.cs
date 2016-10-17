using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace Uniars.Client.Http
{
    public class ApiRequest : RestRequest
    {
        public ApiRequest(string uri, Method method)
            : base(uri, method)
        {
        }

        public ApiRequest(string uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Execute parameterized request
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="url">Request URL</param>
        /// <param name="paramList">Parameters</param>
        /// <param name="result">Async result</param>
        public static void ExecuteParams<T>(string url, IDictionary<string, string> paramList, Action<T> result) where T : new()
        {
            ApiRequest request = new ApiRequest(url);

            if (paramList != null)
            {
                foreach (KeyValuePair<string, string> query in paramList)
                {
                    request.AddParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                result(response.Data);
            });
        }

        /// <summary>
        /// Execute parameterized request
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="paramList">Parameters</param>
        /// <param name="result">Async result</param>
        public static void ExecuteParams<T>(ApiRequest request, IDictionary<string, string> queries, Action<T> result) where T : new()
        {
            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                result(response.Data);
            });
        }

        /// <summary>
        /// Execute parameterized request
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="url">Request URL</param>
        /// <param name="paramList">Parameters</param>
        /// <param name="result">Async result</param>
        public static void ExecuteParams<T>(string url, IDictionary<string, string> queries, Action<IRestResponse<T>> result) where T : new()
        {
            ApiRequest request = new ApiRequest(url);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, result);
        }

        /// <summary>
        /// Execute parameterized request
        /// </summary>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="request">Request object</param>
        /// <param name="paramList">Parameters</param>
        /// <param name="result">Async result</param>
        public static void ExecuteParams<T>(ApiRequest request, IDictionary<string, string> queries, Action<IRestResponse<T>> result) where T : new()
        {
            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, result);
        }
    }
}
