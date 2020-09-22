using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Dollor=0,Percent=1}
        [Required]
        public double Discount { get; set; }
        [Required]
        public double MinimumAmount { get; set; }
        public byte[] Picture { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
