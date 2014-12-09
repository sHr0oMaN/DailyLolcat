using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.Message = "Sign up to receive a random lolcat in urz email each day.";

            return View();
        }

        [HttpPost]
        public ActionResult Signup(string emailAddress)
        {
            if (emailAddress.IsNullOrWhiteSpace())
            {
                ViewBag.Message = "Please enter an email address.";
                return View();
            }

            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference("coolPeople");

                var person = new Person(emailAddress);

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(person);

                // Execute the insert operation.
                table.Execute(insertOperation);

                ViewBag.Message = "Wait one day for kitteh goodness.";
            }
            catch (Exception e)
            {
                ViewBag.Message = "PROBLEMZ :( " + e.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string tags)
        {
            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.Message = "You have not specified a file.";
                return View();
            }

            try
            {
                // Retrieve storage account from connection string.
                var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("lolcatz");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                using (var stream = new StreamReader(file.InputStream))
                {
                    blockBlob.UploadFromStream(stream.BaseStream);
                }

                ViewBag.Message = "File uploaded successfully";
            }
            catch (Exception e)
            {
                ViewBag.Message = "PROBLEMZ :( " + e.Message;
            }

            return View(); 
        }

        public ActionResult ViewRandom()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}