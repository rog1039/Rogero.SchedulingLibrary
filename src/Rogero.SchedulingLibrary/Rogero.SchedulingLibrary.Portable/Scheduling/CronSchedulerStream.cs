using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Streams;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class CronSchedulerStream
    {
        public IEnumerable<CronTime> UpcomingEvents => _cronTimeStream.AdvanceTo(_dateTimeRepository.Now());
        public CronTime LastFiredEvent { get; private set; }

        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly IScheduler _scheduler;
        private readonly CronTimeStreamBase _cronTimeStream;

        private Action<CronTime> _eventCallback;
        private IEnumerator<CronTime> _internalStream;
        private CronTime _nextCronTime;

        public CronSchedulerStream(IDateTimeRepository dateTimeRepository,  IScheduler scheduler, CronTimeStreamBase cronTimeStream)
        {
            _dateTimeRepository = dateTimeRepository;
            _scheduler = scheduler;
            _cronTimeStream = cronTimeStream;
        }

        public void Start(Action<CronTime> eventCallback)
        {
            Logger.Log($"{GetNowTimestampForLogging()} >>> Public start called");
            _eventCallback = eventCallback;
            _internalStream = _cronTimeStream.AdvanceTo(_dateTimeRepository.Now()).GetEnumerator();
            SetCallback();
        }

        private void SetCallback()
        {
            Logger.Log($"{GetNowTimestampForLogging()} >>> Starting set callback");
            if (_internalStream.Current == null && !_internalStream.MoveNext())
            {
                Logger.Log($"{GetNowTimestampForLogging()} >>> The stream has no current value and move next is false");
                return;
            }
            while (true)
            {
                _nextCronTime = _internalStream.Current;
                var nextCronTimeValid = _nextCronTime.DateTime.HasValue &&
                                        _nextCronTime.DateTime.Value >= _dateTimeRepository.Now();
                if (nextCronTimeValid)
                {
                    Logger.Log($"{GetNowTimestampForLogging()} >>> Next callback is valid and setting a callback for {_nextCronTime.DateTime.Value}");
                    SetCallback(_nextCronTime);
                    _internalStream.MoveNext();
                    return;
                }
                else
                {
                    var moveNextSuccessful = _internalStream.MoveNext();
                    if (!moveNextSuccessful) return;
                    return;
                }
                
            }
        }

        private string GetNowTimestampForLogging()
        {
            return _dateTimeRepository.Now().ToString("yyyy-MM-dd  hh:mm:ss tt");
        }

        private void SetCallback(CronTime nextCronTime)
        {
            var timeUntilDue = nextCronTime.DateTime.Value - _dateTimeRepository.Now();
            _scheduler.Schedule(new object(), timeUntilDue, (scheduler, state) => SendEvent());
        }

        private void SendEvent()
        {
            var cronTime = _nextCronTime;
            LastFiredEvent = cronTime;
            Task.Run(() => _eventCallback(cronTime));
            SetCallback();
        }
    }
}