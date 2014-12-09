using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private readonly CloudStorageAccount _storageAccount;

        public HomeController()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            ViewBag.Version = version.ToString();

            // Retrieve the storage account from the connection string.
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

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
                CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();

                CloudTable table = tableClient.GetTableReference("coolPeople");

                var person = new Person(emailAddress);

                if (CheckIfCoolPersonExists(person, table))
                {
                    ViewBag.Message = string.Format("SNEAKY ;) {0} has already signed up.", person.EmailAddress);
                    return View();
                }
                InsertCoolPerson(person, table);
            }
            catch (Exception e)
            {
                ViewBag.Message = "PROBLEMZ :( " + e.Message;
            }

            return View();
        }

        private bool CheckIfCoolPersonExists(Person person, CloudTable table)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Person>(person.PartitionKey, person.RowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            return retrievedResult.Result != null;
        }

        private void InsertCoolPerson(Person person, CloudTable table)
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(person);

            // Execute the insert operation.
            table.Execute(insertOperation);

            ViewBag.Message = "Wait one day for kitteh goodness.";
        }

        [HttpGet]
        public ActionResult Upload()
        {
            ViewBag.Message = "Chooz image file";

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
                // Create the blob client.
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("lolcatz");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);

                using (var stream = new StreamReader(file.InputStream))
                {
                    blockBlob.UploadFromStream(stream.BaseStream);
                }

                ViewBag.Message = "File uploaded successfully";

                // Insert tags for img
                var uploadTags = tags.Split(';').ToList();
                uploadTags.Remove(string.Empty);

                if (uploadTags.Any())
                {
                    var tableClient = _storageAccount.CreateCloudTableClient();
                    var table = tableClient.GetTableReference("imgTags");

                    foreach (var uploadTag in uploadTags)
                    {
                        var insertOperation = TableOperation.Insert(new ImageTag(uploadTag, blockBlob.Uri));
                        table.Execute(insertOperation);
                    }
                }


            }
            catch (Exception e)
            {
                ViewBag.Message = "PROBLEMZ :( " + e.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Search(IEnumerable<string> model)
        {
            ViewBag.Message = "Search here for da kittehs. Seperate tags with semi-colons (;)";

            return View();
        }

        [HttpPost]
        public ActionResult Search(string tags)
        {
            var searchTags = tags.Split(';');

            // Create the table client.
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("imgTags");

            // List to hold kitteh uris
            var imgUris = new List<string>();

            foreach (var searchTag in searchTags)
            {
                var query = new TableQuery<ImageTag>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, searchTag));

                // add results to list
                imgUris.AddRange(table.ExecuteQuery(query).Select(tag => tag.ImageUri.ToString()));
            }


            return View(imgUris);
        }
    }
}