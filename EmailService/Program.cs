using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.EmailHelpers;
using MassTransit;
using Microshop.Contract;
using Nancy;
using Nancy.Hosting.Self;
using NLog;
using NLog.Targets;
using StatsdClient;
using Topshelf;

namespace EmailService
{
    public class EmailConsumer : IConsumer<SendEmail>
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IEmailService _emailService;

        public EmailConsumer()
        {
            var smtpClient = new SmtpClient("localhost", 25);
            _emailService = new EmailService(smtpClient);
        }

        public async Task Consume(ConsumeContext<SendEmail> context)
        {
            await Task.Run(() => SendEmail(context.Message));
        }

        private bool SendEmail(SendEmail message)
        {
            try
            {
                _logger.Info($"Sending email : '{message.EmailType}' to : '{message.Email}'");
                _emailService.SendEmail(message.Email, (EmailType)message.EmailType);
                Metrics.Counter("email_sent");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an issue while sending email");
                Metrics.Counter("email_failure");
                return false;
            }
        }
    }

    public class EmailMessage
    {
        public string Email { get; set; }
        public EmailType Type { get; set; }
    }

    public class MainEndpoint : NancyModule
    {
        public IEnumerable<string> GetLogs()
        {
            var target = (MemoryTarget)LogManager.Configuration.FindTargetByName("memory");
            return target.Logs;
        }

        public Dictionary<string, bool> HealthCheck()
        {
            var smptServerIsOnline = true;
            var isRabbitMqOk = true;
            return new Dictionary<string, bool>
            {
                {"Ok", smptServerIsOnline && isRabbitMqOk },
                {"smtpServer", smptServerIsOnline},
                {"rabbitMQ", isRabbitMqOk }
            };
        }

        public MainEndpoint()
        {
            Get["/"] = args => Response.AsRedirect("/health");

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
            _bus = Bus.Factory.CreateUsingRabbitMq(cf =>
            {
                var host = cf.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cf.ReceiveEndpoint(host, "email_queue", c =>
                {
                    c.Consumer<EmailConsumer>();
                });
            });

            _bus.Start();

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

                f.RunAsLocalService();
                f.SetDescription("Workshop EmailService");
                f.SetDisplayName("Workshop EmailService");
                f.SetServiceName("Workshop EmailService");
            });
        }
    }
}
