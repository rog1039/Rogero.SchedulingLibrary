using System;
using System.Collections.Generic;
using System.Text;

namespace Rogero.SchedulingLibrary
{
    public interface IDateTimeRepository
    {
        DateTime Now();
    }

    public class CronSchedulerCallback
    {
        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly CronTemplate _cronTemplate;
        private Action _callBack;
        private CronTime _cronTime;

        public CronSchedulerCallback(IDateTimeRepository dateTimeRepository, CronTemplate cronTemplate)
        {
            _dateTimeRepository = dateTimeRepository;
            _cronTemplate = cronTemplate;
            CreateCronTimeQueue();
        }

        public void Start(Action callback)
        {
            _callBack = callback;
            Initialize();
            Start();
        }

        private void Initialize()
        {
            CreateCronTimeQueue();
        }

        private void CreateCronTimeQueue()
        {
            _cronTime = new CronTime(_cronTemplate, _dateTimeRepository.Now());
            var cronTimes = CronTimeGenerator.GenerateForLessorOf(_cronTime, TimeSpan.FromMinutes(5), 100);
        }

        private void Start()
        {
            
        }
    }
}
