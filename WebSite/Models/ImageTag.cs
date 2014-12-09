using System;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebSite.Models
{
    public class ImageTag : TableEntity
    {
        public ImageTag(string tag, Uri imageUri)
        {
            PartitionKey = tag;
            RowKey = HttpUtility.UrlEncode(imageUri.ToString());
        }

        public ImageTag() { }

        public Uri ImageUri
        {
            get
            {
                var url = HttpUtility.UrlDecode(RowKey);
                return new Uri(url);
            }
        }
    }
}
