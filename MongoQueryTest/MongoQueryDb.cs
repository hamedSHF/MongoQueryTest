using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest
{
    public class MongoQueryDb
    {
        private const string dataBaseName = "TestDb";
        private const string collectionName = "TestCollection";
        private readonly int sizeOfArray;
        private readonly IMongoDatabase database;
        private IMongoCollection<TestModel> collection;
        private Random random = new Random();
        public MongoQueryDb(int sizeOfArray = 12)
        {
            this.sizeOfArray = sizeOfArray;
            database = MongoConnection.Client.GetDatabase(dataBaseName);
            collection = database.GetCollection<TestModel>(collectionName);
        }
        public async Task CreateAndFillCollection(string collectionName = collectionName,bool capped = true,int size = 100_000)
        {
            
            if(!(await database.ListCollectionNamesAsync()).ToList().Contains(collectionName))
            {
                await database.CreateCollectionAsync(collectionName,new CreateCollectionOptions { Capped = capped,MaxDocuments = size,MaxSize = int.MaxValue });
                collection = database.GetCollection<TestModel>(collectionName);
                await insertDocuments(collection, size);
            }
            else
            {
                collection = database.GetCollection<TestModel>(collectionName);
                if((await collection.CountDocumentsAsync(FilterDefinition<TestModel>.Empty)) == 0)
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
        public async Task ExecuteFirstQuery(int number,List<double> list)
        {
            if(collection != null)
            {
                var query = generateMongoQuery(list);
                var bsonDoc = BsonDocument.Parse(query);
                try
                {
                    var result = collection.Aggregate<Result>(new[] { bsonDoc });
                }
                catch(Exception ex)
                {

                }
            }
        }
        /// <summary>
        /// Return all records which their euclidean distances from target <paramref name="list"/> are less than <paramref name="number"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task ExecuteSecondQuery(int number,List<double> list)
        {

        }
        private string generateMongoQuery(List<double> targetList)
        {
            JObject result = JObject.Parse(File.ReadAllText("FirstQuery.json"));
            var targetArray = (JArray)result["$project"]["distance"]["$let"]["vars"]["pow"]["$reduce"]["input"]["$zip"]["inputs"][0];
            foreach(var item in targetList)
            {
                targetArray.Add(item);
            }
            return result.ToString();
        }
        private async Task insertDocuments(IMongoCollection<TestModel> collection,int size)
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
        private List<TestModel> generateData(int count)
        {
            var finalList = new List<TestModel>(count);
            for(int i = 0;i < count; i++)
            {
                var testModel = new TestModel();
                var list = new List<double>(sizeOfArray);
                for (int j = 0; j < sizeOfArray; j++)
                {
                    list.Add(random.NextDouble());
                }
                testModel.TestArray = list;
                finalList.Add(testModel);
            }
            return finalList;
        }
    }
    public class Result
    {
        [BsonElement("distance")]
        public double Distance { get; set; }
    }
}
