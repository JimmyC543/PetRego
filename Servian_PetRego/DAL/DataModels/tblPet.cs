﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Servian_PetRego.DAL.DataModels
{
    public class tblPet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int FKAnimalTypeId { get; set; }//private set?
        [ForeignKey(nameof(FKAnimalTypeId))]
        public LkpAnimalType AnimalType { get; set; }

        public Guid FKOwnerId { get; set; }
        [ForeignKey(nameof(FKOwnerId))]
        public tblOwner Owner { get; set; }
    }
}
