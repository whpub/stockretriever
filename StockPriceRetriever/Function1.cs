using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using StockPriceRetriever.Stocks;

namespace StockPriceRetriever
{
    public class Function1
    {
	    /*[FunctionName("SyncStockInformation")]
	    public static void RunSyncStockInformation([TimerTrigger("0 0 21 * * SAT")] TimerInfo timer, TraceWriter log)
	    {
		    log.Info($"SyncStockInformation processed at {DateTime.Now}");

	    }

		[FunctionName("UpdateStockPriceData")]
	    public static void RunUpdateStockPriceData([TimerTrigger("0 0 21 * * MON,TUE,WED,THU,FRI")] TimerInfo timer, TraceWriter log)
	    {
		    log.Info($"UpdateStockPriceData processed at {DateTime.Now}");

	    }*/

	    [FunctionName("RetrieveStockData")]
	    public static async Task<HttpResponseMessage> RunHttp(
		    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
		    TraceWriter log)
	    {
		    log.Info($"RetrieveStockData processed at {DateTime.Now}");

			string priceValue = req.GetQueryNameValuePairs()
			    .FirstOrDefault(q => string.Compare(q.Key, "max", StringComparison.OrdinalIgnoreCase) == 0)
			    .Value;

		    bool validMaxPrice = double.TryParse(priceValue, out var maxPrice);

		    StockRetriever retrieve = new StockRetriever();
		    var results = await retrieve.RetrieveAllStockInformationAsync();

			return new HttpResponseMessage(HttpStatusCode.OK)
				{
					RequestMessage = req,
					Content = new StringContent(
						JsonConvert.SerializeObject(!validMaxPrice ? results : results.Where(x => x.Price <= maxPrice && x.Price > 0)),
						Encoding.UTF8,
						"application/json")
				};
	    }
	}
}
