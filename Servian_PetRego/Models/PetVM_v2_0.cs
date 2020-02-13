using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.Models
{
    public class PetVM_v2_0 : PetCoreVM
    {
        [BindNever]
        public string AnimalType { get; set; }//Human-readable value of Animal Type

        [BindNever]
        public string FoodSource { get; set; }

        [BindNever]
        public string OwnersName { get; set; }
    }
}
