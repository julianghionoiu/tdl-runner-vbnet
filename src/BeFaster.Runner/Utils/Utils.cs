using TDL.Client.Queue;
using TDL.Client.Runner;

namespace BeFaster.Runner.Utils
{
    public static class Utils
    {
        public static ChallengeSessionConfig GetConfig() =>
            ChallengeSessionConfig
                .ForJourneyId(CredentialsConfigFile.Get("tdl_journey_id"))
                .WithServerHostname(CredentialsConfigFile.Get("tdl_hostname"))
                .WithColours(CredentialsConfigFile.Get("tdl_use_coloured_output", true))
                .WithRecordingSystemShouldBeOn(CredentialsConfigFile.Get("tdl_require_rec", true));

        public static ImplementationRunnerConfig GetRunnerConfig() =>
            new ImplementationRunnerConfig()
                .SetRequestQueueName(CredentialsConfigFile.Get("tdl_request_queue_name"))
                .SetResponseQueueName(CredentialsConfigFile.Get("tdl_response_queue_name"))
                .SetHostname(CredentialsConfigFile.Get("tdl_hostname"));
    }
}
