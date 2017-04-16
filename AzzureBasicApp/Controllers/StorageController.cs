using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzzureBasicApp.Controllers
{
    public class StorageController : Controller
    {
        // GET: Storage
        public ActionResult Index()
        {
            var s = ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString;
            var sa = CloudStorageAccount.Parse(s);

            var tableClient = sa.CreateCloudTableClient();

            var table = tableClient.GetTableReference("customer");

            table.CreateIfNotExists();

            

            var customer = new Customer(Guid.NewGuid())
            {
                First = "dave",
                Last = "smith",
                Email = "aaa.aol.com"

            };

            var insert = TableOperation.Insert(customer);
            table.Execute(insert);

            var queueClient = sa.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("customerqueue");
            var msg = new CloudQueueMessage(customer.RowKey);
            queue.CreateIfNotExists();
            queue.AddMessage(msg);
            

            
            return View(customer);

        }
    }

    public class Customer : TableEntity
    {
        public Customer(Guid id)
        {
            PartitionKey = "customer";
            RowKey = id.ToString();
        }

        public Customer() { }
        public string First { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

}