using AuthManager.Application.Enums;
using AuthManager.Infrastructure.Identity.Models;
using AuthManager.Web.Abstractions;
using AuthManager.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).OrderByDescending(a => a.CreatedOn).ToListAsync();

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
        [HttpGet("[area]/[controller]/create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(new UserViewModel());
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MailAddress address = new MailAddress(user.Email);
                    string userName = address.User;
                    var _user = new ApplicationUser
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailConfirmed = true,
                        IsActive = user.IsActive,
                        UserName = userName,
                        CreatedOn = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModifiedOn = DateTime.Now,
                        LastModifiedBy = User.Identity.Name
                    };
                    await GetRoles();
                    var result = await _userManager.CreateAsync(_user, user.Password);
                    if (result.Succeeded)
                    {
                        if (user.NewRoleNames is null)
                            await _userManager.AddToRoleAsync(_user, Roles.User.ToString());
                        else
                            await _userManager.AddToRolesAsync(_user, user.NewRoleNames);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(_user);
                        _notify.Success($"Account for {user.Email} created.");
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                            ModelState.AddModelError("DuplicateEmail", "This email address is already taken.");
                        else
                            ModelState.AddModelError(error.Code, error.Description);
                    }
                    return View(user);
                }
                await GetRoles();
                return View(user);
            }
            catch
            {
                _notify.Error("Some Error Happend.");
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
        public async Task<IActionResult> Edit(string id, [FromForm] UserEditViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id != user.Id)
                        return NotFound();
                    var _user = await _userManager.FindByIdAsync(user.Id);
                    if (_user == null)
                        return NotFound();

                    MailAddress address = new MailAddress(user.Email);
                    _user.FirstName = user.FirstName;
                    _user.LastName = user.LastName;
                    _user.Email = address.Address;
                    var roles = await _userManager.GetRolesAsync(_user);
                    var newRoles = user.NewRoleNames;
                    await _userManager.RemoveFromRolesAsync(_user, roles);
                    await _userManager.AddToRolesAsync(_user, newRoles);
                    _notify.Success($"Account for {_user.Email} has updated.");

                    return RedirectToAction(nameof(Index));
                }

                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        _notify.Error(error.ErrorMessage);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notify.Error("Something error hapend.");
                return View(user);
            }
        }


        // POST: UsersController/Edit/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassword(string id, [FromForm] string Password, string ConfirmPassword)
        {
            try
            {
                if (Password != ConfirmPassword)
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");

                var _user = await _userManager.FindByIdAsync(id);
                if (_user is null)
                    return NotFound();
                var user = _mapper.Map<UserViewModel>(_user);
                user.RoleNames = await _userManager.GetRolesAsync(_user);
                await GetRoles();
                if (Password != ConfirmPassword)
                    return View("/Areas/Admin/Views/Users/Edit.cshtml", user);

                string token = await _userManager.GeneratePasswordResetTokenAsync(_user);
                var result = await _userManager.ResetPasswordAsync(_user, token, Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                    _notify.Error("Error");
                    return View("/Areas/Admin/Views/Users/Edit.cshtml", user);
                }

                _notify.Success($"Password for {_user.Email} has updated.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notify.Error("Something error hapend.");
                return View("/Areas/Admin/Views/Users/Edit.cshtml");
            }
        }

        // POST: UsersController/Edit/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(string id, [FromForm] bool IsActive)
        {
            try
            {
                var _user = await _userManager.FindByIdAsync(id);
                if (_user is null)
                    return NotFound();
                _user.IsActive = IsActive;
                var result = await _userManager.UpdateAsync(_user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                    _notify.Error("Error");
                    var user = _mapper.Map<UserViewModel>(_user);
                    user.RoleNames = await _userManager.GetRolesAsync(_user);
                    await GetRoles();
                    return View("/Areas/Admin/Views/Users/Edit.cshtml", user);
                }

                _notify.Success($"Status for {_user.Email} has been updated.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notify.Error("Something error hapend.");
                return View("/Areas/Admin/Views/Users/Edit.cshtml");
            }
        }


        // GET: UsersController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: UsersController/Delete/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id is null)
                    return NotFound();

                var _user = await _userManager.FindByIdAsync(id);
                if (_user != null)
                {
                    var result = await _userManager.DeleteAsync(_user);
                    if (result.Succeeded)
                    {
                        _notify.Success("Succesfully deleted user with email " + _user.Email + ".");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                        _notify.Error(error.Description);
                    }
                }
                else
                    _notify.Error("An Error hapend.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                _notify.Error("An Error hapend.");
                return RedirectToAction(nameof(Index));
            }
        }

        // GetRoles
        public async Task GetRoles()
        {
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
        }
    }
}
