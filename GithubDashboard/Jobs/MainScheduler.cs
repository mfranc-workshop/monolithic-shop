using System;
using Quartz;
using Quartz.Impl;

namespace GithubDashboard.Jobs
{
    public static class MainScheduler
    {
        public static void Start()
        {
            try
            {
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();
                IJobDetail job = JobBuilder.Create<WarehouseJob>().Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
                        .RepeatForever())
                    .Build();

                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}