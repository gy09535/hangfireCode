using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Hangfire;
using Hangfire.Common;
using HangfireDemo.Models;
using HangfireDemo.Mq;

namespace HangfireDemo
{
    public class ReportController : ApiController
    {

        private readonly HighlighterDbContext _dbContext = new HighlighterDbContext();
        private readonly MQHelper helper = new MQHelper();
        public string Get()
        {

            try
            {
                foreach (var obj in _dbContext.ConfigManager)
                {
                    if (obj.IsValid && !obj.IsRuning)
                    {
                        try
                        {
                            var times = !string.IsNullOrEmpty(obj.Times) ? obj.Times : Cron.Minutely();
                            ConfigManagers obj1 = obj;
                            RecurringJob.AddOrUpdate(obj.JobName, () => Method(obj1), times);
                            obj.IsRuning = true;

                        }
                        catch (Exception ex)
                        {
                            _dbContext.LogMessages.Add(new LogMessage()
                            {
                                CreatTime = DateTime.Now,
                                Status = Status.Error,
                                JobName = obj.JobName,
                                ErrorMsg = ex.Message + ex.StackTrace,
                                RequestTime = DateTime.Now
                            });
                        }
                    }
                }
                _dbContext.SaveChanges();
                return "job started";
            }
            catch (Exception ex)
            {
                return "job Start Error:<br/>" + ex.Message + ex.Source;
            }
        }

        public void Method(ConfigManagers manager)
        {
            try
            {
                var client = new WebClient();
                client.DownloadData(new Uri(manager.RequestUrl));
                new MQHelper().SendAsync(new LogMessage()
                 {
                     CreatTime = DateTime.Now,
                     Status = Status.Success,
                     JobName = manager.JobName,
                     ErrorMsg = "NO",
                     RequestTime = DateTime.Now
                 });
            }
            catch (Exception ex)
            {
                helper.SendAsync(new LogMessage()
                {
                    CreatTime = DateTime.Now,
                    Status = Status.Error,
                    JobName = manager.JobName,
                    ErrorMsg = ex.Message + ex.StackTrace,
                    RequestTime = DateTime.Now
                });
            }
        }

        public string Get(string JobName)
        {
            try
            {
                return helper.Reset(JobName);
            }
            catch (Exception ex)
            {
                return "job Reset Error:<br/>" + ex.Message + ex.Source;
            }
        }

        // POST api/values 
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5 
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
