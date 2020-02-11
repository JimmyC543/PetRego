using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL.DataModels
{
    //TODO: Consider making this an enum, if no other animal types/food sources are to be added?
    public class LkpAnimalType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string AnimalType { get; set; }

        //TODO: Should this be a lookup table or perhaps an enum?
        [StringLength(20)]
        public string FoodSource { get; set; }

        //Futures: If possible to expand to multiple food sources per animal type, consider the following:
        //public ICollection<lkpFoodSource> FoodSources { get; set; } = new List<lkpFoodSource>();
        //With an additional table lkpFoodSource with an int Id, string FoodSource, and int FKAnimalType
        //(or even consider a many-to-many relationship between food sources and animal types...)
    }
}
