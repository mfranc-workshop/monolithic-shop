using System;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using SimpleInjector;

namespace GithubDashboard.DI
{
    class SimpleInjectiorJobFactory : SimpleJobFactory
    {
        private readonly Container _container;

        public SimpleInjectiorJobFactory(Container container)
        {
            _container = container;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return (IJob)_container.GetInstance(bundle.JobDetail.JobType);
            }
            catch (Exception e)
            {
                throw new SchedulerException(string.Format("Problem while instantiating job '{0}' from the NinjectJobFactory.", bundle.JobDetail.Key), e);
            }
        }
    }
}