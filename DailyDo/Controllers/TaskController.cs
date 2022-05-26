using DailyDo.Data;
using DailyDo.Models;
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
            var categories = _dbContext.Categories.ToList();
            var listsVM = new TaskAndCategoryListsVM();
            listsVM.Tasks = allTasks;
            listsVM.Categories = categories;
            return View(listsVM);
        }

        [HttpGet]
        public IActionResult AddNewForm()
        {
            var categories = _dbContext.Categories.ToList();
            TaskAndCategoryVM taskAndCategory = new TaskAndCategoryVM();
            taskAndCategory.Categories = categories;
            return View(taskAndCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(TaskAndCategoryVM taskAndCategoryVM)
        {
            var taskFromVM = taskAndCategoryVM.Task; 
           var categoryFromVM= _dbContext.Categories.Where(x => x.CategoryId == taskAndCategoryVM.Task.Category.CategoryId).FirstOrDefault();
            taskFromVM.Category = categoryFromVM;
            taskFromVM.ModificationDate = DateTime.Now;
            taskFromVM.IsDone = false;
            var taskToAdd = _dbContext.Tasks.Add(taskFromVM);
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

       
        public IActionResult Delete (int id)
        {
            var taskToDelete = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
            _dbContext.Remove(taskToDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SetDone([FromBody] DailyDo.Models.Task task)
        {
            var taskToCheck = _dbContext.Tasks.Where(x => x.Id == task.Id).FirstOrDefault();
            if(taskToCheck.IsDone)
            {
                taskToCheck.IsDone = false;
            }
            else
            {
                taskToCheck.IsDone = true;
            }
            
            _dbContext.SaveChanges();
            return Ok(true);
        }
    }
}
