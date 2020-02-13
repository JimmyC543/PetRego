using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.Models
{
    public class PetFoodVM
    {
        public Guid PetId { get; set; }
        public string FoodSource { get; set; }//TODO: If FoodSource gets its own data table, add the FK ID too
    }
}
