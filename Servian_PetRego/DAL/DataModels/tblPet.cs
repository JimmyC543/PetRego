using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL.DataModels
{
    public class tblPet
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int FKAnimalTypeId { get; set; }//private set?
        [ForeignKey(nameof(FKAnimalTypeId))]
        public LkpAnimalType AnimalType { get; set; }

        
        public Guid? FKOwnerId { get; set; }
        [ForeignKey(nameof(FKOwnerId))]
        public tblOwner Owner { get; set; }
    }
}
