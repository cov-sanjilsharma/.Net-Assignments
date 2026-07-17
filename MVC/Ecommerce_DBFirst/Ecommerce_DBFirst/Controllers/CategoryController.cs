using Ecommerce_DBFirst.Models;
using Ecommerce_DBFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Route("ViewAllCategories")]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.PageTitle = "View All Categories";

                var categories = await _categoryService.GetAllCategories();

                return View(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading category list.");
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("AddCategory")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            try
            {
                await _categoryService.CreateCategory(category);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating category.");

                ViewBag.ErrorMessage = "Something went wrong while saving the category ❌";

                return View(category);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("UpdateCategory/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);

                if (category == null)
                    return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while loading category for edit. CategoryId={CategoryId}",
                    id);

                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("UpdateCategory")]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            try
            {
                await _categoryService.UpdateCategory(category);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while updating category. CategoryId={CategoryId}",
                    category.CategoryId);

                ViewBag.ErrorMessage = "Something went wrong while updating the category ❌";

                return View(category);
            }
        }
    }
}