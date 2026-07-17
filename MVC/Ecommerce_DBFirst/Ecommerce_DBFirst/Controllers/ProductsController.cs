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
        public async Task<IActionResult> Index(int? categoryId, string sortBy, string search)
        {
            try
            {
                ViewBag.PageTitle = "Displaying All Products";
                ViewBag.Categories = await _productService.GetAllCategories();

                var productDtos = await _productService.GetAllProducts(categoryId, sortBy, search);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_ProductsTable", productDtos);

                return View(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while loading product list.");
                throw;
            }
        }

        // View product details
        [Route("Details/{id}")]
        public async Task<IActionResult> ViewDetails(int id)
        {
            try
            {
                var productDto = await _productService.GetProductById(id);

                if (productDto == null)
                    return NotFound();

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
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _productService.GetAllCategories();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> Create(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProduct(productDto);

                    TempData["SuccessMessage"] = "Product Added Successfully✅";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while creating product.");

                    ViewBag.ErrorMessage = "Something went wrong while saving the product❌";
                    ViewBag.Categories = await _productService.GetAllCategories();

                    return View(productDto);
                }
            }

            ViewBag.Categories = await _productService.GetAllCategories();
            ViewBag.ErrorMessage = "Failed to add product. Please check the form❌";

            return View(productDto);
        }

        // Edit product
        [Authorize(Roles = "Admin")]
        [Route("UpdateProduct/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var productDto = await _productService.GetProductById(id);

                if (productDto == null)
                    return NotFound();

                ViewBag.Categories = await _productService.GetAllCategories();

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
        public async Task<IActionResult> Edit(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProduct(productDto);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while updating product. ProductId={ProductId}", productDto.ProductId);

                    ViewBag.ErrorMessage = "Something went wrong while updating the product❌";
                    ViewBag.Categories = await _productService.GetAllCategories();

                    return View(productDto);
                }
            }

            ViewBag.Categories = await _productService.GetAllCategories();

            return View(productDto);
        }

        // Delete product
        [Authorize(Roles = "Admin")]
        [Route("DeleteProduct/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting product. ProductId={ProductId}", id);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to delete product."
                    });
                }

                TempData["ErrorMessage"] = "Something went wrong while deleting the product❌";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}