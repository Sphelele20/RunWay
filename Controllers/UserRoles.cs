using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RunWay.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace RunWay.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /UserRoles
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = await _userManager.GetRolesAsync(user)
                };
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }

        // GET: /UserRoles/Manage?userId=xxx
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found!!";
                return View("NotFound");
            }

            ViewBag.UserName = user.UserName;

            // FIX HERE: Was _roleManager.GetRolesAsync.ToList()
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new List<ManageUserRolesViewModel>();

            foreach (var role in roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? ""
                };

                if (!string.IsNullOrEmpty(role.Name))
                {
                    userRolesViewModel.IsSelected = await _userManager.IsInRoleAsync(user, role.Name);
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        // POST: /UserRoles/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles!");
                return View(model);
            }

            var selectedRoles = model.Where(x => x.IsSelected).Select(y => y.RoleName).ToList();

            foreach (var roleName in selectedRoles)
            {
                var addResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", $"Cannot add role {roleName} to user!");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}