using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetRego.DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.DAL
{
    public static class DbInitialiser
    {
        /// <summary>
        /// If the DB hasn't yet been seeded, Initialise will add the initial Animal Types so that pets can be correctly added.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialise(IServiceProvider serviceProvider)
        {
            using (var context = new PetRegoDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<PetRegoDbContext>>()))
            {

                context.Database.EnsureCreated();
                if (context.AnimalTypes.Any())
                {
                    return; // We've already seeded the db
                }

                context.AnimalTypes.AddRange(
                    new LkpAnimalType { AnimalType = "Dog", FoodSource = "Bones" },     // Id = 1,
                    new LkpAnimalType { AnimalType = "Cat", FoodSource = "Fish" },      // Id = 2,
                    new LkpAnimalType { AnimalType = "Chicken", FoodSource = "Corn" },  // Id = 3,
                    new LkpAnimalType { AnimalType = "Snake", FoodSource = "Mice" }     // Id = 4.
                    );

                context.SaveChanges();
            }
        }
    }
}
