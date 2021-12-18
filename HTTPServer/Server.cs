using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(ip);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(10);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket newSocket = this.serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(newSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            byte[] data;
            int receivedLength;
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSocket.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                        break;
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    data = Encoding.ASCII.GetBytes(response.ResponseString);
                    clientSocket.Send(data);
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 

                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "html", content, string.Empty);
                }
                else
                {
                    //TODO: map the relativeURI in request to get the physical path of the resource.
                    string absPath = Configuration.RootPath + request.relativeURI;
                    //TODO: check for redirect
                    string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    //if (!String.IsNullOrEmpty(redirectionPath))
                    if (!String.IsNullOrEmpty(redirectionPath))
                    {
                        absPath = Configuration.RootPath + redirectionPath;
                        content = File.ReadAllText(absPath);
                        return new Response(StatusCode.Redirect, "html", content, redirectionPath.Substring(1, redirectionPath.Length - 1));
                    }

                    //TODO: check file exists
                    //bool exist = CheckFileExistence(absPath);
                    //Console.WriteLine(exist);
                    if (!File.Exists(absPath))
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        return new Response(StatusCode.NotFound, "html", content, "");
                    }

                    else
                    {
                        //TODO: read the physical file
                        content = File.ReadAllText(absPath);

                        // Create OK response
                        return new Response(StatusCode.OK, "html", content, "");
                    }
                }
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "html", content, "");
            }
        }


        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (relativePath[0] == '/')
                relativePath = relativePath.Substring(1,relativePath.Length-1);

            if (Configuration.RedirectionRules.ContainsKey(relativePath))
                return "/" + Configuration.RedirectionRules[relativePath];

            else
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception(defaultPageName + " Page not Exist"));
                return string.Empty;
            }

            // else read file and return its content
            else
            {
                string content = File.ReadAllText(filePath);
                return content;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] RedirectionRulesContent = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                for (int i = 0; i < RedirectionRulesContent.Length; i++)
                {
                    string[] line = RedirectionRulesContent[i].Split(',');
                    Configuration.RedirectionRules.Add(line[0], line[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
