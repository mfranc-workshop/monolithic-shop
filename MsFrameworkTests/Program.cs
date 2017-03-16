using System;
using Nancy;
using Nancy.Hosting.Self;
using Topshelf;

namespace MsFrameworkTests
{
    public sealed class ProfileStatsEndpoint : NancyModule
    {
        public ProfileStatsEndpoint()
        {
            Get["/"] = args => Response.AsJson(true);
        }
    }

    public class Microservice
    {
        private readonly int _portNumber;
        private NancyHost _nancyHost;

        public Microservice(int portNumber)
        {
            _portNumber = portNumber;
        }

        public void Start()
        {
            var cfg = new HostConfiguration();
            cfg.RewriteLocalhost = true;
            cfg.UrlReservations.CreateAutomatically = true;

            _nancyHost = new NancyHost(new Uri($"http://localhost:{_portNumber}"), new DefaultNancyBootstrapper(), cfg);
            _nancyHost.Start();

        }

        public void Stop()
        {
            _nancyHost.Stop();
            Console.WriteLine("Stopped. Good bye!");
        }
    }

    class Program
    {
        // http://blog.amosti.net/self-hosted-http-service-in-c-with-nancy-and-topshelf/

        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<Microservice>(s =>
                {
                    s.ConstructUsing(name => new Microservice(5000));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalService();
                x.SetDescription("Nancy-SelfHost example");
                x.SetDisplayName("Nancy-SelfHost Service");
                x.SetServiceName("Nancy-SelfHost");
            });
        }
    }
}
