using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spicy_demo.Data;
using Spicy_demo.Models;
using Spicy_demo.Utility;

namespace Spicy_demo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Coupon Coupon { get; set; }

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
            Coupon = new Coupon();
        }
        public async Task<IActionResult> Index()
        {
            var coupons = await _db.Coupon.ToListAsync();
            return View(coupons);
        }
        public async Task<IActionResult> Create()
        {
            return View(Coupon);
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    Coupon.Picture = p1;
                }
                _db.Coupon.Add(Coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Coupon);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(c=>c.Id==id).FirstOrDefaultAsync();
            return View(coupon);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    Coupon.Picture = p1;
                }
                coupon.Name = Coupon.Name;
                coupon.IsActive = Coupon.IsActive;
                coupon.MinimumAmount = Coupon.MinimumAmount;
                coupon.Discount = Coupon.Discount;
                coupon.CouponType = Coupon.CouponType;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Coupon);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(c => c.Id == id).FirstOrDefaultAsync();
            return View(coupon);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(c => c.Id == id).FirstOrDefaultAsync();
            return View(coupon);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = await _db.Coupon.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (coupon != null)
            {
                _db.Coupon.Remove(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}
