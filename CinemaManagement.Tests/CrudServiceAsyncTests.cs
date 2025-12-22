using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;
using Xunit;

namespace CinemaManagement.Tests
{
    public class CrudServiceAsyncTests : IDisposable
    {
        private readonly string _testFilePath = "test_data.json";

        // Очистка после каждого теста
        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddElement()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = Movie.CreateNew();

            // Act
            var result = await service.CreateAsync(movie);

            // Assert
            Assert.True(result);
            var allMovies = await service.ReadAllAsync();
            Assert.Single(allMovies);
            Assert.Contains(movie, allMovies);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnElement()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = Movie.CreateNew();
            await service.CreateAsync(movie);

            // Act
            var result = await service.ReadAsync(movie.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(movie.Id, result.Id);
            Assert.Equal(movie.Title, result.Title);
        }

        [Fact]
        public async Task ReadAsync_NonExistentId_ShouldReturnNull()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);

            // Act
            var result = await service.ReadAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllElements()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie1 = Movie.CreateNew();
            var movie2 = Movie.CreateNew();
            var movie3 = Movie.CreateNew();

            await service.CreateAsync(movie1);
            await service.CreateAsync(movie2);
            await service.CreateAsync(movie3);

            // Act
            var result = await service.ReadAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(movie1, result);
            Assert.Contains(movie2, result);
            Assert.Contains(movie3, result);
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnPagedResults()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            
            for (int i = 0; i < 10; i++)
            {
                await service.CreateAsync(Movie.CreateNew());
            }

            // Act
            var page1 = await service.ReadAllAsync(1, 3);
            var page2 = await service.ReadAllAsync(2, 3);
            var page3 = await service.ReadAllAsync(3, 3);

            // Assert
            Assert.Equal(3, page1.Count());
            Assert.Equal(3, page2.Count());
            Assert.Equal(3, page3.Count());
            
            // Убедимся, что элементы на разных страницах
            Assert.Empty(page1.Intersect(page2));
            Assert.Empty(page2.Intersect(page3));
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_InvalidParameters_ShouldReturnEmpty()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            await service.CreateAsync(Movie.CreateNew());

            // Act
            var result1 = await service.ReadAllAsync(0, 5);
            var result2 = await service.ReadAllAsync(1, 0);
            var result3 = await service.ReadAllAsync(-1, -1);

            // Assert
            Assert.Empty(result1);
            Assert.Empty(result2);
            Assert.Empty(result3);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyElement()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = new Movie("Original Title", "Drama", 120, "Director A", 1000000);
            await service.CreateAsync(movie);

            // Act
            var updatedMovie = new Movie("Updated Title", "Comedy", 90, "Director B", 2000000)
            {
                Id = movie.Id
            };
            var result = await service.UpdateAsync(updatedMovie);

            // Assert
            Assert.True(result);
            var retrieved = await service.ReadAsync(movie.Id);
            Assert.Equal("Updated Title", retrieved.Title);
            Assert.Equal("Comedy", retrieved.Genre);
            Assert.Equal(90, retrieved.Duration);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentElement_ShouldReturnFalse()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = Movie.CreateNew();

            // Act
            var result = await service.UpdateAsync(movie);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteElement()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = Movie.CreateNew();
            await service.CreateAsync(movie);

            // Act
            var result = await service.RemoveAsync(movie);

            // Assert
            Assert.True(result);
            var allMovies = await service.ReadAllAsync();
            Assert.Empty(allMovies);
        }

        [Fact]
        public async Task RemoveAsync_NonExistentElement_ShouldReturnFalse()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie = Movie.CreateNew();

            // Act
            var result = await service.RemoveAsync(movie);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveAsync_ShouldSerializeToFile()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie1 = new Movie("TestMovie1", "Action", 120, "Director1", 1000000);
            var movie2 = new Movie("TestMovie2", "Drama", 90, "Director2", 2000000);
            
            await service.CreateAsync(movie1);
            await service.CreateAsync(movie2);

            // Act
            var result = await service.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(_testFilePath));
            
            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.NotEmpty(fileContent);
            Assert.Contains("TestMovie1", fileContent);
            Assert.Contains("TestMovie2", fileContent);

            // Cleanup
            File.Delete(_testFilePath);
        }

        [Fact]
        public async Task ThreadSafety_ParallelOperations_ShouldNotCorruptData()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            const int operationsCount = 100;

            // Act - Параллельно добавляем элементы
            var tasks = Enumerable.Range(0, operationsCount)
                .Select(_ => service.CreateAsync(Movie.CreateNew()))
                .ToArray();

            await Task.WhenAll(tasks);

            // Assert
            var allMovies = await service.ReadAllAsync();
            var count = allMovies.Count();
            
            // Должны получить все 100 элементов без потерь
            Assert.Equal(operationsCount, count);
        }

        [Fact]
        public async Task ThreadSafety_ParallelSaveOperations_ShouldSucceed()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            
            for (int i = 0; i < 10; i++)
            {
                await service.CreateAsync(Movie.CreateNew());
            }

            // Act - Параллельно пытаемся сохранить файл
            var saveTasks = Enumerable.Range(0, 5)
                .Select(_ => service.SaveAsync())
                .ToArray();

            var results = await Task.WhenAll(saveTasks);

            // Assert - Все операции должны завершиться успешно
            Assert.All(results, result => Assert.True(result));
            Assert.True(File.Exists(_testFilePath));

            // Cleanup
            File.Delete(_testFilePath);
        }

        [Fact]
        public async Task IEnumerable_ShouldAllowLinqQueries()
        {
            // Arrange
            var service = new InMemoryCrudServiceAsync<Movie>(_testFilePath);
            var movie1 = new Movie("Movie A", "Action", 120, "Director 1", 1000000);
            var movie2 = new Movie("Movie B", "Drama", 90, "Director 2", 2000000);
            var movie3 = new Movie("Movie C", "Comedy", 110, "Director 3", 1500000);

            await service.CreateAsync(movie1);
            await service.CreateAsync(movie2);
            await service.CreateAsync(movie3);

            // Act - Используем LINQ напрямую на сервисе
            var actionMovies = service.Where(m => m.Genre == "Action").ToList();
            var shortMovies = service.Where(m => m.Duration < 100).ToList();
            var avgBudget = service.Average(m => m.Budget);

            // Assert
            Assert.Single(actionMovies);
            Assert.Equal("Movie A", actionMovies[0].Title);
            
            Assert.Single(shortMovies);
            Assert.Equal("Movie B", shortMovies[0].Title);
            
            Assert.Equal(1500000, avgBudget);
        }
    }
}

