using DailyDo.Data;
using DailyDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DailyDo.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TaskController> _logger;
        public TaskController(ApplicationDbContext dbContext, ILogger<TaskController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("action=index");
            var allTasks = _dbContext.Tasks.ToList();
            var categories = _dbContext.Categories.ToList();
            _logger.LogInformation($"action=index taskCount={allTasks.Count}, categoriesCount={categories.Count}");
            var listsVM = new TaskAndCategoryListsVM();
            listsVM.Tasks = allTasks;
            listsVM.Categories = categories;
            return View(listsVM);
        }

        [HttpGet]
        public IActionResult AddNewForm()
        {
            _logger.LogInformation("action=addNewForm");
            var categories = _dbContext.Categories.ToList();
            _logger.LogInformation($"action=addNewForm categoriesCount={categories.Count}");
            TaskAndCategoryVM taskAndCategory = new TaskAndCategoryVM();
            taskAndCategory.Categories = categories;
            
            return View(taskAndCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(TaskAndCategoryVM taskAndCategoryVM)
        {

            _logger.LogInformation($"action=addNew task='{JsonSerializer.Serialize(taskAndCategoryVM)}'");
            var taskFromVM = taskAndCategoryVM.Task;
            var categoryFromVM = _dbContext.Categories.Where(x => x.CategoryId == taskAndCategoryVM.Task.Category.CategoryId).FirstOrDefault();
            taskFromVM.Category = categoryFromVM;
            taskFromVM.ModificationDate = DateTime.Now;
            taskFromVM.IsDone = false;
            var taskToAdd = _dbContext.Tasks.Add(taskFromVM);
            _dbContext.SaveChanges();
            _logger.LogInformation("action=addNew msg='the task saved'");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditForm(int id)
        {
            _logger.LogInformation($"action=editForm id={id}");
            var taskToEdit = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
            _logger.LogInformation($"action=editForm msg='the task with id:{id} was found'");
            return View(taskToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(DailyDo.Models.Task task)
        {
            _logger.LogInformation($"action=update task='{JsonSerializer.Serialize(task)}'");
            var updateTask = _dbContext.Tasks.Where(x => x.Id == task.Id);
            task.ModificationDate = DateTime.Now;
            task.IsDone = false;
            _dbContext.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _dbContext.SaveChanges();
            _logger.LogInformation("action=update msg='the task was modified'");
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"action=delete id={id}");
            var taskToDelete = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
            _dbContext.Remove(taskToDelete);
            _dbContext.SaveChanges();
            _logger.LogInformation($"action=delete msg='task with id:{id} has been removed'");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDone([FromBody] DailyDo.Models.Task task)
        {
            _logger.LogInformation($"action=setDone task='{JsonSerializer.Serialize(task)}'");
            var taskToCheck = _dbContext.Tasks.Where(x => x.Id == task.Id).FirstOrDefault();
            if (taskToCheck.IsDone)
            {
                taskToCheck.IsDone = false;
            }
            else
            {
                taskToCheck.IsDone = true;
            }
            _dbContext.SaveChanges();
            _logger.LogInformation($"action=index msg='the task isDone:{task.IsDone} was modified'");
            return Ok(true);
        }

    }
}
