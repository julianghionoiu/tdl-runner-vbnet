open BeFaster.Runner
open BeFaster.Runner.Extensions
open BeFaster.App.Solutions

/// ~~~~~~~~~~ Running the system: ~~~~~~~~~~~~~
/// 
///   From IDE, run without args:
///      Set the value of the `WithActionIfNoArgs` below
///      Run this project from the IDE.
/// 
///   From IDE, run with args:
///      Create a separate Run configuration
///      Add the name of the action as an argument to the command-line 
///      Run the newly created configuration from the IDE.
///
///   Available actions:
///        * getNewRoundDescription    - Get the round description (call once per round).
///        * testConnectivity          - Test you can connect to the server (call any number of time)
///        * deployToProduction        - Release your code. Real requests will be used to test your solution.
///                                      If your solution is wrong you get a penalty of 10 minutes.
///                                      After you fix the problem, you should deploy a new version into production.
///
/// ~~~~~~~~~~ The workflow ~~~~~~~~~~~~~
///
///   +------+-----------------------------------------+-----------------------------------------------+
///   | Step |          IDE                            |         Web console                           |
///   +------+-----------------------------------------+-----------------------------------------------+
///   |  1.  |                                         | Start a challenge, should display "Started"   |
///   |  2.  | Run "getNewRoundDescription"            |                                               |
///   |  3.  | Read description from ./challenges      |                                               |
///   |  4.  | Implement the required method in        |                                               |
///   |      |   ./src/BeFaster.App/Solutions          |                                               |
///   |  5.  | Run "testConnectivity", observe output  |                                               |
///   |  6.  | If ready, run "deployToProduction"      |                                               |
///   |  7.  |                                         | Type "done"                                   |
///   |  8.  |                                         | Check failed requests                         |
///   |  9.  |                                         | Go to step 2.                                 |
///   +------+-----------------------------------------+-----------------------------------------------+
[<EntryPoint>]
let main argv = 
    ClientRunner
        .ForUsername(CredentialsConfigFile.Get("tdl_username"))
        .WithServerHostname("run.befaster.io")
        .WithActionIfNoArgs(RunnerAction.TestConnectivity)
        .WithSolutionFor("sum", fun p -> Sum.sum(p.[0].AsInt(), p.[1].AsInt()) :> obj)
        .WithSolutionFor("hello", fun p -> Hello.hello(p.[0]) :> obj)
        .WithSolutionFor("fizz_buzz", fun p -> FizzBuzz.fizzBuzz(p.[0].AsInt()) :> obj)
        .WithSolutionFor("checkout", fun p -> Checkout.checkout(p.[0]) :> obj)
        .Start(argv)
    0 // return an integer exit code
