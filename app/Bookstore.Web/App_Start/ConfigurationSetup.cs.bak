using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using BobsBookstoreClassic.Data;
using Bookstore.Common;

namespace Bookstore.Web
{
    public static class ConfigurationSetup
    {
        public static void ConfigureConfiguration()
        {
            var rootPath = "/" + Constants.AppName;

            const string databasePath = "/Database";
            const string authenticationPath = "/Authentication";
            const string fileServicePath = "/Files";


            if (BookstoreConfiguration.Get("Services/Database") == "aws")
            {
                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest { Name = $"{rootPath}{databasePath}/ConnectionStrings/BookstoreDatabaseConnection" };
                    var response = client.GetParameter(request);

                    BookstoreConfiguration.Add(response.Parameter.Name.Replace($"{rootPath}{databasePath}/", string.Empty), response.Parameter.Value);
                }
            }

            if (BookstoreConfiguration.Get("Services/Authentication") == "aws")
            {
                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParametersByPathRequest { Path = $"{rootPath}{authenticationPath}/", Recursive = true };
                    var response = client.GetParametersByPath(request);

                    foreach (var parameter in response.Parameters)
                    {
                        BookstoreConfiguration.Add(parameter.Name.Replace($"{rootPath}/", string.Empty), parameter.Value);
                    }
                }
            }

            if (BookstoreConfiguration.Get("Services/FileService") == "aws")
            {
                using (var client = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParametersByPathRequest { Path = $"{rootPath}{fileServicePath}/", Recursive = true };
                    var response = client.GetParametersByPath(request);

                    foreach (var parameter in response.Parameters)
                    {
                        BookstoreConfiguration.Add(parameter.Name.Replace($"{rootPath}/", string.Empty), parameter.Value);
                    }
                }
            }
        }
    }
}