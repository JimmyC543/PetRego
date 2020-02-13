using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.Models
{
    public class OwnerVM_v2_0
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

        public IEnumerable<PetVM_v2_0> Pets { get; set; } = new List<PetVM_v2_0>();
    }
}
