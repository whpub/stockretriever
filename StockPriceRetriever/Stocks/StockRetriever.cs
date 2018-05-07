using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StockPriceRetriever.Stocks.Models;

namespace StockPriceRetriever.Stocks
{
    public class StockRetriever
    {
	    const string NasdaqLink = "https://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange={0}&render=download";

	    private enum Exchanges
	    {
			NASDAQ,
			NYSE,
			AMEX
	    }


		public async Task<IEnumerable<StockInformation>> RetrieveAllStockInformationAsync()
		{
			var symbolInfos = new List<StockInformation>();
			using (var handler = new HttpClientHandler{CookieContainer = new CookieContainer()})
			{
				using (HttpClient client = new HttpClient(handler))
				{
					client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
					client.DefaultRequestHeaders.Host = "www.nasdaq.com";
					client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
					client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoCache = true};
					client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
					client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip,deflate,br");
					client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
					client.DefaultRequestHeaders.UserAgent.ParseAdd(
						"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359 Safari/537.36");

					foreach (string exchange in Enum.GetNames(typeof(Exchanges)))
					{
						using (var stream = await client.GetStreamAsync(string.Format(NasdaqLink, exchange)))
						{
							using (var reader = new StreamReader(stream))
							{
								using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.Configuration
								{
									HasHeaderRecord = true,
									PrepareHeaderForMatch = x =>
										!string.IsNullOrEmpty(x) ? x.First().ToString().ToUpper() + x.Substring(1) : string.Empty
								}))
								{
									symbolInfos.AddRange(csv.GetRecords<SymbolInformation>().Select(x => new StockInformation
									{
										Price = Regex.IsMatch(x.LastSale, "[0-9]+") ? double.Parse(x.LastSale) : -1,
										Symbol = x.Symbol,
										TimeStamp = DateTime.Now
									}));
								}
							}
						}
					}
				}
			}

			return symbolInfos;
	    }

    }
}
