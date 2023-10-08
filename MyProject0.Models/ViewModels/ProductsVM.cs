﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject0.Models.ViewModels
{
    public class ProductsVM
    {
        public Product Product { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CatagoryList { get; set; }
    }
}
