using System;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;
using CinemaManagement.MongoDb;
using MongoDB.Driver;
using Xunit;

namespace CinemaManagement.Tests
{
    public class MongoDbTests : IAsyncLifetime
    {
        private MongoDbContext? _context;
        private IRepository<Movie>? _repository;
        private MongoCrudServiceAsync<Movie>? _service;
        private bool _isMongoAvailable = false;

        public async Task InitializeAsync()
        {
            try
            {
                // Try to connect to MongoDB
                _context = new MongoDbContext();
                _repository = new MongoRepository<Movie>(_context.Database);
                _service = new MongoCrudServiceAsync<Movie>(_repository);
                
                // Test connection by trying to list collections
                var cursor = await _context.Database.ListCollectionNamesAsync();
                await cursor.ToListAsync();
                
                _isMongoAvailable = true;
            }
            catch
            {
                _isMongoAvailable = false;
            }
        }

        public async Task DisposeAsync()
        {
            if (_isMongoAvailable && _context != null)
            {
                try
                {
                    // Clean up test data
                    await _context.Database.DropCollectionAsync("Movie");
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Fact]
        public async Task MongoRepository_CreateAsync_ShouldAddElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = Movie.CreateNew();

            // Act
            await _repository!.AddAsync(movie);
            var retrieved = await _repository.GetByIdAsync(movie.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(movie.Id, retrieved.Id);
            Assert.Equal(movie.Title, retrieved.Title);
        }

        [Fact]
        public async Task MongoRepository_GetByIdAsync_NonExistent_ShouldReturnNull()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Act
            var result = await _repository!.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MongoRepository_GetAllAsync_ShouldReturnAllElements()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie1 = Movie.CreateNew();
            var movie2 = Movie.CreateNew();
            var movie3 = Movie.CreateNew();

            await _repository!.AddAsync(movie1);
            await _repository.AddAsync(movie2);
            await _repository.AddAsync(movie3);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.True(result.Count >= 3);
            Assert.Contains(result, m => m.Id == movie1.Id);
            Assert.Contains(result, m => m.Id == movie2.Id);
            Assert.Contains(result, m => m.Id == movie3.Id);
        }

        [Fact]
        public async Task MongoRepository_UpdateAsync_ShouldModifyElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = new Movie("Original Title", "Drama", 120, "Director A", 1000000);
            await _repository!.AddAsync(movie);

            // Act
            movie.Title = "Updated Title";
            movie.Budget = 2000000;
            await _repository.UpdateAsync(movie);

            var updated = await _repository.GetByIdAsync(movie.Id);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
            Assert.Equal(2000000, updated.Budget);
        }

        [Fact]
        public async Task MongoRepository_DeleteAsync_ShouldRemoveElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = Movie.CreateNew();
            await _repository!.AddAsync(movie);

            // Act
            await _repository.DeleteAsync(movie);
            var deleted = await _repository.GetByIdAsync(movie.Id);

            // Assert
            Assert.Null(deleted);
        }

        [Fact]
        public async Task MongoCrudService_CreateAsync_ShouldAddElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = Movie.CreateNew();

            // Act
            var result = await _service!.CreateAsync(movie);

            // Assert
            Assert.True(result);
            var retrieved = await _service.ReadAsync(movie.Id);
            Assert.NotNull(retrieved);
            Assert.Equal(movie.Id, retrieved.Id);
        }

        [Fact]
        public async Task MongoCrudService_ReadAllAsync_ShouldReturnAllElements()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie1 = Movie.CreateNew();
            var movie2 = Movie.CreateNew();
            await _service!.CreateAsync(movie1);
            await _service.CreateAsync(movie2);

            // Act
            var result = await _service.ReadAllAsync();
            var resultList = result.ToList();

            // Assert
            Assert.True(resultList.Count >= 2);
            Assert.Contains(resultList, m => m.Id == movie1.Id);
            Assert.Contains(resultList, m => m.Id == movie2.Id);
        }

        [Fact]
        public async Task MongoCrudService_ReadAllAsync_WithPagination_ShouldReturnPagedResults()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange - Create at least 20 movies for testing
            for (int i = 0; i < 20; i++)
            {
                await _service!.CreateAsync(Movie.CreateNew());
            }

            // Act
            var page1 = await _service!.ReadAllAsync(1, 5);
            var page2 = await _service.ReadAllAsync(2, 5);

            // Assert
            Assert.Equal(5, page1.Count());
            Assert.Equal(5, page2.Count());
            
            // Pages should have different movies
            var page1Ids = page1.Select(m => m.Id).ToList();
            var page2Ids = page2.Select(m => m.Id).ToList();
            Assert.Empty(page1Ids.Intersect(page2Ids));
        }

        [Fact]
        public async Task MongoCrudService_ReadAllAsync_WithPagination_InvalidParameters_ShouldReturnEmpty()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            await _service!.CreateAsync(Movie.CreateNew());

            // Act
            var result1 = await _service.ReadAllAsync(0, 5);
            var result2 = await _service.ReadAllAsync(1, 0);
            var result3 = await _service.ReadAllAsync(-1, -1);

            // Assert
            Assert.Empty(result1);
            Assert.Empty(result2);
            Assert.Empty(result3);
        }

        [Fact]
        public async Task MongoCrudService_UpdateAsync_ShouldModifyElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = new Movie("Original", "Drama", 120, "Director", 1000000);
            await _service!.CreateAsync(movie);

            // Act
            movie.Title = "Updated";
            movie.Budget = 2000000;
            var result = await _service.UpdateAsync(movie);

            // Assert
            Assert.True(result);
            var updated = await _service.ReadAsync(movie.Id);
            Assert.Equal("Updated", updated.Title);
            Assert.Equal(2000000, updated.Budget);
        }

        [Fact]
        public async Task MongoCrudService_RemoveAsync_ShouldDeleteElement()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie = Movie.CreateNew();
            await _service!.CreateAsync(movie);

            // Act
            var result = await _service.RemoveAsync(movie);

            // Assert
            Assert.True(result);
            var deleted = await _service.ReadAsync(movie.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task MongoCrudService_SaveAsync_ShouldReturnTrue()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Act
            var result = await _service!.SaveAsync();

            // Assert - SaveAsync is a no-op for MongoDB but should return true
            Assert.True(result);
        }

        [Fact]
        public async Task MongoCrudService_ParallelOperations_ShouldNotCorruptData()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            const int operationsCount = 50;
            var movies = Enumerable.Range(0, operationsCount)
                .Select(_ => Movie.CreateNew())
                .ToList();

            // Act - Parallel insert
            var tasks = movies.Select(m => _service!.CreateAsync(m)).ToArray();
            await Task.WhenAll(tasks);

            // Assert
            foreach (var movie in movies)
            {
                var retrieved = await _service!.ReadAsync(movie.Id);
                Assert.NotNull(retrieved);
                Assert.Equal(movie.Id, retrieved.Id);
            }
        }

        [Fact]
        public async Task MongoCrudService_IEnumerable_ShouldAllowLinqQueries()
        {
            if (!_isMongoAvailable)
                return; // Skip test if MongoDB is not available

            // Arrange
            var movie1 = new Movie("Action Movie", "Action", 120, "Director 1", 1000000);
            var movie2 = new Movie("Drama Movie", "Drama", 90, "Director 2", 2000000);
            var movie3 = new Movie("Comedy Movie", "Comedy", 110, "Director 3", 1500000);

            await _service!.CreateAsync(movie1);
            await _service.CreateAsync(movie2);
            await _service.CreateAsync(movie3);

            // Wait for cache refresh
            await Task.Delay(500);

            // Act - Use LINQ on service
            var count = _service.Count();
            var hasAction = _service.Any(m => m.Genre == "Action");

            // Assert
            Assert.True(count >= 3);
            Assert.True(hasAction);
        }
    }
}

