using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest
{
    public class MongoConnection
    {
        private const string connectionString = "mongodb://localhost:27017";
        private static MongoClient client;
        public static MongoClient Client
        {
            get
            {
                if(client == null)
                {
                    client = new MongoClient(connectionString);
                }
                return client;
            }
        }
        private MongoConnection() { }
    }
}
