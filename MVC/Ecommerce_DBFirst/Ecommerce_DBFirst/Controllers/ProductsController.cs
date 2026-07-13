using AutoMapper;
using Ecommerce_DBFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_DBFirst.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly EcommerceDbfirstDbContext _dbfirstDbContext;
        private readonly IMapper _mapper;
        public ProductsController(EcommerceDbfirstDbContext dbfirstDbContext, IMapper mapper)
        {
            _dbfirstDbContext = dbfirstDbContext;
            _mapper = mapper;
        }

        // List all products
        [Route("")]
        [Route("ViewAllProducts")]
        public IActionResult Index(int? categoryId, string sortBy, string search)
        {
            ViewBag.PageTitle = "Displaying All Products";
            var categories = _dbfirstDbContext.Categories.ToList();
            ViewBag.Categories = categories;

            var products = _dbfirstDbContext.Products.AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(product => product.CategoryId == categoryId);

            if (sortBy == "price")
                products = products.OrderBy(product => product.Price);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(product => product.ProductName.Contains(search));

            var productDtos = _mapper.Map<List<ProductDTO>>(products.ToList());

            foreach (var dto in productDtos)
            {
                dto.CategoryName = categories.FirstOrDefault(c => c.CategoryId == dto.CategoryId)?.CategoryName;
            }

            // AJAX call → return just the table HTML (partial view)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductsTable", productDtos);
            }

            // Normal page load → return the full page
            return View(productDtos);
        }



        //View product details

        [Route("Details/{id}")]
        //[Route("Products/ViewDetails/{id}")]

        public IActionResult ViewDetails(int id)
        {
            Products product = _dbfirstDbContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            var category = _dbfirstDbContext.Categories.Find(productDto.CategoryId);
            productDto.CategoryName = category?.CategoryName;

            ViewData["PageTitle"] = "Product Details of - " + product.ProductName;
            return View(productDto);
        }


        // Add new product

        //[Route("Products/Create")]
        [Authorize(Roles = "Admin")]
        [Route("AddProduct")]
        public IActionResult Create()
        {
            ViewBag.Categories = _dbfirstDbContext.Categories.ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("AddProduct")]
        public IActionResult Create(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Products>(productDto);
                _dbfirstDbContext.Products.Add(product);
                _dbfirstDbContext.SaveChanges();

                TempData["SuccessMessage"] = "Product Added Successfully✅";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _dbfirstDbContext.Categories.ToList();
            ViewBag.ErrorMessage = "Failed to add product. Please check the form❌";
            return View(productDto);
        }

        // Edit product

        [Authorize(Roles = "Admin")]
        [Route("UpdateProduct/{id}")]
        //[Route("Products/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var product = _dbfirstDbContext.Products.Find(id);
            if (product == null) return NotFound();
            ViewBag.Categories = _dbfirstDbContext.Categories.ToList();
            var productDto = _mapper.Map<ProductDTO>(product);
            return View(productDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("UpdateProduct")]
        //[Route("Products/Edit")]
        public IActionResult Edit(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                var product = _mapper.Map<Products>(productDto);
                _dbfirstDbContext.Products.Update(product);
                _dbfirstDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productDto);
        }


        // Delete product


        //[Route("Products/Delete/{id}")]
        //[Route("DeleteProduct/{id}")]
        //public IActionResult Delete(int id)
        //{
        //    var product = _dbfirstDbContext.Products.Find(id);
        //    if (product == null) return NotFound();
        //    _dbfirstDbContext.Products.Remove(product);
        //    _dbfirstDbContext.SaveChanges();

        //    return RedirectToAction("Index");
        //}


        [Authorize(Roles = "Admin")]
        [Route("DeleteProduct/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _dbfirstDbContext.Products.Find(id);
            if (product == null) return NotFound();

            _dbfirstDbContext.Products.Remove(product);
            _dbfirstDbContext.SaveChanges();

            // If the request came from jQuery/AJAX, return JSON instead of redirecting
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Index");
        }
    }
}
