using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])

            this.code = code;
            string StatusLine = GetStatusLine(code);
            string date = "Date: " + DateTime.Now + "\r\n";
            string server = "Server: Apache/1.17\r\n";
            string ContentType = "Content-Type: " + contentType + "\r\n";
            string ContentLength = "Content-Length: " + content.Length + "\r\n";
            headerLines.Add(date);
            headerLines.Add(server);
            headerLines.Add(ContentType);
            headerLines.Add(ContentLength);
            
            // TODO: Create the request string
            if (code == StatusCode.Redirect)
            {
                string Location = "Location: " + redirectoinPath + "\r\n";
                headerLines.Add(Location);
            }


            responseString = StatusLine;
            for (int i = 0; i < headerLines.Count; i++)
            {
                responseString += headerLines[i];
            }
            responseString += "\r\n";
            responseString += content;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            statusLine = "HTTP/1.1 " + ((int)code).ToString() + " " + code.ToString() + "\r\n";
            return statusLine;
        }
    }
}
