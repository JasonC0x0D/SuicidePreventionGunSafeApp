using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Lock
    {
        [Required]
        [Display(Name = "Lock Id")]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Lock Name")]
        public string name { get; set; }
        [Display(Name = "Locked")]
        public bool locked { get; set; }
        [Display(Name = "Request Pending")]
        public bool request { get; set; }
        [Display(Name = "Alarm Detected")]
        public bool alarm { get; set; }
    }

    //// May be better to use bool values?
    //public enum locked { yes = 1, no = 0 }
    //public enum request { yes = 1, no = 0 }
    //public enum alarm { yes = 1, no = 0 }
}
