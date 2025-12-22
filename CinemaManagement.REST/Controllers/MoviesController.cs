using Microsoft.AspNetCore.Mvc;
using CinemaManagement.Common;
using CinemaManagement.REST.Models;

namespace CinemaManagement.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ICrudServiceAsync<Movie> _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ICrudServiceAsync<Movie> movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService ?? throw new ArgumentNullException(nameof(movieService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all movies or paginated movies
        /// </summary>
        /// <param name="page">Page number (optional)</param>
        /// <param name="amount">Items per page (optional)</param>
        /// <returns>List of movies</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MovieModel>>> GetMovies([FromQuery] int? page, [FromQuery] int? amount)
        {
            try
            {
                IEnumerable<Movie> movies;

                if (page.HasValue && amount.HasValue)
                {
                    _logger.LogInformation("Getting movies with pagination: page={Page}, amount={Amount}", page.Value, amount.Value);
                    movies = await _movieService.ReadAllAsync(page.Value, amount.Value);
                }
                else
                {
                    _logger.LogInformation("Getting all movies");
                    movies = await _movieService.ReadAllAsync();
                }

                var movieModels = movies.Select(MapToModel);
                return Ok(movieModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movies");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a specific movie by ID
        /// </summary>
        /// <param name="id">Movie ID</param>
        /// <returns>Movie details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieModel>> GetMovie(Guid id)
        {
            try
            {
                var movie = await _movieService.ReadAsync(id);

                if (movie == null)
                {
                    _logger.LogWarning("Movie with ID {MovieId} not found", id);
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                return Ok(MapToModel(movie));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movie with ID {MovieId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new movie
        /// </summary>
        /// <param name="createModel">Movie data</param>
        /// <returns>Created movie</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieModel>> CreateMovie([FromBody] MovieCreateModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var movie = MapToEntity(createModel);
                var success = await _movieService.CreateAsync(movie);

                if (!success)
                {
                    _logger.LogWarning("Failed to create movie");
                    return BadRequest(new { message = "Failed to create movie" });
                }

                _logger.LogInformation("Movie created with ID {MovieId}", movie.Id);
                var movieModel = MapToModel(movie);
                return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movieModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing movie
        /// </summary>
        /// <param name="id">Movie ID</param>
        /// <param name="updateModel">Updated movie data</param>
        /// <returns>No content on success</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMovie(Guid id, [FromBody] MovieUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if movie exists
                var existingMovie = await _movieService.ReadAsync(id);
                if (existingMovie == null)
                {
                    _logger.LogWarning("Movie with ID {MovieId} not found for update", id);
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                var movie = MapToEntity(updateModel, id);
                var success = await _movieService.UpdateAsync(movie);

                if (!success)
                {
                    _logger.LogWarning("Failed to update movie with ID {MovieId}", id);
                    return BadRequest(new { message = "Failed to update movie" });
                }

                _logger.LogInformation("Movie with ID {MovieId} updated successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with ID {MovieId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a movie
        /// </summary>
        /// <param name="id">Movie ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            try
            {
                var movie = await _movieService.ReadAsync(id);
                if (movie == null)
                {
                    _logger.LogWarning("Movie with ID {MovieId} not found for deletion", id);
                    return NotFound(new { message = $"Movie with ID {id} not found" });
                }

                var success = await _movieService.RemoveAsync(movie);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete movie with ID {MovieId}", id);
                    return BadRequest(new { message = "Failed to delete movie" });
                }

                _logger.LogInformation("Movie with ID {MovieId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie with ID {MovieId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #region Mapping Methods

        private MovieModel MapToModel(Movie entity)
        {
            return new MovieModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Genre = entity.Genre,
                Duration = entity.Duration,
                Director = entity.Director,
                Budget = entity.Budget
            };
        }

        private Movie MapToEntity(MovieCreateModel dto)
        {
            return new Movie(dto.Title, dto.Genre, dto.Duration, dto.Director, dto.Budget)
            {
                Id = Guid.NewGuid()
            };
        }

        private Movie MapToEntity(MovieUpdateModel dto, Guid id)
        {
            return new Movie(dto.Title, dto.Genre, dto.Duration, dto.Director, dto.Budget)
            {
                Id = id
            };
        }

        #endregion
    }
}

