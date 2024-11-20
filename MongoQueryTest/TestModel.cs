using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest
{
    public class TestModel
    {
        public BsonObjectId Id { get; set; }
        public List<double> TestArray { get; set; }
        [BsonIgnore]
        public double Sum { get; set; }
    }
}
