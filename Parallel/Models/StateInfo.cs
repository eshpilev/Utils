namespace ParallelPages.Models
{
    public class StateInfo
    {
        public int Page { get; set; }
        public int Offset { get; set; }
        public int Num { get; set; }
        public int ThreadNum { get; set; }
        public string Text { get; set; }
        public DateTime ActualDateTime { get; set; }
    }
}
