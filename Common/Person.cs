using Microsoft.WindowsAzure.Storage.Table;

namespace Common
{
    public class Person : TableEntity
    {
        public Person(string emailAddress)
        {
            int indexOfAtSign = emailAddress.IndexOf("@", System.StringComparison.Ordinal);
            var name = emailAddress.Substring(0, indexOfAtSign);
            var domain = emailAddress.Substring(indexOfAtSign + 1, emailAddress.Length - 1 - indexOfAtSign);

            this.PartitionKey = domain;
            this.RowKey = name;
            EmailAddress = emailAddress;
        }

        //TODO: This is a bug
        //public Person()
        //{
        //}

        public string EmailAddress { get; set; }
    }
}
