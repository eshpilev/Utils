namespace ParallelPages.Consumers
{
    public interface IConsumer<in T>
    {
        public void Consume(T item);
    }
}
