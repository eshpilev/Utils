using System.Collections.Concurrent;
using ParallelPages.Consumers;
using ParallelPages.Producers;

namespace ParallelPages
{
    /// <summary> Параллельная обработка страниц данных </summary>
    /// <para>
    /// Данный класс позволяет параллельно обработать страницы с данными 
    /// в качестве буфера исползуется <see cref="T:System.Collections.Concurrent.BlockingCollection"/>
    /// https://learn.microsoft.com/ru-ru/dotnet/standard/collections/thread-safe/blockingcollection-overview 
    /// </para>
    public class ParallelPagesProcessor<T>
    {
        public int PageSize { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        public IConsumer<T> Consumer { get; set; }
        public IProducer<T> Producer { get; set; }

        public void Run()
        {
            Validate();

            using var buffer = new BlockingCollection<T>(PageSize);

            var loaderTask = Task.Run(() => 
            { 
                LoadPages(page => Producer.GetPageData(PageSize, page * PageSize), buffer);
            });

            foreach (var item in buffer.GetConsumingEnumerable())
                Consumer.Consume(item);
            
            loaderTask.GetAwaiter();
        }

        private void Validate()
        {
            if (PageSize <= 0)
                throw new ArgumentException(nameof(PageSize));

            if (Consumer is null)
                throw new ArgumentNullException(nameof(Consumer));

            if (Producer is null)
                throw new ArgumentNullException(nameof(Producer));
        }

        private IEnumerable<int> LoadPages(Func<int, IEnumerable<T>> loader, BlockingCollection<T> buffer)
        {
            if (MaxDegreeOfParallelism <= 0)
                MaxDegreeOfParallelism = 1;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = MaxDegreeOfParallelism
            };

            var errorPages = new List<int> { };
            try
            {
                Parallel.ForEach(InfinitePages(), parallelOptions, (page, loopState) =>
                {
                    try
                    {
                        var items = loader(page)?.ToArray();
                        if (items != null && items.Any())
                        {
                            foreach (var item in items)
                                buffer.Add(item);
                        }
                        else
                            loopState.Break();
                    }
                    catch
                    {
                        errorPages.Add(page);
                    }                    
                });
            }
            finally
            {
                buffer.CompleteAdding();
            }

            return errorPages;
        }


        private static IEnumerable<int> InfinitePages()
        {
            var page = 0;
            while (true)
                yield return page++;
        }
    }
}
