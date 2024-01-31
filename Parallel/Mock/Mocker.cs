using ParallelPages.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace ParallelPages.Mock
{
    public static class Mocker
    {
        public static readonly string BigText = string.Join("_____", Enumerable.Range(0, 1000));

        public static WireMockServer CreateWireMockServer(string url, int limit, int total, int delay)
        {
            var server = WireMockServer.Start(
                new WireMockServerSettings { Urls = new[] { url } });

            var offset = 0;
            while (offset < total)
            {
                var count = Math.Min(limit, total - offset);
                var items = new StateInfo[count];
                for (var i = 0; i < count; i++)
                {
                    items[i] = new StateInfo
                    {
                        Page = offset / limit,
                        Offset = offset,
                        Num = i,
                        ThreadNum = Thread.CurrentThread.ManagedThreadId,
                        Text = BigText,
                        ActualDateTime = DateTime.Now
                    };
                }

                server.Given(
                        Request.Create()
                            .WithPath("/items")
                            .WithParam("limit", $"{limit}")
                            .WithParam("offset", $"{offset}"))
                    .RespondWith(
                        Response.Create()
                            .WithStatusCode(200)
                            .WithHeader("Content-Type", "aplication/json")
                            .WithBodyAsJson(items)
                            .WithDelay(TimeSpan.FromMilliseconds(delay))
                        );
                offset += limit;
            }

            server.Given(
                    Request.Create()
                        .WithPath("/items")
                        .WithParam("limit", $"{limit}")
                        .WithParam("offset", $"{offset}"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(404)
                        .WithHeader("Content-Type", "aplication/json")
                        .WithDelay(TimeSpan.FromMilliseconds(delay))
                );

            return server;
        }
    }
}
