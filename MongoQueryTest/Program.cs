using MongoQueryTest.SecondStartegy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest
{
    public class Program
    {
        private static Random random = new Random();
        static async Task Main(string[] args)
        {
            //MongoFirstDb queryDb = new MongoFirstDb();
            //await queryDb.CreateAndFillCollection();
            var list = generateTargetList();
            //var list1 = await queryDb.ExecuteFirstQuery(10, list);
            //var list2 = await queryDb.ExecuteSecondQuery(0.5, list);
            MongoSecondDb secondQuery = new MongoSecondDb();
            await secondQuery.CreateAndFillCollection();
            var list3 = await secondQuery.ExecuteFirstQuery(10,list);
        }
        private static List<double> generateTargetList()
        {
            var list = new List<double>();
            for (int i = 0; i < 12; i++)
            {
                list.Add(random.NextDouble());
            }
            return list;
        }
    }
}
