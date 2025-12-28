using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaManagement.Common;
using CinemaManagement.REST.Models;

namespace CinemaManagement.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartoonsController : ControllerBase
    {
        private readonly ICrudServiceAsync<Cartoon> _cartoonService;
        private readonly ILogger<CartoonsController> _logger;

        public CartoonsController(ICrudServiceAsync<Cartoon> cartoonService, ILogger<CartoonsController> logger)
        {
            _cartoonService = cartoonService ?? throw new ArgumentNullException(nameof(cartoonService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all cartoons or paginated cartoons
        /// </summary>
        /// <param name="page">Page number (optional)</param>
        /// <param name="amount">Items per page (optional)</param>
        /// <returns>List of cartoons</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CartoonModel>>> GetCartoons([FromQuery] int? page, [FromQuery] int? amount)
        {
            try
            {
                IEnumerable<Cartoon> cartoons;

                if (page.HasValue && amount.HasValue)
                {
                    _logger.LogInformation("Getting cartoons with pagination: page={Page}, amount={Amount}", page.Value, amount.Value);
                    cartoons = await _cartoonService.ReadAllAsync(page.Value, amount.Value);
                }
                else
                {
                    _logger.LogInformation("Getting all cartoons");
                    cartoons = await _cartoonService.ReadAllAsync();
                }

                var cartoonModels = cartoons.Select(MapToModel);
                return Ok(cartoonModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cartoons");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get a specific cartoon by ID
        /// </summary>
        /// <param name="id">Cartoon ID</param>
        /// <returns>Cartoon details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartoonModel>> GetCartoon(Guid id)
        {
            try
            {
                var cartoon = await _cartoonService.ReadAsync(id);

                if (cartoon == null)
                {
                    _logger.LogWarning("Cartoon with ID {CartoonId} not found", id);
                    return NotFound(new { message = $"Cartoon with ID {id} not found" });
                }

                return Ok(MapToModel(cartoon));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cartoon with ID {CartoonId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new cartoon (Requires authentication - Manager or Admin role)
        /// </summary>
        /// <param name="createModel">Cartoon data</param>
        /// <returns>Created cartoon</returns>
        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartoonModel>> CreateCartoon([FromBody] CartoonCreateModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cartoon = MapToEntity(createModel);
                var success = await _cartoonService.CreateAsync(cartoon);

                if (!success)
                {
                    _logger.LogWarning("Failed to create cartoon");
                    return BadRequest(new { message = "Failed to create cartoon" });
                }

                _logger.LogInformation("Cartoon created with ID {CartoonId}", cartoon.Id);
                var cartoonModel = MapToModel(cartoon);
                return CreatedAtAction(nameof(GetCartoon), new { id = cartoon.Id }, cartoonModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cartoon");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Update an existing cartoon (Requires authentication - Manager or Admin role)
        /// </summary>
        /// <param name="id">Cartoon ID</param>
        /// <param name="updateModel">Updated cartoon data</param>
        /// <returns>No content on success</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCartoon(Guid id, [FromBody] CartoonUpdateModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if cartoon exists
                var existingCartoon = await _cartoonService.ReadAsync(id);
                if (existingCartoon == null)
                {
                    _logger.LogWarning("Cartoon with ID {CartoonId} not found for update", id);
                    return NotFound(new { message = $"Cartoon with ID {id} not found" });
                }

                var cartoon = MapToEntity(updateModel, id);
                var success = await _cartoonService.UpdateAsync(cartoon);

                if (!success)
                {
                    _logger.LogWarning("Failed to update cartoon with ID {CartoonId}", id);
                    return BadRequest(new { message = "Failed to update cartoon" });
                }

                _logger.LogInformation("Cartoon with ID {CartoonId} updated successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cartoon with ID {CartoonId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete a cartoon (Requires authentication - Admin role only)
        /// </summary>
        /// <param name="id">Cartoon ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteCartoon(Guid id)
        {
            try
            {
                var cartoon = await _cartoonService.ReadAsync(id);
                if (cartoon == null)
                {
                    _logger.LogWarning("Cartoon with ID {CartoonId} not found for deletion", id);
                    return NotFound(new { message = $"Cartoon with ID {id} not found" });
                }

                var success = await _cartoonService.RemoveAsync(cartoon);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete cartoon with ID {CartoonId}", id);
                    return BadRequest(new { message = "Failed to delete cartoon" });
                }

                _logger.LogInformation("Cartoon with ID {CartoonId} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cartoon with ID {CartoonId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        #region Mapping Methods

        private CartoonModel MapToModel(Cartoon entity)
        {
            return new CartoonModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Genre = entity.Genre,
                Duration = entity.Duration,
                Studio = entity.Studio,
                Is3D = entity.Is3D
            };
        }

        private Cartoon MapToEntity(CartoonCreateModel dto)
        {
            return new Cartoon(dto.Title, dto.Genre, dto.Duration, dto.Studio, dto.Is3D)
            {
                Id = Guid.NewGuid()
            };
        }

        private Cartoon MapToEntity(CartoonUpdateModel dto, Guid id)
        {
            return new Cartoon(dto.Title, dto.Genre, dto.Duration, dto.Studio, dto.Is3D)
            {
                Id = id
            };
        }

        #endregion
    }
}

