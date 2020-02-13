using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetRego.BLL
{
    public class TaskResult
    {
        public object Data { get; set; }

        public bool Success { get; set; } = true;

        //This could be expanded, perhaps to contain exceptions, or a ValidationMessage class
        public IEnumerable<string> ValidationMessages { get; set; }

    }
}
