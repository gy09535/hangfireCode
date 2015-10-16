using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace HostStart.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home
        public ActionResult StartJobServer()
        {
            try
            {
                var process = Process.GetProcessesByName("HangfireDemo");
                if (process.Any())
                {
                    return Content("Job Server is already running");
                }
                Process p = new Process
                {
                    StartInfo =
                        new ProcessStartInfo(@ConfigurationManager.AppSettings["StartProcess"]) { UseShellExecute = false }
                };

                p.Start();


                return Content("job servers  starts Successful");

            }
            catch (Exception ex)
            {
                return Content("error:" + ex.Message + ex.Source);
            }
        }

        public ActionResult StopServer()
        {
            try
            {
                var process = Process.GetProcessesByName("HangfireDemo");
                if (process.Count() >= 0)
                {
                    foreach (var p in process)
                    {
                        p.Kill();
                    }
                    return Content("job servers  stoped Successful");
                }
                return Content("Job Server is already stoped");
            }
            catch (Exception ex)
            {
                return Content("error:" + ex.Message + ex.Source);
            }
        }
    }
}
