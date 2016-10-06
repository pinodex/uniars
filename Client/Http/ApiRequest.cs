using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Net;

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

        public static void Search<T>(string url, IDictionary<string, string> queries, Action<T> result) where T : new()
        {
            ApiRequest request = new ApiRequest(url);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddQueryParameter(query.Key, query.Value);
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

        public static void Search<T>(ApiRequest request, IDictionary<string, string> queries, Action<T> result) where T : new()
        {
            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddQueryParameter(query.Key, query.Value);
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

        public static void Search<T>(string url, IDictionary<string, string> queries, Action<IRestResponse<T>> result) where T : new()
        {
            ApiRequest request = new ApiRequest(url);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddQueryParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, result);
        }

        public static void Search<T>(ApiRequest request, IDictionary<string, string> queries, Action<IRestResponse<T>> result) where T : new()
        {
            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                {
                    request.AddQueryParameter(query.Key, query.Value);
                }
            }

            App.Client.ExecuteAsync<T>(request, result);
        }
    }
}
