using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public Student Student { get; set; }

        public List<SelectListItem> Cohorts { get; set; }

        public List<SelectListItem> Exercises { get; set; }
    }
}
