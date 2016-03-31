using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Rogero.Option;
using Rogero.SchedulingLibrary.Generators;
using Rogero.SchedulingLibrary.Infrastructure;

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
}
