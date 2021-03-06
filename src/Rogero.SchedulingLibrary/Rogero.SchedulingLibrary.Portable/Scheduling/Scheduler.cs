using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Streams;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public static class DateTimeFormats
    {
        public const string DatetimeFormat = "yyyy-MM-dd  hh:mm:ss tt";
        public const string DatetimeFormatWithDayOfWeek = "yyyy-MM-dd  hh:mm:ss tt ddd";
    }
    public class Scheduler
    {
        public IEnumerable<CronTime> UpcomingEvents => _cronTimeStream.AdvanceTo(_dateTimeRepository.Now());
        public CronTime LastFiredEvent { get; private set; }

        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly IScheduler _scheduler;
        private readonly CronTimeStreamBase _cronTimeStream;
        private readonly bool _callbackOnScheduler;

        private Action<CronTime> _eventCallback;
        private IEnumerator<CronTime> _internalStream;
        private CronTime _nextCronTime;
        private IDisposable _scheduledCallback;

        public Scheduler(IDateTimeRepository dateTimeRepository,  IScheduler scheduler, CronTimeStreamBase cronTimeStream, bool callbackOnScheduler = false)
        {
            _dateTimeRepository = dateTimeRepository;
            _scheduler = scheduler;
            _cronTimeStream = cronTimeStream;
            _callbackOnScheduler = callbackOnScheduler;
        }

        public void Start(Action<CronTime> eventCallback)
        {
            try
            {
                Logger.Log($"{GetNowTimestampForLogging()} >>> Public start called");
                _eventCallback = eventCallback;
                _internalStream = _cronTimeStream.AdvanceTo(_dateTimeRepository.Now()).GetEnumerator();
                Run();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void Run()
        {
            Logger.Log($"{GetNowTimestampForLogging()} >>> Starting set callback");
            if (_internalStream.Current == null && !_internalStream.MoveNext())
            {
                Logger.Log($"{GetNowTimestampForLogging()} >>> The stream has no current value and move next is false");
                return;
            }
            while (true)
            {
                Logger.Log($"{GetNowTimestampForLogging()} >>> Beginning while loop inside of SetCallback");
                _nextCronTime = _internalStream.Current;
                Logger.Log($"{GetNowTimestampForLogging()} >>> _nextCronTime = {_nextCronTime.DateTime.Value.ToString(DateTimeFormats.DatetimeFormat)}");
                Logger.Log($"{GetNowTimestampForLogging()} >>> Determining of the _nextCronTimeIsValid");
                var nextCronTimeValid = _nextCronTime.DateTime.HasValue &&
                                        _nextCronTime.DateTime.Value >= _dateTimeRepository.Now();
                Logger.Log($"{GetNowTimestampForLogging()} >>> _nextCronTimeIsValid = {nextCronTimeValid}");
                if (nextCronTimeValid)
                {
                    Logger.Log($"{GetNowTimestampForLogging()} >>> Next callback is valid and setting a callback for {_nextCronTime.DateTime.Value}");
                    ScheduleCallback(_nextCronTime);
                    _internalStream.MoveNext();
                    return;
                }
                else
                {
                    Logger.Log($"{GetNowTimestampForLogging()} >>> Next callback time is invalid.");
                    var moveNextSuccessful = _internalStream.MoveNext();
                    if (!moveNextSuccessful)
                    {
                        Logger.Log($"{GetNowTimestampForLogging()} >>> Attempted to move to next time and there were no elements.");
                        return;
                    }
                    Logger.Log($"{GetNowTimestampForLogging()} >>> Successfully MovedNext() to begin the while loop again.");
                }
            }
        }

        private string GetNowTimestampForLogging()
        {
            return _dateTimeRepository.Now().ToString(DateTimeFormats.DatetimeFormat);
        }

        private void ScheduleCallback(CronTime nextCronTime)
        {
            var timeUntilDue = nextCronTime.DateTime.Value - _dateTimeRepository.Now();
            Logger.Log($"{GetNowTimestampForLogging()} >>> Setting callback for {timeUntilDue}.");
            _scheduledCallback = _scheduler.Schedule(new object(), timeUntilDue, (scheduler, state) => SendEvent());
        }

        private void SendEvent()
        {
            Logger.Log($"{GetNowTimestampForLogging()} >>> Sending event for {_nextCronTime.DateTime.Value.ToString(DateTimeFormats.DatetimeFormat)}.");
            var cronTime = _nextCronTime;
            LastFiredEvent = cronTime;

            if (_callbackOnScheduler)
                try
                {
                    _eventCallback(cronTime);
                }
                catch (Exception e)
                {
                    
                }
            else
                Task.Run(() => _eventCallback(cronTime));

            Run();
        }

        public void Stop()
        {
            _scheduledCallback.Dispose();
        }
    }
}