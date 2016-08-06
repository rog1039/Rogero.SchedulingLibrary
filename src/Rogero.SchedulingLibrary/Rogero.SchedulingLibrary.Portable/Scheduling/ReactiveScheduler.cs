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
        private readonly SubjectWithHookableDispose<CronTime> _schedulerCallbackObservable;

        public ReactiveScheduler(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTimeStreamBase cronTimeStream, bool callbackOnScheduler = false)
        {
            _scheduler = new Scheduler(dateTimeRepository, scheduler, cronTimeStream, callbackOnScheduler);
            _schedulerCallbackObservable = new SubjectWithHookableDispose<CronTime>(DisposedCalled);
        }

        private IObservable<CronTime> GetEventStream()
        {
            if (!_schedulerStarted) _scheduler.Start((cronTime) => _schedulerCallbackObservable.OnNext(cronTime));
            _schedulerStarted = true;
            return _schedulerCallbackObservable;
        }

        private void DisposedCalled(SubjectWithHookableDispose<CronTime> obj)
        {
            _scheduler.Stop();
        }
    }

    public class SubjectWithHookableDispose<T> : IObservable<T>
    {
        private readonly Subject<T> _subject = new Subject<T>();
        private readonly Action<SubjectWithHookableDispose<T>> disposeHookCallback;

        public SubjectWithHookableDispose(Action<SubjectWithHookableDispose<T>> disposeHookCallback)
        {
            this.disposeHookCallback = disposeHookCallback;
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(T value)
        {
            _subject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public void Dispose()
        {
            _subject.Dispose();
        }

        public bool HasObservers
        {
            get { return _subject.HasObservers; }
        }

        public bool IsDisposed
        {
            get { return _subject.IsDisposed; }
        }
    }
}