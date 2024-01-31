namespace ParallelPages.Producers
{
    public interface IProducer<out T> 
    {
        public IEnumerable<T> GetPageData(int limit, int offset);
    }
}
