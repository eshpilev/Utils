using System.Diagnostics;
using ParallelPages.Consumers;
using ParallelPages;
using ParallelPages.Mock;
using ParallelPages.Models;
using ParallelPages.Producers;

//Mocker params
const string url = "http://localhost:64515"; //заглушка url 
const int pageSize = 100; //кол-во элементов на одной странице 
const int total = 100000;  //общее кол-во элементов  
const int delay = 100; // время ответа API
//Producer params
const int maxDegreeOfParallelism = 8; //кол-во потоков загрузки данных
//Consumer params
const string fileName = @"c:\Work\items.csv";
const int maxLinesFile = 10000;


var server = Mocker.CreateWireMockServer(url, pageSize, total, delay);

try
{
    using var loader = new ApiLoader { Url = url };
    using var writer = new WriteRollingFileOperation
    {
        MaxLines = maxLinesFile,
        FileName = fileName
    };

    Console.WriteLine(
        $"Total:{total}, Delay:{delay}, PageSize:{pageSize}, MaxDegreeOfParallelism:{maxDegreeOfParallelism}");

    var sw = Stopwatch.StartNew();

    new ParallelPagesProcessor<StateInfo>
    {
        Producer = loader,
        Consumer = writer,
        PageSize = pageSize,
        MaxDegreeOfParallelism = maxDegreeOfParallelism
    }.Run();

    Console.WriteLine($"Elapsed: {sw.Elapsed}");
}

finally
{
    if (server is { IsStarted: true })
        server.Stop();
}


