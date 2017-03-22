using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Hosting.Self;
using NLog;
using NLog.Targets;
using StatsdClient;
using Topshelf;
using HttpStatusCode = Nancy.HttpStatusCode;

namespace SimpleMicroservice
{
    public class MainEndpoint : NancyModule
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public bool CheckDatabase()
        {
            _logger.Info("Performed check database.");
            return false;
        }

        public string MicroX(int x)
        {
            Metrics.Counter("MicroX");
            _logger.Info($"Executed MicroX with {x}");
            return $"Micro-{x}";
        }

        public string Error()
        {
            _logger.Error("Something bad happened.");
            return "error";
        }

        public IEnumerable<string> GetLogs()
        {
            var target = (MemoryTarget)LogManager.Configuration.FindTargetByName("memory");
            return target.Logs;
        }

        public MainEndpoint()
        {
            Get["/"] = args => Response.AsJson("it works");
            Get["/micro/{x}"] = args => Response.AsJson(new { data = MicroX(args.x) });

            // it is important that service lets us know what is faulting
            Get["/health"] = args => Response.AsJson(new { db = CheckDatabase()}, CheckDatabase() ? HttpStatusCode.OK : HttpStatusCode.FailedDependency);
            Get["/error"] = args => Response.AsJson(new { error = Error() }, HttpStatusCode.InternalServerError);
            Get["/logs"] = args => Response.AsJson(GetLogs());
        }
    }

    public class ServiceHost
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private NancyHost _nancyHost;

        public void Start()
        {
            Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = "statsd.hostedgraphite.com",
                Prefix = "506cdd58-a78d-41a3-85c7-04df4e9e9549",
                StatsdServerPort = 8125
            });

            var cfg = new HostConfiguration();
            cfg.RewriteLocalhost = true;
            cfg.UrlReservations.CreateAutomatically = true;

            _nancyHost = new NancyHost(new Uri("http://localhost:50001"), new DefaultNancyBootstrapper(), cfg);
            _nancyHost.Start();
            _logger.Info("Service has started.");
        }

        public void Stop()
        {
            _nancyHost.Stop();
            _logger.Info("Service has stopped.");
        }
    }

    class Program
    {
        static void Main()
        {
            HostFactory.Run(f =>
            {
                f.Service<ServiceHost>(cfg =>
                {
                    cfg.ConstructUsing(name => new ServiceHost());
                    cfg.WhenStarted(sh => sh.Start());
                    cfg.WhenStopped(sh => sh.Stop());
                });

                f.RunAsLocalService();
                f.SetDescription("Simple Microservice");
                f.SetDisplayName("Simple Microservice");
                f.SetServiceName("Simple Microservice");
            });
        }
    }
}
