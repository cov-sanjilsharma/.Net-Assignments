using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // List all products
        [Route("")]
        [Route("ViewAllProducts")]
        public IActionResult Index(int? categoryId, string sortBy, string search)
        {
            try
            {
                //throw new Exception("Test exception for global handler verification");   // TEMPORARY - remove after testing
                ViewBag.PageTitle = "Displaying All Products";
                ViewBag.Categories = _productService.GetAllCategories();

                var productDtos = _productService.GetAllProducts(categoryId, sortBy, search);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ProductsTable", productDtos);

                return View(productDtos);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading product list.");
                throw; // let the global handler take over — don't swallow it silently
            }
        }

        //View product details
        [Route("Details/{id}")]
        public IActionResult ViewDetails(int id)
        {
            try
            {
                var productDto = _productService.GetProductById(id);
                if (productDto == null)
                {
                    return NotFound();
                }

                ViewData["PageTitle"] = "Product Details of - " + productDto.ProductName;
                return View(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading product details. ProductId={ProductId}", id);
                throw;
            }
        }

        // Add new product
        [Authorize(Roles = "Admin")]
        [Route("AddProduct")]
        public IActionResult Create()
        {
            ViewBag.Categories = _productService.GetAllCategories();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProduct")]
        public IActionResult Create(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productService.CreateProduct(productDto);
                    TempData["SuccessMessage"] = "Product Added Successfully✅";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Something went wrong while saving the product❌";
                    ViewBag.Categories = _productService.GetAllCategories();
                    return View(productDto);
                }
            }
            ViewBag.Categories = _productService.GetAllCategories();
            ViewBag.ErrorMessage = "Failed to add product. Please check the form❌";
            return View(productDto);
        }

        // Edit product
        [Authorize(Roles = "Admin")]
        [Route("UpdateProduct/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var productDto = _productService.GetProductById(id);
                if (productDto == null) return NotFound();

                ViewBag.Categories = _productService.GetAllCategories();
                return View(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading product for edit. ProductId={ProductId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("UpdateProduct")]
        public IActionResult Edit(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _productService.UpdateProduct(productDto);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ViewBag.ErrorMessage = "Something went wrong while updating the product❌";
                    return View(productDto);
                }
            }
            return View(productDto);
        }

        // Delete product
        [Authorize(Roles = "Admin")]
        [Route("DeleteProduct/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.DeleteProduct(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction("Index");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Failed to delete product." });
                }

                TempData["ErrorMessage"] = "Something went wrong while deleting the product❌";
                return RedirectToAction("Index");
            }
        }
    }
}