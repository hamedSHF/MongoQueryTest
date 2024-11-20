using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Random rand = new Random();
            MongoQueryDb queryDb = new MongoQueryDb();
            await queryDb.CreateAndFillCollection();
            var list = new List<double>();
            for(int i = 0;i < 12;i++)
            {
                list.Add(rand.NextDouble());    
            }
            queryDb.ExecuteFirstQuery(10, list);
        }
    }
}
