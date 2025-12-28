using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CinemaManagement.MongoDb
{
    /// <summary>
    /// MongoDB context for managing database connections
    /// </summary>
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private static bool _guidSerializerRegistered = false;
        private static readonly object _lock = new object();

        public IMongoDatabase Database => _database;

        public MongoDbContext()
        {
            // Register Guid serializer once
            RegisterGuidSerializer();

            // Read connection string from environment or use default
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION") 
                ?? "mongodb://localhost:27017";

            // Read database name from environment or use default
            var databaseName = Environment.GetEnvironmentVariable("MONGO_DB") 
                ?? "CinemaManagementDb";

            try
            {
                var client = new MongoClient(connectionString);
                _database = client.GetDatabase(databaseName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to connect to MongoDB. Connection: {connectionString}, Database: {databaseName}", 
                    ex);
            }
        }

        private static void RegisterGuidSerializer()
        {
            lock (_lock)
            {
                if (!_guidSerializerRegistered)
                {
                    try
                    {
                        // Register Guid serializer to use standard representation
                        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                        _guidSerializerRegistered = true;
                    }
                    catch
                    {
                        // Serializer might already be registered
                    }
                }
            }
        }
    }
}

