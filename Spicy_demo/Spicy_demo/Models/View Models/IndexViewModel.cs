using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Models.View_Models
{
    public class IndexViewModel
    {
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<MenuItem> MenuItem { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }
    }
}
