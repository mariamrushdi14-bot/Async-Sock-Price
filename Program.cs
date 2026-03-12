namespace Async_Stock_price
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           var cts = new CancellationTokenSource();
            var fatcher = new StockPriceFetcher();
            var Sources = new List<string>
            {
                "NYSE",
                "NASDAQ",
                "LSE",
                "TSE",
                "SSE"
            };
            Console.WriteLine("Starting stock fetch prices . press 'c' to cancele  ");
            var task=fatcher.FetchAllPrice(Sources, cts.Token);
            while (!task.IsCompleted)
            {
                if (Console.ReadKey().KeyChar == 'c')
                {
                    cts.Cancel();
                    break;
                }
                await Task.Delay(200);
                try
                {
                    var prices = await task;
                    Console.WriteLine("Successful price :");
                    foreach (var item in prices)
                    {
                        Console.WriteLine(item);
                    }
                    if (prices.Any())
                    {
                        Console.WriteLine($"Average prices is {prices.Average()}");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("opertion is canceled");
                }

            }
        }
    }
    public class StockPriceFetcher
    {
       
        public async Task<decimal> GetStockPriceAsync(string sourceName , CancellationToken token)
        {
             Random random= new Random();
            int delay = random.Next(500, 3000);
            await Task.Delay(delay, token);
            if (random.NextDouble()<.1)
            {
                throw new Exception($"Faild to fatch from {sourceName}");
            }
            decimal price = random.Next(100, 500);
            return price;
        }
         public async Task<List<decimal>> FetchAllPrice(IEnumerable<string> sources, CancellationToken token)
        {
            List<Task<decimal>> tasks = new List <Task<decimal>>();
            foreach (string source in sources)
            {
                tasks.Add(GetStockPriceAsync(source, token));

            }
            List<decimal> price = new List<decimal>();
            foreach (var task in tasks)
            {
                try
                {
                   var priceResult=await Task.WhenAll(task);
                    price.AddRange(priceResult);
                }
                catch 
                {
                    Console.WriteLine("Source is Faild");
                }
            }
            return price;

        }

    }
}
