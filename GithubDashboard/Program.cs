using System;
using System.IO;
using System.Threading;
using GithubDashboard.Jobs;
using Microsoft.AspNetCore.Hosting;
using Quartz;
using Quartz.Impl;

namespace GithubDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
