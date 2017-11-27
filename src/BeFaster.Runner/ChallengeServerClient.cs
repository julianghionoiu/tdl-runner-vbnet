using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using BeFaster.Runner.Exceptions;
using RestSharp;
using RestSharp.Extensions.MonoHttp;

namespace BeFaster.Runner
{
    public class ChallengeServerClient
    {
        private readonly string journeyId;
        private readonly string acceptHeader;
        private readonly RestClient restClient;

        public ChallengeServerClient(string hostname, string journeyId, bool useColours)
        {
            this.journeyId = journeyId;
            this.acceptHeader = useColours ? "text/coloured" : "text/not-coloured";
            this.restClient = new RestClient($"http://{hostname}:8222");
        }

        
        //~~~~~~~ GET ~~~~~~~~
        
        public string GetJourneyProgress()
        {
            return Get("journeyProgress");
        }
        
        public string GetAvailableActions()
        {
            return Get("availableActions");
        }

        public string GetRoundDescription()
        {
            return Get("roundDescription");
        }
        
        private string Get(string name)
        {
            var encodedPath = HttpUtility.UrlEncode(this.journeyId, Encoding.UTF8);
            var request = new RestRequest($"{name}/{encodedPath}", Method.GET);
            
            request.AddHeader("Accept", acceptHeader);
            request.AddHeader("Accept-Charset", "UTF-8");

            var response = restClient.Execute(request);
            EnsureStatusOk(response);
            return response.Content;
        }
        
        //~~~~~~~ POST ~~~~~~~~
        
        public string SendAction(string name)
        {
            var encodedPath = HttpUtility.UrlEncode(this.journeyId, Encoding.UTF8);
            var request = new RestRequest($"action/{name}/{encodedPath}", Method.POST);
            
            request.AddHeader("Accept", acceptHeader);
            request.AddHeader("Accept-Charset", "UTF-8");

            var response = restClient.Execute(request);
            EnsureStatusOk(response);
            return response.Content;
        }

        //~~~~~~~ Error handling ~~~~~~~~~
        
        private static void EnsureStatusOk(IRestResponse response)
        {
            //DEBT WE should switch to Unitrest to handle all the requests consistently accross all languages
            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new OtherCommunicationException(response.ErrorMessage);
            }
                
            var responseStatus = (int) response.StatusCode;
            if (IsClientError(responseStatus)) {
                throw new ClientErrorException(response.Content);
            } else if (IsServerError(responseStatus)) {
                throw new ServerErrorException(response.StatusDescription);
            } else if (IsOtherErrorResponse(responseStatus)) {
                throw new OtherCommunicationException(response.StatusDescription);
            }
        }

        private static bool IsClientError(int responseStatus)
        {
            return responseStatus >= 400 && responseStatus < 500;
        }

        private static bool IsServerError(int responseStatus)
        {
            return responseStatus >= 500 && responseStatus < 600;
        }

        private static bool IsOtherErrorResponse(int responseStatus)
        {
            return responseStatus < 200 || responseStatus > 300;
        }

    }

    internal class ClientErrorException : Exception
    {
        public ClientErrorException(string responseContent) 
            : base(responseContent)
        {
        }
    }

    internal class ServerErrorException : Exception
    {
        public ServerErrorException(string responseStatusDescription)           
            : base(responseStatusDescription)
        {
        }
    }

    internal class OtherCommunicationException : Exception
    {
        public OtherCommunicationException(string responseStatusDescription)
            : base(responseStatusDescription)
        {
        }
    }
}