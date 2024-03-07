using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTJOB.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SetRole()
        {
            var users = await _userManager.Users.Select(u => u.UserName).ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> SetRole(string userId, string role)
        {
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound("User's not exits");
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return NotFound("Role's not exits");
            }

            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index", "Home");
        }
    }
}
