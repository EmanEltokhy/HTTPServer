using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>

        string[] HTTPRequest;
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter   
            HTTPRequest = Regex.Split(requestString, Environment.NewLine);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (HTTPRequest.Length >= 3)
            {
                // Parse Request line
                // Validate blank line exists
                // Load header lines into HeaderLines dictionary
                requestLines = HTTPRequest[0].Split(' ');
                if (ParseRequestLine() && ValidateBlankLine() &&LoadHeaderLines())
                    return true;  
                else
                    return false;  
            }
            else
                return false;
        }

        private bool ParseRequestLine()
        {

            if (requestLines.Length == 3)
            {
                if (requestLines[0].ToUpper() == "GET")
                    method = RequestMethod.GET;
                else if (requestLines[0].ToUpper() == "POST")
                    method = RequestMethod.POST;
                else if (requestLines[0].ToUpper() == "HEAD")
                    method = RequestMethod.HEAD;
                else
                    return false;


                if (requestLines[2] == "HTTP/1.0")
                {
                    httpVersion = HTTPVersion.HTTP10;
                }
                else if (requestLines[2] == "HTTP/1.1")
                {
                    httpVersion = HTTPVersion.HTTP11;
                }

                else
                {
                    return false;
                }

                relativeURI = requestLines[1];
                return ValidateIsURI(relativeURI);
            }
            else
                return false;
        }
        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            headerLines = new Dictionary<string, string>();
            if (HTTPRequest.Length >= 3)
            {
                for (int i = 1; i < HTTPRequest.Length - 2; i++)
                {
                    string[] headers = HTTPRequest[i].Split(':');
                    headerLines.Add(headers[0], headers[1]);

                }
                return true;
            }
            else
                return false;
        }

        private bool ValidateBlankLine()
        {
            if (String.IsNullOrEmpty(HTTPRequest[(HTTPRequest.Length - 2)]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
