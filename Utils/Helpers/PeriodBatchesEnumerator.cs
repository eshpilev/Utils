using System.Collections;

namespace Utils.Helpers
{
    /// <summary> Итератор возвращающий диапазон строк с разбивкой по периоду </summary>
    public class PeriodBatchesEnumerator<T> : IEnumerator<IEnumerable<T>> where T : IRecord
    {
        public PeriodBatchesSettings Settings { get; set; }
        public IOrderedQueryable<T> Query { get; set; }
        private DateTime _fromDate;
        private DateTime _toDate;
        private bool _getNextPeriod;
        private int _batch;
        public PeriodBatchesEnumerator(PeriodBatchesSettings settings)
        {
            ValidateSettings(settings);
            Settings = settings;
            _fromDate = InitFromDate();
            _toDate = InitToDate();
        }
        private void ValidateSettings(PeriodBatchesSettings settings)
        {
            if (settings.BatchSize <= 0)
                throw new ArgumentException("Не задан размер одной пачки");
            if (settings.FromDate == DateTime.MinValue || settings.FromDate == DateTime.MaxValue)
                throw new ArgumentException("Не задана минимальная дата");
            if (settings.ToDate == DateTime.MinValue || settings.ToDate == DateTime.MaxValue)
                throw new ArgumentException("Не задана максимальная дата");
            if (settings.FromDate > settings.ToDate)
                throw new ArgumentException("Минимальная дата больше максимальной даты");
            if (Query != null)
                throw new ArgumentNullException(nameof(Query));
        }
        public IEnumerable<T> Current
        {
            get
            {
                var messages = Query
                    .Where(x => x.CreateDateTime >= _fromDate && x.CreateDateTime < _toDate)
                    .Skip(_batch * Settings.BatchSize)
                    .Take(Settings.BatchSize)
                    .ToArray();
                _getNextPeriod = messages.Length < Settings.BatchSize || messages.Length == 0;
                _batch++;
                return messages;
            }
        }
        public bool MoveNext()
        {
            if (_getNextPeriod)
            {
                _batch = 0;
                _fromDate = _toDate;
                _toDate = InitToDate();
            }
            return _fromDate <= Settings.ToDate;
        }
        private DateTime InitFromDate() =>
            Settings.Periodicity switch
            {
                Periodicity.Daily => Settings.FromDate.Date,
                Periodicity.Monthly => new DateTime(Settings.FromDate.Year, Settings.FromDate.Month, 1),
                Periodicity.Yearly => new DateTime(Settings.FromDate.Year, 1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(Settings.Periodicity), Settings.Periodicity, null)
            };

        private DateTime InitToDate() =>
            Settings.Periodicity switch
            {
                Periodicity.Daily => _fromDate.AddDays(1),
                Periodicity.Monthly => _fromDate.AddMonths(1),
                Periodicity.Yearly => _fromDate.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(Settings.Periodicity), Settings.Periodicity, null)
            };

        public void Reset()
        {
            _fromDate = InitFromDate();
            _toDate = InitToDate();
            _batch = 0;
        }
        object IEnumerator.Current => Current;
        public void Dispose() { }
    }

    public class PeriodBatchesEnumeratorDemo : PeriodBatchesEnumerator<DemoRecord>
    {
        public PeriodBatchesEnumeratorDemo(PeriodBatchesSettings settings) : base(settings)
        {
        }
        public static void Run()
        {
            var fromDate = new DateTime(2024, 01, 01);
            var toDate = new DateTime(2024, 12, 31);
            using var batches = new PeriodBatchesEnumerator<DemoRecord>(new PeriodBatchesSettings
            {
                BatchSize = 10,
                Periodicity = Periodicity.Monthly,
                FromDate = fromDate,
                ToDate = toDate
            });
            var records = new List<DemoRecord> { };
            var i = 0;
            for (var currentDate = fromDate; currentDate < toDate; currentDate = currentDate.AddDays(1))
            {
                records.Add(new DemoRecord
                {
                    Hash = $"{i++}",
                    CreateDateTime = currentDate
                });
            }
            batches.Query = records.AsQueryable().OrderBy(x => x.Hash);
            var batchNum = 0;
            while (batches.MoveNext())
            {
                Console.WriteLine($"BatchNum:{batchNum}");
                foreach (var record in batches.Current!)
                    Console.WriteLine($"Hash:{record.Hash}, CreateDateTime:{record.CreateDateTime}");
                batchNum++;
            }
        }
    }

    public class PeriodBatchesSettings
    {
        public int BatchSize { get; set; }
        public Periodicity Periodicity { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public interface IRecord
    {
        public DateTime CreateDateTime { get; set; }
    }
    public enum Periodicity
    {
        Daily,
        Monthly,
        Yearly
    }
    public struct DemoRecord : IRecord
    {
        public string Hash { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
