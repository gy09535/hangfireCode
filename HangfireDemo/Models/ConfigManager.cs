using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireDemo.Models
{
    public class ConfigManagers
    {
        public int Id { get; set; }
        [Required, Display(Name = "C# source")]
        public string RequestUrl { get; set; }
        public string Times { get; set; }
        public string CreatePerson { get; set; }
        public DateTime? CreateTime { get; set; }
        //是否有效
        public bool IsValid { get; set; }

        public bool IsRuning { get; set; }
        [Required]
        public string JobName { get; set; }
    }
}
