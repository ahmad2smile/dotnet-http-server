using System;
using System.Net;
using System.Text;

namespace WebServer
{
    internal class Program
    {
        public static void SendResponse(HttpListenerRequest request, HttpListenerResponse response)
        {
            var str = string.Format("Hello World", DateTime.Now);

            var buffer = Encoding.UTF8.GetBytes(str);

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private static void Main(string[] args)
        {
            var ws = new Server(SendResponse, "http://localhost:8000/");
            ws.Run();

            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }
    }
}