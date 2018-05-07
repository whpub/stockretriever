using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace StockPriceRetriever.Repositories.Models
{
    public class StockInfoEntity : TableEntity
    {
		public const string PartKey = "1";

		public StockInfoEntity(string symbol)
	    {
		    PartitionKey = PartKey;
		    RowKey = symbol.ToUpper();
	    }

	    public StockInfoEntity()
	    {
	    }

		public double Price { get; set; }

		public DateTime TimeStamp { get; set; }
    }
}
