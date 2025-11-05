Imports BeFaster.App
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
Namespace BeFaster.App
    Module SendCommandToServer

        Sub Main(args As String())

            Dim entryPointMapping As New EntryPointMapping()

            Dim runner As IImplementationRunner =
                New QueueBasedImplementationRunner.Builder().
                    SetConfig(Utils.GetRunnerConfig()).
                    WithSolutionFor("sum", AddressOf entryPointMapping.Sum).
                    WithSolutionFor("hello", AddressOf entryPointMapping.Hello).
                    WithSolutionFor("fizz_buzz", AddressOf entryPointMapping.FizzBuzz).
                    WithSolutionFor("checkout", AddressOf entryPointMapping.Checkout).
                    WithSolutionFor("rabbit_hole", AddressOf entryPointMapping.RabbitHole).
                    WithSolutionFor("render_house", AddressOf entryPointMapping.RenderHouse).
                    WithSolutionFor("amazing_maze", AddressOf entryPointMapping.AmazingMaze).
                    WithSolutionFor("ultimate_maze", AddressOf entryPointMapping.UltimateMaze).
                    WithSolutionFor("increment", AddressOf entryPointMapping.Increment).
                    WithSolutionFor("to_uppercase", AddressOf entryPointMapping.ToUppercase).
                    WithSolutionFor("letter_to_santa", AddressOf entryPointMapping.LetterToSanta).
                    WithSolutionFor("count_lines", AddressOf entryPointMapping.CountLines).
                    WithSolutionFor("array_sum", AddressOf entryPointMapping.ArraySum).
                    WithSolutionFor("int_range", AddressOf entryPointMapping.IntRange).
                    WithSolutionFor("filter_pass", AddressOf entryPointMapping.FilterPass).
                    WithSolutionFor("inventory_add", AddressOf entryPointMapping.InventoryAdd).
                    WithSolutionFor("inventory_size", AddressOf entryPointMapping.InventorySize).
                    WithSolutionFor("inventory_get", AddressOf entryPointMapping.InventoryGet).
                    WithSolutionFor("waves", AddressOf entryPointMapping.Waves).
                    Create()

            ChallengeSession.
                ForRunner(runner).
                WithConfig(Utils.GetConfig()).
                WithActionProvider(New UserInputAction(args)).
                Start()

            If Not Console.IsInputRedirected Then
                Console.WriteLine("Press any key to exit...")
                Console.ReadKey()
            End If
        End Sub

    End Module
End Namespace