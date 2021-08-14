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

namespace AuthManager.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : BaseController<RolesController>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IdentityContext _context;

        public RolesController(RoleManager<IdentityRole> roleManager, IdentityContext context)
        {
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
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            var roleVm = _mapper.Map<RoleViewModel>(role);
            var allUserRoles = await _context.UserRoles.ToListAsync();
            roleVm.NumberOfUsers = allUserRoles.Count(ur => ur.RoleId == roleVm.Id);

            return View(roleVm);
        }

        // GET: RolesController/Create
        [HttpGet("[area]/[controller]/create")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController/Create
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

        // GET: RolesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RolesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: RolesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RolesController/Delete/5
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
