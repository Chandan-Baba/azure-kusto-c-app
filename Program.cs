using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
//using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        const string Cluster = "https://democlusterlb.westus.kusto.windows.net";
        const string Database = "lrmtdb";

        static void Main()
        {

            // The query provider is the main interface to use when querying Kusto.
            // It is recommended that the provider be created once for a specific target database,
            // and then be reused many times (potentially across threads) until it is disposed-of.
            var kcsb = new KustoConnectionStringBuilder(Cluster, Database)
                .WithAadUserPromptAuthentication();
            using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb))
            {
                // The query -- Note that for demonstration purposes, we send a query that asks for two different
                // result sets (HowManyRecords and SampleRecords).
                var query = "tblProject| where LanguageId ==2|take 1";

                // It is strongly recommended that each request has its own unique
                // request identifier. This is mandatory for some scenarios (such as cancelling queries)
                // and will make troubleshooting easier in others.
                var clientRequestProperties = new ClientRequestProperties() { ClientRequestId = Guid.NewGuid().ToString() };

                DataSet ds = new DataSet();


                using (var reader = queryProvider.ExecuteQuery(query, clientRequestProperties))
                {
                    // Read HowManyRecords
                    while (reader.Read())
                    {
                        var howManyRecords = reader.GetInt32(0);
                        
                        MyType mType = new MyType();
                        mType.ProjectId = howManyRecords;
                        Console.WriteLine($"There are {mType.ProjectId} records in the table");

                    }

                    ////Suggested by Yoni L
                    var result = queryProvider.ExecuteQuery<MyType>(query, clientRequestProperties).ToList<MyType>();
                    foreach (var item in result)
                    {
                        //Empty result set

                        //item.ProjectName = item.ProjectName;
                        //Console.WriteLine($"The project name is {item.ProjectName} in the table");
                    }
                    
                    // Move on to the next result set, SampleRecords
                    //reader.NextResult();
                    //Console.WriteLine();
                    //while (reader.Read())
                    //{
                    //    // Important note: For demonstration purposes we show how to read the data
                    //    // using the "bare bones" IDataReader interface. In a production environment
                    //    // one would normally use some ORM library to automatically map the data from
                    //    // IDataReader into a strongly-typed record type (e.g. Dapper.Net, AutoMapper, etc.)
                    //    DateTime time = reader.GetDateTime(0);
                    //    string type = reader.GetString(1);
                    //    string state = reader.GetString(2);
                    //    Console.WriteLine("{0}\t{1,-20}\t{2}", time, type, state);
                    //}
                }
                Console.WriteLine();
                Console.ReadLine();


            }
        }

        /// <summary>
        /// Public class created to support ToList() in above method.
        /// </summary>
        public class MyType
        {

            public int ProjectId { get; set; }
         
            public string CustomerName { get; set; }

            public string ProjectName { get; set; }
          
        }
    }
}
