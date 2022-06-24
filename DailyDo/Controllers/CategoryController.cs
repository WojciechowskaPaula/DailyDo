using DailyDo.Data;
using DailyDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace DailyDo.Controllers
{
    [Authorize(Roles ="Admin,User")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMemoryCache _memoryCache;
        public CategoryController(ApplicationDbContext dbContext, ILogger<CategoryController> logger, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("action=categoryIndex");
                var categoryListFromCache = _memoryCache.Get<List<Category>>("categoryList");
                if(categoryListFromCache == null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    var categoryList = _dbContext.Categories.ToList();
                    _memoryCache.Set("categoryList", categoryList, cacheEntryOptions);
                    categoryListFromCache = categoryList;
                }
                _logger.LogInformation($"action=categoryIndex categoryCount:{categoryListFromCache.Count}");
                return View(categoryListFromCache);
            }
            catch(Exception ex)
            {
                _logger.LogError($"action=categoryIndex msg='{ex.Message}'", ex);
                return RedirectToAction("Error", "Home");
            }
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
            try
            {
                _logger.LogInformation($"action=addNewCategory category='{JsonSerializer.Serialize(category)}'");
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                _logger.LogInformation($"action=addNewCategory msg='A new category has been saved'");
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _logger.LogError($"action=addNewCategory msg='{ex.Message}' category='{JsonSerializer.Serialize(category)}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"action=categoryDelete id={id}");
                var categoryToRemove = _dbContext.Categories.Where(x => x.CategoryId == id).FirstOrDefault();
                _dbContext.Remove(categoryToRemove);
                _dbContext.SaveChanges();
                _logger.LogInformation($"action=categoryDelete msg='Category with id:{id} has been removed'");
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _logger.LogError($"action=categoryDelete msg={ex.Message} id={id}", ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
