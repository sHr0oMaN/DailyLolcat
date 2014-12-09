using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Signup()
        {
            ViewBag.Message = "Your application description page.";

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
            /*
                TODO:
             *          1. Download file
             *          2. Check for uniqueness (MD5 hash)
             *          3. If unqiue,
             *              3.1 upload, show thanks
             *              3.2 else show thanks
             */

            if (file == null || file.ContentLength <= 0)
            {
                ViewBag.Message = "You have not specified a file.";
            }

            try
            {
                // Retrieve storage account from connection string.
                var connectionString = CloudConfigurationManager.GetSetting("BlobStorageConnectionString");
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
                ViewBag.Message = "ERROR:" + e.Message + "<br/><br/>";
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