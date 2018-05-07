using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPriceRetriever.Stocks.Models
{
    public class StockInformation
    {
		public string Symbol { get; set; }

		public DateTime TimeStamp { get; set; }

		public double Price { get; set; }
    }
}
