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
    
    public partial class Hotel : Product
    {
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> Rooms { get; set; }
        public Nullable<decimal> Star { get; set; }
        public Nullable<decimal> CustomerRating { get; set; }
        public string Address { get; set; }
        public string CurrencyCode { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Lon { get; set; }
    
        public virtual Location Location { get; set; }
    }
}
