Imports BeFaster.App.Solutions.SUM
Imports BeFaster.App.Solutions.CHK
Imports BeFaster.App.Solutions.CHL
Imports BeFaster.App.Solutions.HLO
Imports BeFaster.App.Solutions.FIZ
Imports BeFaster.App.Solutions.ARRS
Imports BeFaster.App.Solutions.IRNG
Imports BeFaster.Runner
Imports BeFaster.Runner.Utils
Imports TDL.Client
Imports TDL.Client.Queue
Imports TDL.Client.Runner
Imports Newtonsoft.Json.Linq


'
' ~~~~~~~~~~ Running the system: ~~~~~~~~~~~~~
'
'   From IDE:
'      Configure the "BeFaster.App" solution to Run on External Console then run.
'
'   From command line:
'      dotnet run --project src\BeFaster.App
'
'   To run your unit tests locally:
'      Run the "BeFaster.App.Tests" project.
'        or
'      dotnet test
'
' ~~~~~~~~~~ The workflow ~~~~~~~~~~~~~
'
'   By running this file you interact with a challenge server.
'   The interaction follows a request-response pattern:
'        * You are presented with your current progress and a list of actions.
'        * You trigger one of the actions by typing it on the console.
'        * After the action feedback is presented, the execution will stop.
'
'   +------+-----------------------------------------------------------------------+
'   | Step | The usual workflow                                                    |
'   +------+-----------------------------------------------------------------------+
'   |  1.  | Run this file.                                                        |
'   |  2.  | Start a challenge by typing "start".                                  |
'   |  3.  | Read the description from the "challenges" folder.                    |
'   |  4.  | Locate the file corresponding to your current challenge in:           |
'   |      |   .\src\BeFaster.App\Solutions                                        |
'   |  5.  | Replace the following placeholder exception with your solution:       |
'   |      |   Throw New NotImplementedException()                                 |
'   |  6.  | Deploy to production by typing "deploy".                              |
'   |  7.  | Observe the output, check for failed requests.                        |
'   |  8.  | If passed, go to step 1.                                              |
'   +------+-----------------------------------------------------------------------+
'
'   You are encouraged to change this project as you please:
'        * You can use your preferred libraries.
'        * You can use your own test framework.
'        * You can change the file structure.
'        * Anything really, provided that this file stays runnable.
'
'
Module SendCommandToServer

    Sub Main(args As String())

        Dim runner As IImplementationRunner =
            New QueueBasedImplementationRunner.Builder().
                SetConfig(Utils.GetRunnerConfig()).
                WithSolutionFor("sum", Function(p As List(of JToken)) Sum.Sum(p(0).ToObject(of Integer)(), p(1).ToObject(of Integer)())).
                WithSolutionFor("hello", Function(p As List(of JToken)) Hello.Hello(p(0).ToObject(of String)())).
                WithSolutionFor("array_sum", Function(p As List(of JToken)) ArraySum.Compute(p(0).ToObject(of List(of Integer))())).
                WithSolutionFor("int_range", Function(p As List(of JToken)) IntRange.Generate(p(0).ToObject(of Integer)(), p(1).ToObject(of Integer)())).
                WithSolutionFor("fizz_buzz", Function(p As List(of JToken)) FizzBuzz.FizzBuzz(p(0).ToObject(of Integer)())).
                WithSolutionFor("checkout", Function(p As List(of JToken)) Checkout.ComputePrice(p(0).ToObject(of String)())).
                WithSolutionFor("checklite", Function(p As List(of JToken)) Checklite.ComputePrice(p(0).ToObject(of String)())).
                Create()

        ChallengeSession.
            ForRunner(runner).
            WithConfig(Utils.GetConfig()).
            WithActionProvider(New UserInputAction(args)).
            Start()

        Console.Write("Press any key to continue . . . ")
        Console.ReadKey()
    End Sub

End Module
