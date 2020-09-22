using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spicy_demo.Data;
using Spicy_demo.Models;
using Spicy_demo.Models.View_Models;
using Spicy_demo.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategory.Include(s=>s.Category).ToListAsync();
            return View(subCategories);
            
        }
        public async Task<IActionResult> Create()
        {
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(subcategoryandcategoryViewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubcategoryandcategoryViewmodel model)
        {
            if(ModelState.IsValid)
            {
                var subCategories = await _db.SubCategory.Include(s => s.Category).Where(s=>s.Name==model.SubCategory.Name && s.Category.Id==model.SubCategory.CategoryId).ToListAsync();
                if (subCategories.Count()>0)
                {
                    StatusMessage = "Error: SubCategory exists under" + subCategories.First().Category.Name + "category. Please use another name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMesssage = StatusMessage
            };
            return View(subcategoryandcategoryViewmodel);

        }

        //Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory==null)
            {
                return NotFound();
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(subcategoryandcategoryViewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubcategoryandcategoryViewmodel model)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var subCategories = await _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId).ToListAsync();
                if (subCategories.Count() > 0)
                {
                    StatusMessage = "Error: SubCategory exists under" + subCategories.First().Category.Name + "category. Please use another name.";
                }
                else
                {
                    var subCategory = await _db.SubCategory.FindAsync(id);
                    subCategory.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMesssage = StatusMessage
            };
            subcategoryandcategoryViewmodel.SubCategory.Id = id;
            return View(subcategoryandcategoryViewmodel);

        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(subcategoryandcategoryViewmodel);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(subcategoryandcategoryViewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, SubcategoryandcategoryViewmodel model)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return NotFound();
                }
                else
                {
                    var subCategory = await _db.SubCategory.FindAsync(id);
                    if (subCategory == null)
                    {
                        return NotFound();
                    }
                    _db.SubCategory.Remove(subCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubcategoryandcategoryViewmodel subcategoryandcategoryViewmodel = new SubcategoryandcategoryViewmodel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMesssage = StatusMessage
            };
            subcategoryandcategoryViewmodel.SubCategory.Id = id;
            return View(subcategoryandcategoryViewmodel);

        }
    }
}
