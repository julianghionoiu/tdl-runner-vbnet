Imports BeFaster.App.Solutions
Imports BeFaster.Runner
Imports BeFaster.Runner.Extensions

'
' ~~~~~~~~~~ Running the system: ~~~~~~~~~~~~~
'
'   From IDE:
'      Configure the "BeFaster.App" solution to Run on External Console then run.
'
'   From command line:
'      msbuild befaster.sln; src\BeFaster.App\bin\Debug\BeFaster.App.exe
'
'   To run your unit tests locally:
'      Run the "BeFaster.App.Tests - Unit Tests" configuration.
'
' ~~~~~~~~~~ The workflow ~~~~~~~~~~~~~
'
'   By running this file you interact with a challenge server.
'   The interaction follows a request-response pattern:
'        * You are presented with your current progress and a list of actions.
'        * You trigger one of the actions by typing it on the console.
'        * After the action feedback is presented, the execution will stop.
'
'   +------+-------------------------------------------------------------+
'   | Step | The usual workflow                                          |
'   +------+-------------------------------------------------------------+
'   |  1.  | Run this file.                                              |
'   |  2.  | Start a challenge by typing "start".                        |
'   |  3.  | Read description from the "challenges" folder               |
'   |  4.  | Implement the required method in                            |
'   |      |   .\src\BeFaster.App\Solutions                              |
'   |  5.  | Deploy to production by typing "deploy".                    |
'   |  6.  | Observe output, check for failed requests.                  |
'   |  7.  | If passed, go to step 3.                                    |
'   +------+-------------------------------------------------------------+
'
'
Module ConnectToServer

    Sub Main(args As String())
        ClientRunner.
            ForUsername(CredentialsConfigFile.Get("tdl_username")).
            WithServerHostname(CredentialsConfigFile.Get("tdl_hostname")).
            WithActionIfNoArgs(RunnerAction.TestConnectivity).
            WithSolutionFor("sum", Function(p() As String) Sum.Sum(p(0).AsInt(), p(1).AsInt())).
            WithSolutionFor("hello", Function(p() As String) Hello.Hello(p(0))).
            WithSolutionFor("fizz_buzz", Function(p() As String) FizzBuzz.FizzBuzz(p(0).AsInt())).
            WithSolutionFor("checkout", Function(p() As String) Checkout.Checkout(p(0))).
            Start(args)
    End Sub

End Module
