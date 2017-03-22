using System;
using System.Net.Mail;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using MicroShop.Services;
using Quartz.Impl;
using RestEase;
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

            container.Register<IEmailService, EmailService>();
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

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            container.Register<IBus>(() => bus);

            container.Verify();
        }
    }
}