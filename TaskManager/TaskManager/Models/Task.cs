using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class Task
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string notes { get; set; }

        [Required(ErrorMessage = "This field[Start Date] mustn't be empty!")] // If this field is empty, then show the error message.
        public DateTime startDate { get; set; }
        [Required(ErrorMessage = "This field[End Date] mustn't be empty!")] // If this field is empty, then show the error message.
        public DateTime endDate { get; set; }
        public Boolean isCompleted { get; set; }
    }
}
