using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MCVUI.Areas.Admin.AdminVMClasses
{
    public class CategoryVM
    {
        public List<Category>  Categories { get; set; }
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
    }
}