using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceRetriever.Stocks.Models
{
	public class SymbolInformation
	{
		public string Symbol { get; set; }
		public string Name { get; set; }
		public string Sector { get; set; }
		public string Industry { get; set; }
		public string LastSale { get; set; }
		public string MarketCap { get; set; }
	}
}
