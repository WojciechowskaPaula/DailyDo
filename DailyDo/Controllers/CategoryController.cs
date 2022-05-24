using DailyDo.Data;
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
        public IActionResult Index()
        {
            var categoryList = _dbContext.Categories.ToList();
            return View(categoryList);
        }
    }
}
