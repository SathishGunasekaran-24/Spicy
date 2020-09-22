using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Models.View_Models
{
    public class OrderCartDetails
    {
        public List<ShopingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
