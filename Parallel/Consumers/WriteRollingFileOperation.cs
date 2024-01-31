using ParallelPages.Models;

namespace ParallelPages.Consumers
{
    public class WriteRollingFileOperation : IConsumer<StateInfo>, IDisposable
    {
        public int MaxLines { get; set; }
        public string FileName { get; set; }

        private StreamWriter _writer;
        private long _counter;

        public void Consume(StateInfo item)
        {
            if (item is null)
                return;

            if (_writer == null || _counter % MaxLines == 0)
            {
                if (_writer != null)
                {
                    _writer.Close();
                    _writer = null;
                }
                var fileName =
                    Path.Combine(Path.GetDirectoryName(FileName) ?? string.Empty,
                        $"{Path.GetFileNameWithoutExtension(FileName)}_{DateTime.Now:yyyyMMdd_HHmmss}_" +
                        $"{_counter / MaxLines}{Path.GetExtension(FileName)}");
                _writer = new StreamWriter(fileName);
            }
            _writer.WriteLine($"{item.Page};{item.Offset};{item.Num};{item.ThreadNum};{item.ActualDateTime};{item.Text}");
            _counter++;
        }
        public void Dispose() => _writer?.Close();
    }
}
