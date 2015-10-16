using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using HangfireDemo.Models;

namespace HangfireDemo.Mq
{
    public class MQHelper
    {

        private readonly HighlighterDbContext _dbContext = new HighlighterDbContext();
        private MessageQueue _queue;
        private static readonly string _queuePath = ConfigurationManager.AppSettings["MqPath"];
        private static readonly object _obj = new object();
        private MessageQueue Queue
        {
            get
            {
                if (_queue == null)
                {
                    if (!MessageQueue.Exists(_queuePath))
                    {
                        lock (_obj)
                        {
                            if (!MessageQueue.Exists(_queuePath))
                            {
                                _queue = MessageQueue.Create(_queuePath);
                            }
                        }

                    }
                    _queue = new MessageQueue(_queuePath)
                    {
                        Formatter = new XmlMessageFormatter(new Type[] { typeof(LogMessage) })
                    };
                    return _queue;
                }
                return _queue;
            }
        }

        /// <summary>
        /// 往队列中发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendAsync(LogMessage message)
        {
            new TaskFactory().StartNew(() => Queue.Send(new Message(message)));
        }

        public void Recevice()
        {
            Queue.ReceiveCompleted += ReceiveCompleted;
            while (true)
            {
                try
                {
                    using (var messageEnumerator = Queue.GetMessageEnumerator2())
                    {
                        while (messageEnumerator.MoveNext())
                        {
                            try
                            {
                                Queue.BeginReceive();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Exception:ReceiveMessage Exception:" + ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _dbContext.LogMessages.Add(new LogMessage()
                    {
                        CreatTime = DateTime.Now,
                        Status = Status.Error,
                        JobName = "MqReceiveError",
                        ErrorMsg = ex.Message + ex.StackTrace,
                        RequestTime = DateTime.Now
                    });
                    return;
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        /// <summary>
        /// ReceiveCompleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                lock (_obj)
                {
                    var messageQueue = (MessageQueue)sender;
                    var message = messageQueue.EndReceive(e.AsyncResult);
                    var messageObj = (LogMessage)message.Body;
                    _dbContext.LogMessages.Add(messageObj);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _dbContext.LogMessages.Add(new LogMessage()
                {
                    CreatTime = DateTime.Now,
                    Status = Status.Error,
                    JobName = "MqReceiveError",
                    ErrorMsg = ex.Message + ex.StackTrace,
                    RequestTime = DateTime.Now
                });
            }
        }

        public string Reset(string JobName)
        {
            try
            {
                if (!string.IsNullOrEmpty(JobName))
                {
                    RecurringJob.RemoveIfExists(JobName);
                    var objs = from Item in _dbContext.ConfigManager
                               where Item.JobName.Contains(JobName.ToLower())
                               select Item;
                    if (objs.Any())
                    {
                        var obj = objs.First();
                        obj.IsRuning = false;
                    }
                }
                else
                {
                    foreach (var obj in _dbContext.ConfigManager)
                    {
                        if (obj.IsValid && obj.IsRuning)
                        {
                            RecurringJob.RemoveIfExists(obj.JobName);
                            obj.IsRuning = false;
                        }
                    }
                }
                _dbContext.SaveChanges();
                return "Job Remove Ok";
            }
            catch (Exception ex)
            {
                return "job Reset Error:<br/>" + ex.Message + ex.Source;
            }
        }
    }
}
