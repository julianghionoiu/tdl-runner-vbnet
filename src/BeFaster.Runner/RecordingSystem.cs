using System;
using System.Net;
using BeFaster.Runner.Exceptions;
using RestSharp;

namespace BeFaster.Runner
{
    internal static class RecordingSystem
    {
        private const string RecordingSystemEndpoint = "http://localhost:41375";

        private static readonly IRestClient RestClient = new RestClient(RecordingSystemEndpoint);

        public static bool IsRecordingSystemOk()
        {
            // ReSharper disable once SimplifyConditionalTernaryExpression
            return RecordingSystem.IsRecordingRequired() ? RecordingSystem.IsRunning() : true;
        }

        public static bool IsRunning()
        {
            try
            {
                var request = new RestRequest("status", Method.GET);
                var response = RestClient.Execute(request);

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                throw new RecordingSystemNotReachable(e);
            }
        }

        private static bool IsRecordingRequired()
        {
            return bool.Parse(CredentialsConfigFile.Get("tdl_require_rec", "true"));
        }

        public static void NotifyEvent(string lastFetchedRound, string actionName)
        {
            Console.WriteLine($@"Notify round ""{lastFetchedRound}"", event ""{actionName}""");

            if (!IsRecordingRequired()) {
                return;
            }
            
            try
            {
                var request = new RestRequest("notify", Method.POST);
                request.AddParameter("text/plain", $"{lastFetchedRound}/{actionName}", ParameterType.RequestBody);
                var response = RestClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Recording system returned code: {response.StatusCode}");
                }
                else if (!response.Content.StartsWith("ACK"))
                {
                    Console.WriteLine($"Recording system returned body: {response.Content}");
                }
            }
            catch (Exception e)
            {
                throw new RecordingSystemNotReachable(e);
            }
        }
    }
}
