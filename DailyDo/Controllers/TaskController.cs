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

        [HttpGet]
        public IActionResult EditForm(int id)
        {
            var taskToEdit = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
            return View(taskToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update (DailyDo.Models.Task task)
        {
            var updateTask = _dbContext.Tasks.Where(x => x.Id == task.Id);
            task.ModificationDate = DateTime.Now;
            task.IsDone = false;
            _dbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete (int id)
        {
            var taskToDelete = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
            _dbContext.Remove(taskToDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
