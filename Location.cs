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
    
    public partial class Location
    {
        public Location()
        {
            this.Hotels = new HashSet<Hotel>();
            this.SubLocations = new HashSet<Location>();
            this.HotelLocations = new HashSet<HotelLocation>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ICollection<Hotel> Hotels { get; set; }
        public virtual ICollection<Location> SubLocations { get; set; }
        public virtual Location ParentLocation { get; set; }
        public virtual ICollection<HotelLocation> HotelLocations { get; set; }
    }
}
