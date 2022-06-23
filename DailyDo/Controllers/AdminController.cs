using DailyDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DailyDo.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(RoleManager<IdentityRole> roleManager, ILogger<AdminController> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("action=adminIndex");
            return View();
        }

        [HttpGet]
        public IActionResult AddNewRoleForm()
        {
            _logger.LogInformation("action=AddNewRoleForm");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> AddNewRole(ProjectRoleVM projectRole)
        {
            try
            {
                _logger.LogInformation($"action=AddNewRole projectRole={JsonSerializer.Serialize(projectRole)}");
                var roleExist = await _roleManager.RoleExistsAsync(projectRole.RoleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(projectRole.RoleName));
                }
                _logger.LogInformation($"action=AddNewRole msg='A new role have been saved'");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"action=AddNewRole msg='{ex.Message}' projectRole={JsonSerializer.Serialize(projectRole)}", ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}