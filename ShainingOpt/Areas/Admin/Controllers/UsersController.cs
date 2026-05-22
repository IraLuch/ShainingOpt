using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.DataBase;
using ShainingOpt.Models;
using ShainingOpt.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShainingOpt.Areas_Admin_Controllers
{

    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Users.Include(u => u.Role);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    RoleId = model.RoleId,
                    PhoneNumber = model.PhoneNumber,
                    
                };
                var res = await _userManager.CreateAsync(user, model.Password);
                if (res.Succeeded)
                {
                    var role = await _context.Roles.FindAsync(model.RoleId);
                    if (role != null)
                    {
                        // 2. Добавляем пользователя в роль через UserManager
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }

                    return RedirectToAction(nameof(Index));

                }
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
            return View(model);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

            var model = new UserCreateViewModel
            {
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                RoleId = user.RoleId.Value
            };
            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserCreateViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }

            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                try
                {
                    if (user.RoleId != model.RoleId)
                    {
                        // Удаляем старые роли из системы Identity
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);

                        // Добавляем новую роль
                        var newRole = await _context.Roles.FindAsync(model.RoleId);
                        if (newRole != null)
                        {
                            await _userManager.AddToRoleAsync(user, newRole.Name);
                        }
                    }
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.RoleId = model.RoleId;


                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var res = await _userManager.RemovePasswordAsync(user);
                        if (res.Succeeded)
                        {
                            await _userManager.AddPasswordAsync(user, model.Password);
                        }
                    }
                    var resUpdate = await _userManager.UpdateAsync(user);
                    if (!resUpdate.Succeeded)
                    {
                        return View();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            try
            {
            if (user != null)
            {
                _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            }

            }
            catch
            {
                return View("Delete", user);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
