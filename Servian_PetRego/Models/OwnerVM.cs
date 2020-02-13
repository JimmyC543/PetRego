using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PetRego.Models
{
    public class OwnerVM
    {
        public Guid? OwnerId { get; set; } = null;

        [Required]
        [StringLength(50, ErrorMessage = "First name must be shorter than 51 characters.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z'\s]{0,49}$", ErrorMessage = "Names can start with only a letter, then any combination of letters, spaces, apostrophes and hyphens.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Last name must be shorter than 51 characters.")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z'\s]{0,49}$", ErrorMessage = "Names can start with only a letter, then any combination of letters, spaces, apostrophes and hyphens.")]
        public string LastName { get; set; }

        public IEnumerable<PetVM> Pets { get; set; } = new List<PetVM>();
    }
}
