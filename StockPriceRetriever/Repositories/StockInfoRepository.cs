using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StockPriceRetriever.Repositories.Models;

namespace StockPriceRetriever.Repositories
{
    public class StockInfoRepository
    {
	    private readonly string _connectionString;

	    public StockInfoRepository(string connectionString)
	    {
		    _connectionString = connectionString;
	    }

	    private CloudTable InitializeTable()
	    {
		    // Retrieve the storage account from the connection string.
		    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

		    // Create the table client.
		    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

		    // Create the CloudTable object that represents the "people" table.
		    return tableClient.GetTableReference("stocks");
		}

	    public bool InsertMany(IEnumerable<StockInfoEntity> stocks)
	    {
		    try
		    {
			    TableBatchOperation batchOperation = new TableBatchOperation();

			    foreach (var stock in stocks)
			    {
				    batchOperation.InsertOrReplace(stock);
			    }

			    InitializeTable().ExecuteBatch(batchOperation);

			    return true;
		    }
		    catch
		    {
			    return false;
		    }
	    }

	    private void DeleteHistoricTables(DateTime limit)
	    {
		    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

		    // Create the table client.
		    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

		    for (var i = 1; i < 7; i++)
		    {
			    if (!tableClient.GetTableReference($"stocks-{limit.AddDays(-i).Date}").DeleteIfExists())
				    break;
			}
		}

	    public IEnumerable<StockInfoEntity> GetAll()
	    {
		    try
		    {
			    TableQuery<StockInfoEntity> rangeQuery = new TableQuery<StockInfoEntity>();
			    return InitializeTable().ExecuteQuery(rangeQuery);
		    }
		    catch
		    {
			    return null;
		    }
		}

		public IEnumerable<StockInfoEntity> GetFiltered(int maxPrice)
	    {
		    try
		    {
				TableQuery<StockInfoEntity> rangeQuery = new TableQuery<StockInfoEntity>().Where(
					TableQuery.CombineFilters(
						TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, StockInfoEntity.PartKey),
						TableOperators.And,
						TableQuery.GenerateFilterCondition("Price", QueryComparisons.LessThanOrEqual, maxPrice.ToString())));

			    return InitializeTable().ExecuteQuery(rangeQuery);
			}
		    catch
		    {
			    return null;
		    }
	    }
	}
}
