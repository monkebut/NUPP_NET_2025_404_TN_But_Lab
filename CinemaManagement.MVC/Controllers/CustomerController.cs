using Microsoft.AspNetCore.Mvc;
using CinemaManagement.Common;
using CinemaManagement.MVC.Models;

namespace CinemaManagement.MVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICrudServiceAsync<Customer> _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICrudServiceAsync<Customer> customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _customerService.ReadAllAsync();
                var viewModels = customers.Select(MapToViewModel).ToList();
                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                TempData["ErrorMessage"] = "Error loading customers. Please try again.";
                return View(new List<CustomerViewModel>());
            }
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var customer = await _customerService.ReadAsync(id.Value);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    TempData["ErrorMessage"] = $"Customer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = MapToViewModel(customer);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
                TempData["ErrorMessage"] = "Error loading customer details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View(new CustomerCreateViewModel());
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = new Customer(viewModel.Name, viewModel.Age, viewModel.Email)
                    {
                        Id = Guid.NewGuid()
                    };

                    var success = await _customerService.CreateAsync(customer);

                    if (success)
                    {
                        _logger.LogInformation("Customer created with ID {CustomerId}", customer.Id);
                        TempData["SuccessMessage"] = "Customer created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create customer");
                        ModelState.AddModelError("", "Failed to create customer. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating customer");
                    ModelState.AddModelError("", "An error occurred while creating the customer.");
                }
            }

            return View(viewModel);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var customer = await _customerService.ReadAsync(id.Value);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for edit", id);
                    TempData["ErrorMessage"] = $"Customer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new CustomerEditViewModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    Age = customer.Age,
                    Email = customer.Email
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer for edit with ID {CustomerId}", id);
                TempData["ErrorMessage"] = "Error loading customer for edit.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = new Customer(viewModel.Name, viewModel.Age, viewModel.Email)
                    {
                        Id = viewModel.Id
                    };

                    var success = await _customerService.UpdateAsync(customer);

                    if (success)
                    {
                        _logger.LogInformation("Customer with ID {CustomerId} updated successfully", id);
                        TempData["SuccessMessage"] = "Customer updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _logger.LogWarning("Failed to update customer with ID {CustomerId}", id);
                        ModelState.AddModelError("", "Failed to update customer. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating customer with ID {CustomerId}", id);
                    ModelState.AddModelError("", "An error occurred while updating the customer.");
                }
            }

            return View(viewModel);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var customer = await _customerService.ReadAsync(id.Value);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for delete", id);
                    TempData["ErrorMessage"] = $"Customer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = MapToViewModel(customer);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer for delete with ID {CustomerId}", id);
                TempData["ErrorMessage"] = "Error loading customer for delete.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var customer = await _customerService.ReadAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
                    TempData["ErrorMessage"] = $"Customer with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                var success = await _customerService.RemoveAsync(customer);

                if (success)
                {
                    _logger.LogInformation("Customer with ID {CustomerId} deleted successfully", id);
                    TempData["SuccessMessage"] = "Customer deleted successfully!";
                }
                else
                {
                    _logger.LogWarning("Failed to delete customer with ID {CustomerId}", id);
                    TempData["ErrorMessage"] = "Failed to delete customer. Please try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the customer.";
            }

            return RedirectToAction(nameof(Index));
        }

        #region Mapping Methods

        private CustomerViewModel MapToViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Age = customer.Age,
                Email = customer.Email
            };
        }

        #endregion
    }
}
