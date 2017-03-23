using System;
using System.Collections.Generic;
using MassTransit;
using Nancy;
using Nancy.Hosting.Self;
using NLog;
using NLog.Targets;
using Quartz.Impl;
using SimpleInjector;
using StatsdClient;
using Topshelf;
using TransferCheckService.Jobs;

namespace TransferCheckService
{
    public class MainEndpoint : NancyModule
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<string> GetLogs()
        {
            var target = (MemoryTarget)LogManager.Configuration.FindTargetByName("memory");
            return target.Logs;
        }

        public Dictionary<string, bool> HealthCheck()
        {
            var externalBankSystem = true;
            var database = true;
            var rabbitMq = true;
            return new Dictionary<string, bool>
            {
                {"Ok", externalBankSystem && database && rabbitMq },
                {"externalBankSystem", externalBankSystem},
                {"database", database},
                { "rabbitMq", rabbitMq }
            };
        }

        public MainEndpoint()
        {
            Get["/"] = args => Response.AsJson("it works");

            // it is important that service lets us know what is faulting
            Get["/health"] = args => Response.AsJson(HealthCheck());
            Get["/logs"] = args => Response.AsJson(GetLogs());
        }
    }

    public class ServiceHost
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private NancyHost _nancyHost;
        private IBusControl _bus;

        public void Start()
        {
            var container = new Container();
            container.Register(() =>
            {
                var sched = new StdSchedulerFactory().GetScheduler();
                sched.JobFactory = new SimpleInjectiorJobFactory(container);
                return sched;
            });

            container.Register<IEmailService, EmailService>();
            container.Register<ITransferCheckService, TransferCheckService>();
            container.Register<CheckTransferJob>();

            Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = "statsd.hostedgraphite.com",
                Prefix = "506cdd58-a78d-41a3-85c7-04df4e9e9549",
                StatsdServerPort = 8125
            });

            _bus = Bus.Factory.CreateUsingRabbitMq(cf =>
            {
                var host = cf.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            _bus.Start();

            container.Register<IBus>(() => _bus);

            MainScheduler.Start(container);

            var cfg = new HostConfiguration();
            cfg.RewriteLocalhost = true;
            cfg.UrlReservations.CreateAutomatically = true;

            _nancyHost = new NancyHost(new Uri("http://localhost:50002"), new DefaultNancyBootstrapper(), cfg);
            _nancyHost.Start();
            _logger.Info("Service has started.");
        }

        public void Stop()
        {
            MainScheduler.Stop();
            _nancyHost.Stop();
            _bus.Stop();
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

                f.RunAs($"{Environment.MachineName}\\mfranc", "mopsik00");
                f.SetDescription("Transfer Check Service");
                f.SetDisplayName("Transfer Check Service");
                f.SetServiceName("Transfer Check Service");
            });
        }
    }
}
