using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spicy_demo.Data;
using Spicy_demo.Models;
using Spicy_demo.Models.View_Models;
using Spicy_demo.Utility;

namespace Spicy_demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public OrderCartDetails detailsCart { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            detailsCart = new OrderCartDetails
            {
                OrderHeader = new Models.OrderHeader()
            };
            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShopingCart.Where(C => C.ApplicationUserId == claim.Value);
            if (cart!=null)
            {
                detailsCart.ListCart = cart.ToList(); ;
            }
            foreach (var list in detailsCart.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(c => c.Id == list.MenuItemId);
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            detailsCart.OrderHeader.OrderTotalOrginal = detailsCart.OrderHeader.OrderTotal;
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _db.Coupon.Where(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOrginal);
            }
            return View(detailsCart);
        }
        public async Task<IActionResult> Summary(int id)
        {
            detailsCart = new OrderCartDetails
            {
                OrderHeader = new Models.OrderHeader()
            };
            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationuser =await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShopingCart.Where(C => C.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailsCart.ListCart = cart.ToList(); ;
            }
            foreach (var list in detailsCart.ListCart)
            {
                list.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(c => c.Id == list.MenuItemId);
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);
            }
            detailsCart.OrderHeader.OrderTotalOrginal = detailsCart.OrderHeader.OrderTotal;
            detailsCart.OrderHeader.PickUpName = applicationuser.Name;
            detailsCart.OrderHeader.PhoneNumber = applicationuser.PhoneNumber;
            detailsCart.OrderHeader.PickUpTime = DateTime.Now;
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _db.Coupon.Where(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOrginal);
            }
            return View(detailsCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            detailsCart.OrderHeader.OrderTotal = 0;
            detailsCart.ListCart = await _db.ShopingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();
            detailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailsCart.OrderHeader.OrderDate = DateTime.Now;
            detailsCart.OrderHeader.UserId = claim.Value;
            detailsCart.OrderHeader.Status = SD.StatusInProcess;

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(detailsCart.OrderHeader);
            await _db.SaveChangesAsync();

            detailsCart.OrderHeader.OrderTotalOrginal = 0;


            foreach (var item in detailsCart.ListCart)
            {
                item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                detailsCart.OrderHeader.OrderTotalOrginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);

                //return RedirectToAction("Confirm","Order",new {id = detailsCart.OrderHeader.Id });
            }



            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _db.Coupon.Where(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOrginal);
            }
            else
            {
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotalOrginal;
            }
            detailsCart.OrderHeader.CouponCodeDiscount = detailsCart.OrderHeader.OrderTotalOrginal - detailsCart.OrderHeader.OrderTotal;

            _db.ShopingCart.RemoveRange(detailsCart.ListCart);
            HttpContext.Session.SetInt32(SD.ssShopingCartCount, 0);
            await _db.SaveChangesAsync();
            return RedirectToAction("Confirm", "Order", new { id = detailsCart.OrderHeader.Id });
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCoupon()
        {
            if (detailsCart.OrderHeader.CouponCode == null)
            {
                detailsCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailsCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveCoupon()
        {
            if (detailsCart.OrderHeader.CouponCode == null)
            {
                detailsCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShopingCart.FirstOrDefaultAsync(m => m.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> minus(int cartId)
        {
            var cart = await _db.ShopingCart.FirstOrDefaultAsync(m => m.Id == cartId);
            if (cart.Count==1)
            {
                _db.ShopingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShopingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShopingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> remove(int cartId)
        {
            var cart = await _db.ShopingCart.FirstOrDefaultAsync(m => m.Id == cartId);
            _db.ShopingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShopingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.ssShopingCartCount, cnt);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderHistory()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<OrderHeader> orderHeadersList = await _db.OrderHeader.Where(c => c.UserId == claim.Value).ToListAsync();
            return View(orderHeadersList);
        }
    }
}
