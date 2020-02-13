using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.Models
{
    public class PetCoreVM
    {
        public Guid? PetId { get; set; } = null;
        [Required]
        [StringLength(50, ErrorMessage = "Pet name is too long!")]
        public string Name { get; set; }

        [Required]
        public int? AnimalTypeId { get; set; }

        public Guid? OwnerId { get; set; } = null;
    }
}
