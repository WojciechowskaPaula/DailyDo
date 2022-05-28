using DailyDo.Data;
using DailyDo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DailyDo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categoryList = _dbContext.Categories.ToList();
            return View(categoryList);
        }

        [HttpGet]
        public IActionResult AddNewCategoryForm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
           var categoryToRemove = _dbContext.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
            _dbContext.Remove(categoryToRemove);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
