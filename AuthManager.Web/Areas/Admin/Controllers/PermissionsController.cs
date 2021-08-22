using AuthManager.Application.Constants;
using AuthManager.Infrastructure.DbContexts;
using AuthManager.Infrastructure.Identity.Models;
using AuthManager.Infrastructure.Shared.Services;
using AuthManager.Web.Abstractions;
using AuthManager.Web.Areas.Admin.Models;
using AuthManager.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthManager.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PermissionsController : BaseController<PermissionsController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _context;

        public PermissionsController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IdentityContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }


        // GET: PermissionsController
        [HttpGet("[area]/[controller]")]
        public async Task<IActionResult> Index()
        {
            var role = await _roleManager.FindByNameAsync("SuperAdmin");
            //var userId = User.FindFirstValue(ClaimTypes.Name);
            //var user = await _userManager.FindByIdAsync(userId);
            //var role = await _userManager.GetRolesAsync(user);
            //var allPermissions = new List<RoleClaimsViewModel>();
            //var permissionsViewModel = new List<string>();
            //allPermissions.GetPermissions(typeof(Permissions.Dashboard), role.Id);
            //allPermissions.GetPermissions(typeof(Permissions.Users), role.Id);
            //allPermissions.GetPermissions(typeof(Permissions.Roles), role.Id);
            //allPermissions.GetPermissions(typeof(Permissions.Permission), role.Id);
            //model.RoleId = roleId;
            var claims = await _roleManager.GetClaimsAsync(role);
            //var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claims.Select(a => a.Value).ToList();
            //var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            //foreach (var permission in allPermissions)
            //{
            //    if (authorizedClaims.Any(a => a == permission.Value))
            //    {
            //        permissionsViewModel.Add(permission.Value);
            //    }
            //}
            return View(roleClaimValues);
        }

        // GET: PermissionsController/Details/5
        [HttpGet("[area]/[controller]/{id?}/show")]
        public ActionResult Details(string id)
        {
            var permission = _context.RoleClaims.Where(rc => rc.ClaimValue == id).Select(rc => new RoleClaimsViewModel { Type = rc.ClaimType, Value = rc.ClaimValue }).FirstOrDefault();
            return View(permission);
        }

        // GET: PermissionsController/Create
        [HttpGet("[area]/[controller]/[action]")]
        public ActionResult Create()
        {
            ViewBag.Modules = Permissions.Modules();
            return View(new PermissionCreateViewModel());
        }

        // POST: PermissionsController/Create
        [HttpPost("[area]/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PermissionCreateViewModel permission)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(permission);
                var module = TextService.UpperCase(permission.Module);
                var value = TextService.UpperCase(permission.Value);
                var claimVaule = $"Permissions.{module}.{value}";
                var role = await _roleManager.FindByNameAsync("SuperAdmin");
                await _roleManager.AddPermissionClaim(role, claimVaule);
                _notify.Success($"Succesfully created a permission with name: {value}");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PermissionsController/Edit/5
        [HttpGet("[area]/[controller]/{id?}/[action]")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id is null)
                return NotFound();
            var permission = await _context.RoleClaims.Where(rc => rc.ClaimValue == id).FirstOrDefaultAsync();
            string[] claimValue = permission.ClaimValue.Split('.');
            var permissionViewModel = new PermissionEditViewModel
            {
                Type = claimValue[0],
                Module = claimValue[1],
                Value = claimValue[2]
            };
            ViewBag.Modules = Permissions.Modules();
            return View(permissionViewModel);
        }

        // POST: PermissionsController/Edit/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] PermissionEditViewModel permission)
        {
            try
            {
                if (id is null)
                    return NotFound();
                if (!ModelState.IsValid)
                    return NotFound();
                var module = TextService.UpperCase(permission.Module);
                var value = TextService.UpperCase(permission.Value);
                var claimVaule = $"Permissions.{module}.{value}";
                var _permission = await _context.RoleClaims.Where(rc => rc.ClaimValue == id).FirstOrDefaultAsync();
                _permission.ClaimValue = claimVaule;
                var result = await _context.SaveChangesAsync();
                if (result == 1)
                    _notify.Success($"Succesfully updated permission with new name: {value}");
                else
                    _notify.Error("Some Error happend");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PermissionsController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: PermissionsController/Delete/5
        [HttpPost("[area]/[controller]/{id?}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id is null)
                    return NotFound();
                var _permission = await _context.RoleClaims.Where(rc => rc.ClaimValue == id).FirstOrDefaultAsync();
                _context.RoleClaims.Remove(_permission);
                var result = await _context.SaveChangesAsync();
                if (result == 1)
                    _notify.Success($"Succesfully deleted permission with name: {_permission.ClaimValue}");
                else
                    _notify.Error("Some Error happend");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
