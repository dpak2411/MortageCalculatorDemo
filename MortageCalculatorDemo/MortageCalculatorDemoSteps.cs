using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using AlteryxGalleryAPIWrapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using System.Windows.Forms;

namespace MortageCalculatorDemo
{
    [Binding]
    public class MortageCalculatorDemoSteps
    {
        private string _devurl;
        private string alteryxurl;
        private string _sessionid;
        private string _id;
        private string _userid;
        private string jobid;
        private string outputid;
      //  private string getmetadata;
        // Client obj = new Client("https://gallery.alteryx.com/api/");

        Client obj = new Client("http://devgallery.alteryx.com/api/");

        [Given(@"alteryx running at""(.*)""")]
        public void GivenAlteryxRunningAt(string url)
        {
            alteryxurl = url;
        }

        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {

            _sessionid = obj.Authenticate(user, password).sessionId;

        }

        //[When(@"I publish application: mortgage calculator")]
        ////public void WhenIPublishApplicationMortgageCalculator()
        ////{

        ////    ScenarioContext.Current.Pending();
        ////}

        [Given(@"I run mortgage calculator with principle (.*) interest (.*) payments (.*)")]
        public void GivenIRunMortgageCalculatorWithPrincipleInterestPayments(int principal, Decimal interest, int numpayment)
        {
            //url + "/apps/studio/?search=" + appName + "&limit=20&offset=0"
            //Search for App & Get AppId & userId 
            string response = obj.SearchApps("mortgage");
            var appresponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(response);
            int count = appresponse["recordCount"];
            for (int i = 0; i <= count - 1; i++)
            {
                _id = appresponse["records"][i]["id"];
               _userid =appresponse["records"][i]["owner"]["id"];
            }            
            
            //url +"/apps/" + appPackageId + "/interface/
            //Get the app interface - not required
            string appinterface = obj.GetAppInterface(_id);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);
             string keyv ="[{\"key\":\"Payment\",\"value\":true}]";
            //Post the response to Job Queue
             string jsonparam = "{\"appPackage\":{\"id\":\"" + _id + "\"},\"appName\":\"Mortgage_Calculator.yxwz\",\"jobName\":\"\",\"userId\":\"" + _userid + "\",\"questions\":[{\"name\":\"IntRate\",\"answer\":\"" + interest + "\"},{\"name\":\"NumPayments\",\"answer\":\"" + numpayment + "\"},{\"name\":\"Payment\",\"answer\":1834.41},{\"name\":\"LoanAmount\",\"answer\":\"" + principal + "\"}]}"; 
             string resjobqueue = obj.QueueJob(jsonparam);
             
            var jobqueue =new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(resjobqueue);
            jobid = jobqueue["id"];
            
            //Get the job status
            
            string status = "";
            while (status != "Completed")
            {
                string jobstatusresp = obj.GetJobStatus(jobid);
                var statusresp = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(jobstatusresp);
                status = statusresp["status"];
            }
           

        }


        [Then(@"I see output (.*)")]
        public void ThenISeeOutput(int answer)
        {
            //url + "/apps/jobs/" + jobId + "/output/"
            string getmetadata = obj.GetOutputMetadata(jobid);
            dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);         
            outputid = metadataresp[0]["id"];
            //int count = metadataresp.Length;
            //for (int j = 0; j <= count-1 ; j++)
            //{
            //    outputid = metadataresp[j]["id"];
            //}
                    
           
            string getjoboutput = obj.GetJobOutput(jobid, outputid, "raw");
           // dynamic outputresp = JsonConvert.DeserializeObject(getjoboutput);
        //    var outputres = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(getjoboutput);
           
        }
    }
}
