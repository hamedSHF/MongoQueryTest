using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest.Utils
{
    public static class MongoUtils
    {
        private static Random random = new Random();
        public static List<double> GenerateArray(int sizeOfArray)
        {
            var list = new List<double>(sizeOfArray);
            for (int j = 0; j < sizeOfArray; j++)
            {
                list.Add(random.NextDouble());
            }
            return list;
        }
    }
}
