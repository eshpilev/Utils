using System.Text;

namespace Utils.Helpers
{
    /// <summary>Декоратор StringBuilder с ограничением кол-ва строк </summary>
    public class StringBuilderLimited
    {
        private const int LimitDefault = 1000;
        public int Count { get; set; }
        public int Limit { get; }
        public StringBuilder StringBuilder { get; }
        public StringBuilderLimited(int limit = LimitDefault)
        {
            Limit = limit;
            StringBuilder = new StringBuilder();
        }

        public void AppendLine(string message)
        {
            if (Count++ < Limit)
                StringBuilder.AppendLine($"{Count}. {message}");
        }

        public override string ToString()
        {
            if (Count > Limit)
                StringBuilder.Insert(0, $"ВНИМАНИЕ! Список сообщений переполнен! Отображено {Limit} из {Count}:{Environment.NewLine}");

            return StringBuilder.ToString();
        }
    }
}