//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductList
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public Product()
        {
            this.Categories = new HashSet<Categorie>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public Nullable<decimal> UnitCost { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public int CreatedByUser { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ICollection<Categorie> Categories { get; set; }
    }
}