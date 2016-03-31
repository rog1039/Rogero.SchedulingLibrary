using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Rogero.SchedulingLibrary.Streams;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class ReactiveScheduler
    {
        public IObservable<CronTime> SchedulerObservable => GetEventStream();
        public IEnumerable<CronTime> UpcomingEvents => _scheduler.UpcomingEvents;
        public CronTime LastFiredSchedule => _scheduler.LastFiredEvent;

        private readonly Scheduler _scheduler;
        private bool _schedulerStarted = false;
        private readonly Subject<CronTime> _schedulerCallbackObservable = new Subject<CronTime>();

        public ReactiveScheduler(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTimeStreamBase cronTimeStream)
        {
            _scheduler = new Scheduler(dateTimeRepository, scheduler, cronTimeStream);
        }

        private IObservable<CronTime> GetEventStream()
        {
            if (!_schedulerStarted) _scheduler.Start((cronTime) => _schedulerCallbackObservable.OnNext(cronTime));
            _schedulerStarted = true;
            return _schedulerCallbackObservable;
        }
    }
}