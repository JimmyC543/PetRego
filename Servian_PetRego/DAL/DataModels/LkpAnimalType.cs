using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL.DataModels
{
    //TODO: Consider making this an enum, if no other animal types are to be added?
    public class LkpAnimalType
    {
        public int Id { get; set; }
        public string AnimalType { get; set; }

        //TODO: Should this be a lookup table or perhaps an enum?
        public string FoodSource { get; set; }
    }
}
