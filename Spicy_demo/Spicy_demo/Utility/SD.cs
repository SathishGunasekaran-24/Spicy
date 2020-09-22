using Spicy_demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Utility
{
    public class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
		public const string ssShopingCartCount = "ssCartCode";
		public const string ssCouponCode = "ssCouponCode";


		public const string StatusSubmitted = "Submitted";
		public const string StatusInProcess = "Being Prepared";
		public const string StatusReady = "Ready for Pickup";
		public const string StatusCompleted = "Completed";
		public const string StatusCancelled = "Cancelled";

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";








		public static string ConvertToRawHtml(string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		public static double DiscountedPrice(Coupon couponFromDb, double orginalPrice )
        {
			if(couponFromDb == null)
            {
				return orginalPrice;
            }
            else
            {
                if (couponFromDb.MinimumAmount > orginalPrice)
                {
					return orginalPrice;
				}
                else
                {
					if(Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Dollor)
                    {
						return Math.Round(orginalPrice - couponFromDb.Discount,2);
                    }
					else if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupon.ECouponType.Percent)
					{
						return Math.Round(orginalPrice - (couponFromDb.Discount * orginalPrice/100), 2);
					}
				}
            }
			return orginalPrice;
        }

	}
}
