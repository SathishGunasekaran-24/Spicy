using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy_demo.Models.View_Models
{
    public class SubcategoryandcategoryViewmodel
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryList { get; set; }
        public string StatusMesssage { get; set; }
    }
}
