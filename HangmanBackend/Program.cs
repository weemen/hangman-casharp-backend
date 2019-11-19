using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using HangmanBackend.Application;
using HangmanBackend.Domain;
using HangmanBackend.Infrastructure;
using Newtonsoft.Json;

namespace HangmanBackend
{
    class Program
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData = 
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";
        
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                var guid = req.Url.AbsolutePath.Replace("/hangman/", "");
                if (req.HttpMethod == "POST" && Guid.TryParse(guid, out var gameId))
                {
                    Console.WriteLine("Hangman initialized");
                    var bodyStream = new StreamReader(req.InputStream);
                    var bodyText = bodyStream.ReadToEnd();
                    //send StartGame Command
                    Console.WriteLine(bodyText);
                    dynamic body = JsonConvert.DeserializeObject(bodyText);
                    Console.WriteLine(body.command);

                    const int DEFAULTPORT = 1113;
                    var settings = ConnectionSettings.Create();//.EnableVerboseLogging().UseConsoleLogger();
                    var conn = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, DEFAULTPORT));
                    conn.ConnectAsync().Wait();
                    switch (body.command.ToString())
                    {
                        case "StartGame":
                            Console.WriteLine("StartGameCommand");
                            try
                            {
                                var command = new StartGame(gameId, new Guid(), "word", DifficultySetting.EASY);
                                var cmdHandler = new HangmanCommandHandler(new EventStoreRepository(conn, "Game"));
                                cmdHandler.handleStartGame(command);
                            }
                            catch (System.AggregateException)
                            {
                                Console.WriteLine("Game already exists! Cannot start same game twice.");
                            }
                            break;
                        default:
                            Console.WriteLine("Command not found");
                            break;
                    }
                }

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting simple HTTP server");
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}