using AuthManager.Infrastructure.Identity.Models;
using AuthManager.Web.Abstractions;
using AuthManager.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AuthManager.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : BaseController<UsersController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: UsersController
        [HttpGet("[area]/[controller]")]
        public async Task<IActionResult> Index()
        {
            var allUsersWithRoles = new List<UserViewModel>();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();

            foreach (var user in allUsersExceptCurrentUser)
            {
                var users = _mapper.Map<UserViewModel>(user);
                allUsersWithRoles.Add(users);
            }

            foreach (var user in allUsersWithRoles)
            {
                user.RoleNames = await _userManager.GetRolesAsync(await _userManager.Users.FirstAsync(s => s.UserName == user.UserName));
            }

            return View(allUsersWithRoles);
        }

        // GET: UsersController/Details/5
        [HttpGet("[area]/[controller]/{id?}/show")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var userVm = _mapper.Map<UserViewModel>(user);

            userVm.RoleNames = await _userManager.GetRolesAsync(user);

            return View(userVm);
        }

        // GET: UsersController/Create
        [HttpGet("[area]/[controller]/{id?}/create")]
        public ActionResult Create()
        {
            return View(new UserViewModel());
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        [HttpGet("[area]/[controller]/{id?}/[action]")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var userVm = _mapper.Map<UserViewModel>(user);

            userVm.RoleNames = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            return View(userVm);
        }

        // POST: UsersController/Edit/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id != user.Id)
                    {
                        return NotFound();
                    }
                    var _user = await _userManager.FindByIdAsync(user.Id);
                    if (_user == null)
                    {
                        return NotFound();
                    }
                    if (user.Password != null || user.ConfirmPassword != null)
                    {
                        string token = await _userManager.GeneratePasswordResetTokenAsync(_user);
                        var result = await _userManager.ResetPasswordAsync(_user, token, user.Password);
                        if (!result.Succeeded)
                        {
                            _notify.Error("Something error hapend.");
                            return View(user);
                        }
                        _notify.Success($"Password for {_user.Email} has updated.");
                    }
                    else
                    {
                        MailAddress address = new MailAddress(user.Email);
                        _user.FirstName = user.FirstName;
                        _user.LastName = user.LastName;
                        _user.Email = address.Address;
                        var roles = await _userManager.GetRolesAsync(_user);
                        var newRoles = user.NewRoleNames;
                        await _userManager.RemoveFromRolesAsync(_user, roles);
                        await _userManager.AddToRolesAsync(_user, newRoles);
                        _notify.Success($"Account for {_user.Email} has updated.");
                    }
                    return RedirectToAction(nameof(Index));
                }

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _notify.Error(error.ErrorMessage);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notify.Error("Something error hapend.");
                return View(user);
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
