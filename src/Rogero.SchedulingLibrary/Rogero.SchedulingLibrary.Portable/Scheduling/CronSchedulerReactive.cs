using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Rogero.SchedulingLibrary.Streams;

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
            if (!_schedulerStarted) _cronSchedulerCallback.Start(() => _schedulerCallbackObservable.OnNext(Unit.Default));
            return _schedulerCallbackObservable;
        }
    }

    public class CronSchedulerStreamReactive
    {
        public IObservable<CronTime> SchedulerObservable => GetEventStream();
        public IEnumerable<CronTime> UpcomingEvents => _cronSchedulerStream.UpcomingEvents;
        public CronTime LastFiredSchedule => _cronSchedulerStream.LastFiredEvent;

        private readonly CronSchedulerStream _cronSchedulerStream;
        private bool _schedulerStarted = false;
        private readonly Subject<CronTime> _schedulerCallbackObservable = new Subject<CronTime>();

        public CronSchedulerStreamReactive(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTimeStreamBase cronTimeStream)
        {
            _cronSchedulerStream = new CronSchedulerStream(dateTimeRepository, scheduler, cronTimeStream);
        }

        private IObservable<CronTime> GetEventStream()
        {
            if (!_schedulerStarted) _cronSchedulerStream.Start((cronTime) => _schedulerCallbackObservable.OnNext(cronTime));
            _schedulerStarted = true;
            return _schedulerCallbackObservable;
        }
    }
}