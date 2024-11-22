using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest.Utils
{
    public class CommonModel
    {
        public BsonObjectId Id { get; set; }
        public List<double> TestArray { get; set; }
    }
}
