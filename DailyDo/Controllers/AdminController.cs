using DailyDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DailyDo.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddNewRoleForm()
        {
           
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> AddNewRole(ProjectRoleVM projectRole)
        {
            var roleExist = await _roleManager.RoleExistsAsync(projectRole.RoleName);
            if (!roleExist)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(projectRole.RoleName));
            }
            return View();
        }
    }
}