using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Management;
using Hangfire;
using Hangfire.SqlServer;
using HangfireDemo.Migrations;
using HangfireDemo.Models;
using HangfireDemo.Mq;
using Microsoft.Owin.Hosting;
using Owin;

namespace HangfireDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = ConfigurationManager.AppSettings["siteIp"];
            var helper = new MQHelper();
            new TaskFactory().StartNew(helper.Recevice, TaskCreationOptions.LongRunning);
            // Start OWIN host 
            try
            {
                using (WebApp.Start<Startup>(baseAddress))
                {
                    // Create HttpCient and make a request to api/values 
                    WebClient client = new WebClient();
                    client.DownloadData(new Uri(baseAddress + "api/report?JobName"));
                    client.DownloadData(new Uri(baseAddress + "api/report"));
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            helper.Reset(string.Empty);
            Console.ReadLine();
        }
    }
}
