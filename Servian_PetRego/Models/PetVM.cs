using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.Models
{
    public class PetVM : PetCoreVM
    {
        [BindNever]
        public string AnimalType { get; set; }//Human-readable value of Animal Type
        [BindNever]
        public string OwnersName { get; set; }
    }
}
