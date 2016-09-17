using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Nancy;

namespace Uniars.Server.Http
{
    public class ErrorJsonResponse : Response
    {
        public ErrorJsonResponse(int httpCode, int code, string message, object data = null)
        {
            this.StatusCode = (HttpStatusCode)httpCode;
            this.ContentType = "text/json";

            ErrorModel e = new ErrorModel
            {
                Error = new ErrorContents {
                    Code = code,
                    Message = message,
                    Data = data
                }
            };

            this.Contents = stream =>
            {
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e));
                stream.Write(bytes, 0, bytes.Length);
            };
        }

        public class ErrorModel
        {
            public ErrorContents Error { get; set; }
        }

        public class ErrorContents
        {
            public int Code { get; set; }

            public string Message { get; set; }

            public object Data { get; set; }
        }
    }
}
