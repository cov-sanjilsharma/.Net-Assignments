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

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [Route("ViewAllCategories")]
        public IActionResult Index()
        {
            try
            {
                ViewBag.PageTitle = "View All Categories";
                var categories = _categoryService.GetAllCategories();
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
        [Route("AddCategory")]
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.CreateCategory(category);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Something went wrong while saving the category❌";
                    return View(category);
                }
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [Route("UpdateCategory/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var category = _categoryService.GetCategoryById(id);
                if (category == null) return NotFound();
                return View(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading category for edit. CategoryId={CategoryId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("UpdateCategory")]
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(category);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Something went wrong while updating the category❌";
                    return View(category);
                }
            }
            return View(category);
        }
    }
}