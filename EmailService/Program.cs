using System;
using System.Collections.Generic;
using System.Net.Mail;
using EmailService.EmailHelpers;
using Nancy;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using NLog;
using NLog.Targets;
using StatsdClient;
using Topshelf;

namespace EmailService
{
    public class EmailMessage
    {
        public string Email { get; set; }
        public EmailType Type { get; set; }
    }

    public class MainEndpoint : NancyModule
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private IEmailService _emailService;

        public IEnumerable<string> GetLogs()
        {
            var target = (MemoryTarget)LogManager.Configuration.FindTargetByName("memory");
            return target.Logs;
        }

        public bool SendEmail(EmailMessage message)
        {
            try
            {
                _logger.Info($"Sending email : '{message.Type}' to : '{message.Email}'");
                _emailService.SendEmail(message.Email, message.Type);
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

        public Dictionary<string, bool> HealthCheck()
        {
            var smptServerIsOnline = _emailService.PingServer();
            return new Dictionary<string, bool>
            {
                {"Ok", smptServerIsOnline},
                {"smtpServer", smptServerIsOnline}
            };
        }


        public MainEndpoint()
        {
            var smtpClient = new SmtpClient("localhost", 25);
            _emailService = new EmailService(smtpClient);

            Get["/"] = args => Response.AsRedirect("/health");

            Post["/email"] = args =>
            {
                var data = this.Bind<EmailMessage>();
                var emailSendResult = SendEmail(data);
                return Response.AsJson(string.Empty, emailSendResult ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
            };

            // it is important that service lets us know what is faulting
            Get["/health"] = args => Response.AsJson(HealthCheck());
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
                f.SetDescription("Workshop EmailService");
                f.SetDisplayName("Workshop EmailService");
                f.SetServiceName("Workshop EmailService");
            });
        }
    }
}
