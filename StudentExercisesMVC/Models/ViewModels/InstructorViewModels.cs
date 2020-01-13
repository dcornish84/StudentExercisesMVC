using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class InstructorViewModels
    {
        public Instructors Instructor { get; set; }

        public List<SelectListItem> Cohorts { get; set; }
    }
}
