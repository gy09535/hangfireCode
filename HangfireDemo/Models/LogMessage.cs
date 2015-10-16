using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangfireDemo.Models
{
    public class LogMessage
    {
        public int Id { get; set; }

        [Required]
        public string JobName { get; set; }

        public DateTime RequestTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatTime { get; set; }
        public string ErrorMsg { get; set; }
    }

    public class Status
    {
        public static string Success = "Success";
        public static string Error = "Error";
    }
}
