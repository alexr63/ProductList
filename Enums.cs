using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductList
{
    public class Enums
    {
        public enum ProductTypeEnum
        {
            Hotels = 1,
            Clothes,
            Books,
            HomeAndGardens,
            Boats
        }

        public enum DisplayModeEnum 
        {
            Hotels = 1,
            Products,
            Boats
        }
        
        public enum SortCriteriaEnum
        {
            Name = 1,
            PriceAsc,
            PriceDesc,
            RatingAsc,
            RatingDesc,
            DistanceAsc,
            DistanceDesc
        }

    }
}