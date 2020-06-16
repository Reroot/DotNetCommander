using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Pfe.Xrm;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.Net.Http;

namespace Azure_Functions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var orgProxy = GetOrganizationServiceProxy();


            var fetchXml = $@"
<fetch top='10'>
  <entity name='contact'>
    <attribute name='emailaddress1' />
    <attribute name='firstname' />
    <attribute name='mobilephone' />
    <attribute name='lastname' />
  </entity>
</fetch>";


            var Query = new Dictionary<string, QueryBase>();
            Query.Add("contact", new FetchExpression(fetchXml));

            var Results = orgProxy.ParallelProxy.RetrieveMultiple(Query, true)["contact"].Entities;
            foreach (Entity Result in Results)
            {
                // Displaying all Accounts name...
                if (Result.Contains("mobilephone"))
                log.Info("Successfully Retrieved---------> " + Result.Attributes["mobilephone"].ToString());
            }


            return req.CreateResponse(HttpStatusCode.OK, Results);

        }
        private static OrganizationServiceManager GetOrganizationServiceProxy()
        {
            try
            {
                var orgName = "notsmooth";    // CRM Organization Name
                var userName = "readonly@notsmooth.onmicrosoft.com";   // User Name
                var password = "$kjmnw1234";                          // Password
                var uri = XrmServiceUriFactory.CreateOnlineOrganizationServiceUri(orgName);
                var serviceManager = new OrganizationServiceManager(uri, userName, password);
                return serviceManager;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
