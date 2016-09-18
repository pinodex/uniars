using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Self;

namespace Uniars.Server.Http
{
    public delegate void ServerStartedEvent(object sender, EventArgs e);

    public delegate void ServerStartingEvent(object sender, EventArgs e);

    public delegate void ServerStoppedEvent(object sender, EventArgs e);

    public class Server
    {
        /// <summary>
        /// Instance of Nancy Binay
        /// </summary>
        private NancyHost Nancy;

        /// <summary>
        /// Fired when the server has started
        /// </summary>
        public event ServerStartedEvent OnStart;

        /// <summary>
        /// Fired when the server is starting
        /// </summary>
        public event ServerStartingEvent OnStarting;

        /// <summary>
        /// Fired when the server has stopped
        /// </summary>
        public event ServerStoppedEvent OnStop;

        /// <summary>
        /// Returns true if the server is started/running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Host used by the server
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Port used by the server
        /// </summary>
        public uint Port { get; private set; }

        /// <summary>
        /// Constructs Server
        /// </summary>
        /// <param name="host">Host to bind</param>
        /// <param name="port">Port to bind</param>
        public Server(string host, uint port)
        {
            Host = host;
            Port = port;

            string binding = string.Format("http://{0}:{1}", host, port);

            HostConfiguration configuration = new HostConfiguration
            {
                RewriteLocalhost = true,
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                }
            };

            Bootstrapper bootstrapper = new Bootstrapper();

            Nancy = new NancyHost(bootstrapper, configuration, new Uri(binding));
        }

        /// <summary>
        /// Start server listening
        /// </summary>
        public void Start()
        {
            if (OnStarting != null)
            {
                OnStarting(this, null);
            }

            if (Nancy != null && IsRunning == false)
            {
                Nancy.Start();
            }

            IsRunning = true;

            if (OnStart != null)
            {
                OnStart(this, null);
            }
        }

        /// <summary>
        /// Stop server listening
        /// </summary>
        public void Stop()
        {
            if (Nancy != null && IsRunning == true)
            {
                Nancy.Stop();
            }

            IsRunning = false;

            if (OnStop != null)
            {
                OnStop(this, null);
            }
        }

        /// <summary>
        /// Restart server listening
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }
    }
}
