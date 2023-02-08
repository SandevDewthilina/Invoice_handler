using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HRMS_WEB.Models;
using HRMS_WEB.Viewmodels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HRMS_WEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHostApplicationLifetime appLifetime;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHostApplicationLifetime appLifetime)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.appLifetime = appLifetime;
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        public void Kill()
        {
            appLifetime.StopApplication();
            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    runCron();
            //}
        }

        private String runCron()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "/root/pub/start.sh";
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;

            Process process = Process.Start(ps);
            process.WaitForExit();
            return "Success";
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }

        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        public async Task<IActionResult> EditRole(String id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in await userManager.Users.ToListAsync())
            {

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return NotFound();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);

            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(String roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return NotFound();
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in await userManager.Users.ToListAsync())
            {
                var userroleviewmodel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userroleviewmodel.IsSelected = true;
                }
                else
                {
                    userroleviewmodel.IsSelected = false;
                }

                model.Add(userroleviewmodel);

            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(String roleId, List<UserRoleViewModel> model)
        {

            var role = await roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                return NotFound();
            }

            for(int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult results = null;

                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    results = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    results = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if(results.Succeeded)
                {
                    if(i < model.Count - 1)
                    {
                        continue;
                    } 
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId});
                    }
                }

            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }
}

