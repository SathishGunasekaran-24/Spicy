using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spicy_demo.Data;
using Spicy_demo.Data.Migrations;
using Spicy_demo.Models;
using Spicy_demo.Models.View_Models;
using Spicy_demo.Utility;

namespace Spicy_demo.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()

            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim!=null)
            {
                var cart = _db.ShopingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShopingCartCount, cart);
            }
            return View(indexViewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }
            ShopingCart shopingCart = new ShopingCart()
            {
                MenuItemId = menuItem.Id,
                MenuItem = menuItem
            };
            return View(shopingCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShopingCart cartObj)
        {
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShopingCart cartFromDb = await _db.ShopingCart.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId 
                                           && c.MenuItemId == cartObj.MenuItemId).FirstOrDefaultAsync();
                if (cartFromDb==null)
                {
                    await _db.ShopingCart.AddAsync(cartObj);
                    
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count+cartObj.Count;
                }
                await _db.SaveChangesAsync();
                var count = _db.ShopingCart.Where(c => c.ApplicationUserId == cartObj.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShopingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var menuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == cartObj.Id);
                if (menuItem == null)
                {
                    return NotFound();
                }
                ShopingCart shopingCart = new ShopingCart()
                {
                    MenuItemId = menuItem.Id,
                    MenuItem = menuItem
                };
                return View(cartObj);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
