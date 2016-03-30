using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Rogero.Option;
using Rogero.SchedulingLibrary.Generators;
using Rogero.SchedulingLibrary.Infrastructure;
using Rogero.SchedulingLibrary.Streams;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class CronSchedulerCallback
    {
        public IList<CronTime> CronTimeQueue { get; private set; } = new List<CronTime>();
        public CronTime LastFiredSchedule { get; private set; }

        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly CronTemplate _cronTemplate;
        private readonly IScheduler _scheduler;
        private static int _desiredCronQueueSize = 100;
        
        private Action _callBack;
        private CronTime _cronTime;

        public CronSchedulerCallback(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTemplate cronTemplate)
        {
            _dateTimeRepository = dateTimeRepository;
            _cronTemplate = cronTemplate;
            _scheduler = scheduler;
            CreateCronTimeQueue();
        }

        public void Start(Action callback)
        {
            _callBack = callback;
            Initialize();
            SetTimer(TimeSpan.Zero);
        }

        private void Initialize()
        {
            CreateCronTimeQueue();
        }

        private void CreateCronTimeQueue()
        {
            _cronTime = new CronTime(_cronTemplate, _dateTimeRepository.Now());
            var cronTimes = _cronTime
                .ToSeries()
                .ForLesserOfWithMinimum(TimeSpan.FromMinutes(5), count: 10, minReturned: _desiredCronQueueSize);
            CronTimeQueue.AddRange(cronTimes);
            CronTimeQueue = CronTimeQueue.OrderBy(z => z.Time).ToList();
        }

        private void Start()
        {
            Logger.Log($"Starting:         {_dateTimeRepository.Now():O}");

            var cronTimesDueNow = GetDueCronTimes();
            if (cronTimesDueNow.HasValue)
            {
                RemoveCronTimes(cronTimesDueNow.Value);
                TopOfCronTimeQueue();
                SendCallbackToClient();
            }
            TopOfCronTimeQueue();
            SetNextRunTime();
        }

        private Option<IList<CronTime>>  GetDueCronTimes()
        {
            var dueCronTimes = CronTimeQueue.Where(z => z.DateTime < _dateTimeRepository.Now()).ToList();
            if (dueCronTimes.Count == 0) return Option<IList<CronTime>>.None;
            return dueCronTimes;
        }

        private void RemoveCronTimes(IList<CronTime> dueCronTimes)
        {
            CronTimeQueue = CronTimeQueue.Except(dueCronTimes).OrderBy(z => z.Time).ToList();
            LastFiredSchedule = dueCronTimes.OrderByDescending(z => z.Time).First();
        }

        private void SendCallbackToClient()
        {
            Logger.Log($"Sending Callback: {_dateTimeRepository.Now():O}");
            Task.Run(() => _callBack());
        }

        private void TopOfCronTimeQueue()
        {
            var currentQueueSize = CronTimeQueue.Count;
            var newCronTimesNeeded = _desiredCronQueueSize - currentQueueSize;
            var lastCronTime = CronTimeQueue.LastOrDefault() ?? new CronTime(_cronTemplate, _dateTimeRepository.Now());
            var newCronTimes = lastCronTime.ToSeries().Take(newCronTimesNeeded);
            CronTimeQueue.AddRange(newCronTimes);
        }

        private void SetNextRunTime()
        {
            var timeUntilFirstDue = CronTimeQueue.First().DateTime.Value.Subtract(_dateTimeRepository.Now());

            var timeUntilTimerFires = (timeUntilFirstDue < TimeSpan.FromMilliseconds(300))
                ? timeUntilFirstDue
                : MultipleTimespanByConstant(timeUntilFirstDue, 0.9);
            SetTimer(timeUntilTimerFires);
        }

        private static TimeSpan MultipleTimespanByConstant(TimeSpan timeUntilFirstDue, double factor)
        {
            return new TimeSpan((long) (timeUntilFirstDue.Ticks*factor));
        }

        private void SetTimer(TimeSpan timeUntilTimerFires)
        {
            Logger.Log($"Set Timer:        {timeUntilTimerFires:G}");
            _scheduler.Schedule(state: (object)null, dueTime: timeUntilTimerFires, action: (x, y) => Start());
        }
    }

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
            Task.Run(() => _eventCallback(cronTime));
            SetCallback();
        }
    }
}
