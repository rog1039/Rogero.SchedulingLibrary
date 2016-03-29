using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class CronSchedulerReactive
    {
        public IObservable<Unit> SchedulerObservable => GetSchedulerObservable();
        public IList<CronTime> Next100CronTimes => _cronSchedulerCallback.CronTimeQueue;
        public CronTime LastFiredSchedule => _cronSchedulerCallback.LastFiredSchedule;

        private bool _schedulerStarted = false;

        private readonly Subject<Unit> _schedulerCallbackObservable = new Subject<Unit>();
        private readonly CronSchedulerCallback _cronSchedulerCallback;

        public CronSchedulerReactive(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTemplate cronTemplate)
        {
            _cronSchedulerCallback = new CronSchedulerCallback(dateTimeRepository, scheduler, cronTemplate);
        }

        private IObservable<Unit> GetSchedulerObservable()
        {
            if(!_schedulerStarted) _cronSchedulerCallback.Start(() => _schedulerCallbackObservable.OnNext(Unit.Default));
            return _schedulerCallbackObservable;
        }
    }
}