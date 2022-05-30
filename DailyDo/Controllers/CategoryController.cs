using DailyDo.Data;
using DailyDo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DailyDo.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ApplicationDbContext dbContext, ILogger<CategoryController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("action=index");
            var categoryList = _dbContext.Categories.ToList();
            _logger.LogInformation($"action=index categoryCount:{categoryList.Count}");
            return View(categoryList);
        }

        [HttpGet]
        public IActionResult AddNewCategoryForm()
        {
            _logger.LogInformation("action=addNewCategoryForm");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Category category)
        {
            _logger.LogInformation($"action=addNew category='{JsonSerializer.Serialize(category)}'");
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            _logger.LogInformation($"action=addNew msg='A new category has been saved'");
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"action=delete id={id}");
           var categoryToRemove = _dbContext.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
            _dbContext.Remove(categoryToRemove);
            _dbContext.SaveChanges();
            _logger.LogInformation($"action=delete msg='Category with id:{id} has been removed'");
            return RedirectToAction("Index");
        }
    }
}
