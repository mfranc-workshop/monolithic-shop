using System.Net.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MicroShop.Services;
using Quartz.Impl;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;

namespace MicroShop.DI
{
    public static class AppExtension
    {
        public static void InitializeContainer(this IApplicationBuilder app, Container container, IHostingEnvironment env)
        {
            container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            container.RegisterMvcControllers(app);

            container.RegisterSingleton<IEmailService, EmailService>();
            container.RegisterSingleton(() => new SmtpClient("localhost", 25));
            container.Register(() =>
            {
                var sched = new StdSchedulerFactory().GetScheduler();
                sched.JobFactory = new SimpleInjectiorJobFactory(container);
                return sched;
            });

            container.Register<IPaymentProvider, PaymentProvider>();
            container.Register<IReportGenerator, ReportGenerator>();
            container.Register<ITransferCheckService, TransferCheckService>();

            container.Verify();
        }
    }
}