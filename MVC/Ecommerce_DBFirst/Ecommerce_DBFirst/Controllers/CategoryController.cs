using Ecommerce_DBFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_DBFirst.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly EcommerceDbfirstDbContext _dbfirstDbContext;
        public CategoryController(EcommerceDbfirstDbContext dbfirstDbContext)
        {
            _dbfirstDbContext = dbfirstDbContext;
        }

        //List all categories
        //[Route("")]
        [Route("ViewAllCategories")]
        public IActionResult Index()
        {
            ViewBag.PageTitle = "View All Categories";
            var categories = _dbfirstDbContext.Categories.ToList();
            return View(categories);
        }

        //Add Category
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
                _dbfirstDbContext.Categories.Add(category);
                _dbfirstDbContext.SaveChanges();
                return RedirectToAction("Index");
                //return Content("hi");

            }
            return View(category);
            //return Content("hii");
        }




        //Edit category
        [Authorize(Roles = "Admin")]
        [Route("UpdateCategory/{id}")]
        public IActionResult Edit(int id)
        {
            var category = _dbfirstDbContext.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [Route("UpdateCategory")]
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _dbfirstDbContext.Categories.Update(category);
                _dbfirstDbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}
