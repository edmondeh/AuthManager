using AuthManager.Infrastructure.DbContexts;
using AuthManager.Web.Abstractions;
using AuthManager.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthManager.Web.Helpers;
using AuthManager.Infrastructure.Shared.Services;
using AuthManager.Infrastructure.Identity.Models;

namespace AuthManager.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : BaseController<RolesController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _context;

        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IdentityContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: RolesController
        [HttpGet("[area]/[controller]")]
        public async Task<IActionResult> Index()
        {
            var allUserRoles = await _context.UserRoles.ToListAsync();
            var rolesVM = await _roleManager.Roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                //NumberOfUsers = allUserRoles.Count(ur => ur.RoleId == r.Id)
            }).ToListAsync();

            foreach (var role in rolesVM)
                role.NumberOfUsers = allUserRoles.Count(ur => ur.RoleId == role.Id);

            return View(rolesVM);
        }

        // GET: RolesController/Details/5
        [HttpGet("[area]/[controller]/{id?}/show")]
        public async Task<IActionResult> Details(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var roleVm = _mapper.Map<RoleViewModel>(role);
            var allUserRoles = await _context.UserRoles.ToListAsync();
            var users = _userManager.GetUsersInRoleAsync
            roleVm.NumberOfUsers = allUserRoles.Count(ur => ur.RoleId == roleVm.Id);
            ViewBag.Permissions = await GetPermissions(role.Name);
            return View(roleVm);
        }

        // GET: RolesController/Create
        [HttpGet("[area]/[controller]/[action]")]
        public async Task<IActionResult> Create()
        {
            await GetPermissions();
            return View(new RoleViewModel());
        }

        // POST: RolesController/Create
        [HttpPost("[area]/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] RoleViewModel role)
        {
            try
            {
                await GetPermissions();
                if (!ModelState.IsValid)
                    return View(role);

                //var _role = _mapper.Map<IdentityRole>(role);
                var _role = new IdentityRole { Name = TextService.UpperCase(role.Name) };
                var result = await _roleManager.CreateAsync(_role);
                if (result.Succeeded)
                {
                    foreach (var permission in role.Permissions)
                    {
                        await _roleManager.AddPermissionClaim(_role, $"Permissions.{permission}");
                    }
                    _notify.Success($"Role with name: {_role.Name} created.");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return View(role);
            }
            catch
            {
                return View();
            }
        }

        // GET: RolesController/Edit/5
        [HttpGet("[area]/[controller]/{id?}/[action]")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id.ToString() is null)
                return NotFound();
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id.ToString());
            if (role is null)
                return NotFound();
            var roleVm = _mapper.Map<RoleViewModel>(role);
            await GetPermissions();
            roleVm.Permissions = await GetPermissions(role.Name);
            return View(roleVm);
        }

        // POST: RolesController/Edit/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] RoleViewModel role)
        {
            try
            {
                //if (id != role.Id)
                if (id.Equals(role.Id))
                    return NotFound();
                if (!ModelState.IsValid)
                    return NotFound();
                var _role = await _roleManager.FindByIdAsync(id.ToString());
                _role.Name = TextService.UpperCase(role.Name);
                var result = await _roleManager.UpdateAsync(_role);

                if (result.Succeeded)
                {
                    var claims = await _roleManager.GetClaimsAsync(_role);
                    foreach (var claim in claims)
                        await _roleManager.RemoveClaimAsync(_role, claim);
                    foreach (var permission in role.Permissions)
                        await _roleManager.AddPermissionClaim(_role, $"Permissions.{permission}");
                    _notify.Success($"Succesfully updated!");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);

                //PopulateRolesPermissionsDropDownList(role.Permissions);
                return View(role);
            }
            catch
            {
                return View();
            }
        }

        // GET: RolesController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: RolesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                if (id.ToString() is null)
                    return NotFound();

                var _role = await _roleManager.FindByIdAsync(id.ToString());
                if (_role != null)
                {
                    var result = await _roleManager.DeleteAsync(_role);
                    if (result.Succeeded)
                        _notify.Success("Succesfully deleted role with name " + _role.Name + ".");
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
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Get All Permissions
        /// </summary>
        /// <returns></returns>
        private async Task GetPermissions()
        {
            //var permissions = _context.RoleClaims.ToList();
            var role = await _roleManager.FindByNameAsync("SuperAdmin");
            var permissions = await _roleManager.GetClaimsAsync(role);
            var permissionsViewModel = new List<string>();
            foreach (var permission in permissions)
            {
                string[] value = permission.Value.Split('.');
                permissionsViewModel.Add($"{value[1]}.{value[2]}");
            }
            ViewBag.Permissions = permissionsViewModel;
        }

        /// <summary>
        /// Get Permissions For Role
        /// </summary>
        /// <returns>List of string</returns>
        private async Task<List<string>> GetPermissions(string roleName)
        {
            if (roleName is null)
                return new List<string>();
            var permissionsViewModel = new List<string>();
            var role = await _roleManager.FindByNameAsync(roleName);
            var permissions = await _roleManager.GetClaimsAsync(role);
            foreach (var permission in permissions)
            {
                string[] value = permission.Value.Split('.');
                permissionsViewModel.Add($"{value[1]}.{value[2]}");
            }
            return permissionsViewModel;
        }
    }
}
