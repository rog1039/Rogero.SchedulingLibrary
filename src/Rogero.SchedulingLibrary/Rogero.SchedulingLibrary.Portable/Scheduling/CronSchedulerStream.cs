using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                Logger.Log($"{GetNowTimestampForLogging()} >>> Public start called");
                _eventCallback = eventCallback;
                _internalStream = _cronTimeStream.AdvanceTo(_dateTimeRepository.Now()).GetEnumerator();
                SetCallback();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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
                Logger.Log($"A");
                _nextCronTime = _internalStream.Current;
                Logger.Log($"B");
                var nextCronTimeValid = _nextCronTime.DateTime.HasValue &&
                                        _nextCronTime.DateTime.Value >= _dateTimeRepository.Now();
                Logger.Log($"C");
                if (nextCronTimeValid)
                {
                    Logger.Log($"{GetNowTimestampForLogging()} >>> Next callback is valid and setting a callback for {_nextCronTime.DateTime.Value}");
                    SetCallback(_nextCronTime);
                    _internalStream.MoveNext();
                    return;
                }
                else
                {
                    Logger.Log($"E");
                    var moveNextSuccessful = _internalStream.MoveNext();
                    if (!moveNextSuccessful)
                    {
                        Logger.Log($"F");
                        return;
                    }

                    Logger.Log($"G");
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
            Logger.Log($"D");
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