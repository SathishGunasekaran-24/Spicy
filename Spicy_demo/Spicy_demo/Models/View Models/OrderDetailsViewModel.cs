using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Models.View_Models
{
    public class OrderDetailsViewModel
    {
        public OrderHeader orderHeader { get; set; }
        public List<OrderDetails> listOrderDetails { get; set; }
    }
}
