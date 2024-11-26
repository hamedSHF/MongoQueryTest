using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoQueryTest.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

namespace MongoQueryTest
{
    public class MongoFirstDb
    {
        private const string dataBaseName = "TestDb";
        private const string collectionName = "TestCollection";
        private BsonDocument sortPipeLine;
        private readonly int sizeOfArray;
        private readonly IMongoDatabase database;
        private IMongoCollection<CommonModel> collection;
        public MongoFirstDb(int sizeOfArray = 12)
        {
            this.sizeOfArray = sizeOfArray;
            database = MongoConnection.Client.GetDatabase(dataBaseName);
            collection = database.GetCollection<CommonModel>(collectionName);
            sortPipeLine = new BsonDocument("$sort", new BsonDocument("distance", 1));
        }
        public async Task CreateAndFillCollection(string collectionName = collectionName,bool capped = true,int size = 100_000)
        {
            
            if(!(await database.ListCollectionNamesAsync()).ToList().Contains(collectionName))
            {
                await database.CreateCollectionAsync(collectionName,new CreateCollectionOptions { Capped = capped,MaxDocuments = size,MaxSize = int.MaxValue });
                collection = database.GetCollection<CommonModel>(collectionName);
                await insertDocuments(collection, size);
            }
            else
            {
                collection = database.GetCollection<CommonModel>(collectionName);
                if((await collection.CountDocumentsAsync(FilterDefinition<CommonModel>.Empty)) == 0)
                {
                    await insertDocuments(collection, size);
                }
            }
        }
        /// <summary>
        /// Return first <paramref name="number"/> records which have the least euclidean distance from target <paramref name="list"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<ResultModel>> ExecuteFirstQuery(int number, List<double> list)
        {
            var query = generateMongoQuery(list);
            var bsonDoc = BsonDocument.Parse(query);
            return await collection.Aggregate<ResultModel>(new[] { bsonDoc, sortPipeLine, new BsonDocument("$limit", number) }).ToListAsync();
        }
        /// <summary>
        /// Return all records which their euclidean distances from target <paramref name="list"/> are less than <paramref name="number"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<ResultModel>> ExecuteSecondQuery(double number,List<double> list)
        {
            var query = generateMongoQuery(list);
            var bsonDoc = BsonDocument.Parse(query);
            return await collection.Aggregate<ResultModel>(new[] { bsonDoc,
                new BsonDocument("$match",new BsonDocument("distance",new BsonDocument("$lt",number))) }).ToListAsync();
        }
        private string generateMongoQuery(List<double> targetList)
        {
            JObject result = JObject.Parse(File.ReadAllText("FirstStrategy\\FirstQuery.json"));
            var targetArray = (JArray)result["$addFields"]["distance"]["$let"]["vars"]["pow"]["$reduce"]["input"]["$zip"]["inputs"][0];
            foreach(var item in targetList)
            {
                targetArray.Add(item);
            }
            return result.ToString();
        }
        private async Task insertDocuments(IMongoCollection<CommonModel> collection,int size)
        {
            if (size > 10_000)
            {
                var numberOfBatches = Math.Floor(size / 10_000d);
                var mod = size % 10_000;
                for (int i = 1; i <= numberOfBatches; i++)
                {
                    var generated = generateData(i * 10_000);
                    await collection.InsertManyAsync(generated);
                }
                if (mod != 0)
                {
                    await collection.InsertManyAsync(generateData(mod * 10_000));
                }
            }
            else
            {
                await collection.InsertManyAsync(generateData(size));
            }
        }
        private List<CommonModel> generateData(int count)
        {
            var finalList = new List<CommonModel>(count);
            for (int i = 0; i < count; i++)
            {
                var testModel = new CommonModel();
                testModel.TestArray = Utils.MongoUtils.GenerateArray(sizeOfArray); ;
                finalList.Add(testModel);
            }
            return finalList;
        }
    }
}
