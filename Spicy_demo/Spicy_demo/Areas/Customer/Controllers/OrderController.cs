using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Spicy_demo.Data;
using Spicy_demo.Models;
using Spicy_demo.Models.View_Models;

namespace Spicy_demo.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {

            var claimIdentiy = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentiy.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel
            {
                orderHeader = await _db.OrderHeader.Include(o => o.ApplicationUser).FirstOrDefaultAsync(c => c.Id == id & c.UserId == claim.Value),
                listOrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };
            return View(orderDetailsViewModel);
        }
        public async Task<IActionResult> OrderHistory()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            List<OrderHeader> orderHeadersList = await _db.OrderHeader.Include(c=>c.ApplicationUser).Where(c => c.UserId == claim.Value).ToListAsync();
            return View(orderHeadersList);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
