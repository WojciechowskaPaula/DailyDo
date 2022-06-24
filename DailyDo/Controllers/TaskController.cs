using DailyDo.Data;
using DailyDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace DailyDo.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<TaskController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMemoryCache _memoryCache;
        public TaskController(ApplicationDbContext dbContext, ILogger<TaskController> logger, UserManager<IdentityUser> userManager, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("action=index");
                var signInUser = await _userManager.GetUserAsync(HttpContext.User);
                var allTasks = _dbContext.Tasks.Where(x => x.User == signInUser).ToList();
                var categories = _dbContext.Categories.ToList();
                _logger.LogInformation($"action=index taskCount={allTasks.Count}, categoriesCount={categories.Count}");
                var listsVM = new TaskAndCategoryListsVM();
                listsVM.Tasks = allTasks;
                listsVM.Categories = categories;
                return View(listsVM);
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=index msg='{ex.Message}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult AddNewForm()
        {
            try
            {
                _logger.LogInformation("action=addNewForm");
                var categoryListFromCache =  _memoryCache.Get<List<Category>>("categoryList");
                if(categoryListFromCache == null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var categories = _dbContext.Categories.ToList();
                    _memoryCache.Set("categoryList", categories,cacheEntryOptions);
                    categoryListFromCache = categories;
                }
               
                _logger.LogInformation($"action=addNewForm categoriesCount={categoryListFromCache.Count}");
                TaskAndCategoryVM taskAndCategory = new TaskAndCategoryVM();
                taskAndCategory.Categories = categoryListFromCache;
                return View(taskAndCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=addNewForm msg='{ex.Message}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNew(TaskAndCategoryVM taskAndCategoryVM)
        {
            try
            {
                _logger.LogInformation($"action=addNew task='{JsonSerializer.Serialize(taskAndCategoryVM)}'");
                var taskFromVM = taskAndCategoryVM.Task;
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var categoryFromVM = _dbContext.Categories.Where(x => x.CategoryId == taskAndCategoryVM.Task.Category.CategoryId).FirstOrDefault();
                taskFromVM.Category = categoryFromVM;
                taskFromVM.ModificationDate = DateTime.Now;
                taskFromVM.IsDone = false;
                taskFromVM.User = user;
                var taskToAdd = _dbContext.Tasks.Add(taskFromVM);
                _dbContext.SaveChanges();
                _logger.LogInformation("action=addNew msg='the task saved'");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=addNew msg='{ex.Message}' task='{JsonSerializer.Serialize(taskAndCategoryVM)}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult EditForm(int id)
        {
            try
            {
                _logger.LogInformation($"action=editForm id={id}");
                var taskToEdit = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
                _logger.LogInformation($"action=editForm msg='the task with id:{id} was found'");
                return View(taskToEdit);
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=editForm msg='{ex.Message}' id='{id}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(DailyDo.Models.Task task)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"action=update msg='{ex.Message}' task='{JsonSerializer.Serialize(task)}'", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"action=delete id={id}");
                var taskToDelete = _dbContext.Tasks.Where(x => x.Id == id).FirstOrDefault();
                _dbContext.Remove(taskToDelete);
                _dbContext.SaveChanges();
                _logger.LogInformation($"action=delete msg='task with id:{id} has been removed'");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=delete msg='{ex.Message}' id={id}", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetDone([FromBody] DailyDo.Models.Task task)
        {
            try
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
            catch(Exception ex)
            {
                _logger.LogError($"action=setDone msg='{ex.Message}' task='{JsonSerializer.Serialize(task)}'", ex);
                return RedirectToAction("Error","Home");
            }
        }
    }
}