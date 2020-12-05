using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerce.Models
{
    public class ProductViewModel
    {
        public long ProductId { get; set; }
        [Required(ErrorMessage = "Please enter Product Name"), MaxLength(250)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Please enter Product Description"), MaxLength(4000)]
        public string ProductDescription { get; set; }
        
        public ICollection<ProductAttributeViewModel> ListProductAttributes { get; set; }

        [Required(ErrorMessage = "Please select Category")]
        public int ProdCatId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<ProductCategoryViewModel> ListProductCategory { get; set; }
    }

    public class ProductAttributeViewModel
    {
        public long ProductId { get; set; }
        public int AttributeId { get; set; }

        [Required(ErrorMessage = "Please enter Attribute Name"), MaxLength(250)]
        public string AttributeValue { get; set; }
        public int ProdCatId { get; set; }
        public string AttributeName { get; set; }
    }

    public class ProductCategoryViewModel
    {
        public int ProdCatId { get; set; }
        public string CategoryName { get; set; }
    }

    public class ProductAttributeLookupViewModel
    {
        public int AttributeId { get; set; }
        public int ProdCatId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }
}