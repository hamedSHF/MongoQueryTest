using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MongoQueryTest
{
    public class ResultModel
    {
        public BsonObjectId Id { get; set; }
        public List<double> TestArray { get; set; }
        [BsonElement("distance")]
        public double Distance { get; set; }
    }
}
