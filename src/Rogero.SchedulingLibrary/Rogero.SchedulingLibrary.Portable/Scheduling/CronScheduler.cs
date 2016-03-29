using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Rogero.Option;

namespace Rogero.SchedulingLibrary.Scheduling
{
    public class CronSchedulerReactive
    {
        private bool _schedulerStarted = false;

        private readonly Subject<Unit> _schedulerCallbackObservable = new Subject<Unit>();
        private readonly CronSchedulerCallback _cronSchedulerCallback;

        public CronSchedulerReactive(IDateTimeRepository dateTimeRepository, IScheduler scheduler, CronTemplate cronTemplate)
        {
            _cronSchedulerCallback = new CronSchedulerCallback(dateTimeRepository, scheduler, cronTemplate);
        }

        public IObservable<Unit> GetSchedulerObservable()
        {
            if(!_schedulerStarted) _cronSchedulerCallback.Start(() => _schedulerCallbackObservable.OnNext(Unit.Default));
            return _schedulerCallbackObservable;
        }
    }

    public class CronSchedulerCallback
    {
        private readonly IDateTimeRepository _dateTimeRepository;
        private readonly CronTemplate _cronTemplate;
        private readonly IScheduler _scheduler;
        private IList<CronTime> _cronTimeQueue = new List<CronTime>();
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
            var cronTimes = CronTimeGenerator.CreateByLesserOfTimeSpanOrCountWithMinimum(_cronTime,
                                                                                         TimeSpan.FromMinutes(5), 10,
                                                                                         _desiredCronQueueSize);
            _cronTimeQueue.AddRange(cronTimes);
            _cronTimeQueue = _cronTimeQueue.OrderBy(z => z.Time).ToList();
        }

        private void Start()
        {
            Logger.Log($"Starting:         {_dateTimeRepository.Now():O}");

            var cronTimesDueNow = GetDueCronTimes();
            if (cronTimesDueNow.HasValue)
            {
                RemoveCronTimes(cronTimesDueNow.Value);
                SendCallbackToClient();
            }
            TopOfCronTimeQueue();
            SetNextRunTime();
        }

        private Option<IList<CronTime>>  GetDueCronTimes()
        {
            var dueCronTimes = _cronTimeQueue.Where(z => z.DateTime < _dateTimeRepository.Now()).ToList();
            if (dueCronTimes.Count == 0) return Option<IList<CronTime>>.None;
            return dueCronTimes;
        }

        private void RemoveCronTimes(IList<CronTime> dueCronTimes)
        {
            _cronTimeQueue = _cronTimeQueue.Except(dueCronTimes).OrderBy(z => z.Time).ToList();
        }

        private void SendCallbackToClient()
        {
            Logger.Log($"Sending Callback: {_dateTimeRepository.Now():O}");
            Task.Run(() => _callBack());
        }

        private void TopOfCronTimeQueue()
        {
            var currentQueueSize = _cronTimeQueue.Count;
            var newCronTimesNeeded = _desiredCronQueueSize - currentQueueSize;
            var lastCronTime = _cronTimeQueue.LastOrDefault() ?? new CronTime(_cronTemplate, _dateTimeRepository.Now());
            var newCronTimes = CronTimeGenerator.Generate(lastCronTime).Take(newCronTimesNeeded);
            _cronTimeQueue.AddRange(newCronTimes);
        }

        private void SetNextRunTime()
        {
            var timeUntilFirstDue = _cronTimeQueue.First().DateTime.Value.Subtract(_dateTimeRepository.Now());

            var timeUntilTimerFires = (timeUntilFirstDue < TimeSpan.FromMilliseconds(300))
                ? timeUntilFirstDue
                : MultipleTimespanByConstant(timeUntilFirstDue, 0.9);
            SetTimer(timeUntilTimerFires);
        }

        private static TimeSpan MultipleTimespanByConstant(TimeSpan timeUntilFirstDue, double factor)
        {
            return new TimeSpan((long) ((double) timeUntilFirstDue.Ticks*factor));
        }

        private void SetTimer(TimeSpan timeUntilTimerFires)
        {
            Logger.Log($"Set Timer:        {timeUntilTimerFires:G}");
            var stateToIgnore = (object) null;
            _scheduler.Schedule(new object(), timeUntilTimerFires, (x, y) => Start());
        }
    }
}
