using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Spicy_demo.Data;
using Spicy_demo.Models.View_Models;
using Spicy_demo.Utility;

namespace Spicy_demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public MenuItemViewModel MenuItemViewModel { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            MenuItemViewModel = new MenuItemViewModel
            {
                Categories = _db.Category,
                SubCategories = _db.SubCategory,
                MenuItem = new Models.MenuItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).ToListAsync();
            return View(menuItems);
        }

        public async Task<IActionResult> Create()
        {
            return View(MenuItemViewModel);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuItemViewModel);
            }
            _db.MenuItem.Add(MenuItemViewModel.MenuItem);
            await _db.SaveChangesAsync();

            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemViewModel.MenuItem.Id);

            if (files.Count() > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extensions = Path.GetExtension(files[0].FileName);
                using (var filename = new FileStream(Path.Combine(uploads, MenuItemViewModel.MenuItem.Id + extensions), FileMode.Create))
                {
                    files[0].CopyTo(filename);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemViewModel.MenuItem.Id + extensions;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemViewModel.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemViewModel.MenuItem.Id + ".png";
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(MenuItemViewModel);
            }


            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemViewModel.MenuItem.Id);

            if (files.Count() > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extensions = Path.GetExtension(files[0].FileName);
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using (var filename = new FileStream(Path.Combine(uploads, MenuItemViewModel.MenuItem.Id + extensions), FileMode.Create))
                {
                    files[0].CopyTo(filename);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemViewModel.MenuItem.Id + extensions;
            }
            menuItemFromDb.Category = MenuItemViewModel.MenuItem.Category;
            menuItemFromDb.SubCategory = MenuItemViewModel.MenuItem.SubCategory;
            menuItemFromDb.Price = MenuItemViewModel.MenuItem.Price;
            menuItemFromDb.Name = MenuItemViewModel.MenuItem.Name;
            menuItemFromDb.Spicyness = MenuItemViewModel.MenuItem.Spicyness;
            menuItemFromDb.Description = MenuItemViewModel.MenuItem.Description;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemViewModel.MenuItem = await _db.MenuItem.Include(s => s.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(s => s.Id == id);
            if (MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, MenuItemViewModel menuItemViewModel)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var menuItem = await _db.MenuItem.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound();
                }
                string webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.MenuItem.Remove(menuItem);

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemViewModel);

        }
    }
}
