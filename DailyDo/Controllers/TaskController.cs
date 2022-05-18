using DailyDo.Data;
using Microsoft.AspNetCore.Mvc;

namespace DailyDo.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var allTasks = _dbContext.Tasks.ToList();
            return View(allTasks);
        }

        [HttpGet]
        public IActionResult AddNewForm()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(DailyDo.Models.Task task)
        {
           var taskToAdd = _dbContext.Tasks.Add(task);
            task.ModificationDate = DateTime.Now;
            task.IsDone = false;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
