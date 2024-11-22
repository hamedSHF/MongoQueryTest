using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoQueryTest.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoQueryTest.SecondStartegy
{
    public class MongoSecondDb
    {
        private const string dataBaseName = "TestDb2";
        private const string collectionName = "TestCollection2";
        private BsonDocument sortPipeLine;
        private readonly int sizeOfArray;
        private readonly IMongoDatabase database;
        private IMongoCollection<TestModel> collection;
        public MongoSecondDb(int sizeOfArray = 12)
        {
            this.sizeOfArray = sizeOfArray;
            database = MongoConnection.Client.GetDatabase(dataBaseName);
            collection = database.GetCollection<TestModel>(collectionName);
            sortPipeLine = new BsonDocument("$sort", new BsonDocument("distance", 1));
        }
        public async Task CreateAndFillCollection(string collectionName = collectionName, bool capped = true, int size = 100_000)
        {

            if (!(await database.ListCollectionNamesAsync()).ToList().Contains(collectionName))
            {
                await database.CreateCollectionAsync(collectionName, new CreateCollectionOptions { Capped = capped, MaxDocuments = size, MaxSize = int.MaxValue });
                collection = database.GetCollection<TestModel>(collectionName);
                await insertDocuments(collection, size);
            }
            else
            {
                collection = database.GetCollection<TestModel>(collectionName);
                if ((await collection.CountDocumentsAsync(FilterDefinition<TestModel>.Empty)) == 0)
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
        public async Task<List<TestModel>> ExecuteFirstQuery(int number)
        {
            return await collection.Find(FilterDefinition<TestModel>.Empty).SortByDescending(x => x.TestArray_Sum).Limit(number).ToListAsync();
        }
        private async Task insertDocuments(IMongoCollection<TestModel> collection, int size)
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
            for (int i = 0; i < count; i++)
            {
                var testModel = new TestModel();
                testModel.TestArray = Utils.MongoUtils.GenerateArray(sizeOfArray);
                testModel.TestArray_Sum = testModel.TestArray.Sum();
                finalList.Add(testModel);
            }
            return finalList;
        }
    }
}
