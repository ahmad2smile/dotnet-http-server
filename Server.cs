using System;
using System.Net;
using System.Threading;

namespace WebServer
{
    public class Server
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Action<HttpListenerRequest, HttpListenerResponse> _handler;

        public Server(Action<HttpListenerRequest, HttpListenerResponse> handler, params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Http Listener server is not supported");
            }

            // URI prefixes are required eg: "http://localhost:8000/test/"
            if (prefixes == null || prefixes.Length == 0 || handler == null)
            {
                throw new ArgumentException("URI prefixes are required");
            }

            foreach (var s in prefixes)
            {
                _listener.Prefixes.Add(s);
            }

            _handler = handler;
            _listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(httpListenerContext =>
                    {
                        if (!(httpListenerContext is HttpListenerContext context))
                        {
                            return;
                        }

                        _handler(context.Request, context.Response);
                    }, _listener.GetContext());
                }
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}