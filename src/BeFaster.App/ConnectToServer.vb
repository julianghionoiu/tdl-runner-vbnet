Imports BeFaster.App.Solutions
Imports BeFaster.Runner
Imports BeFaster.Runner.Extensions

'~~~~~~~~~~ Running the system: ~~~~~~~~~~~~~

'  From IDE, run without args:
'     Set the value of the `WithActionIfNoArgs` below
'     Run this project from the IDE.
'
'  From IDE, run with args:
'     Create a separate Run configuration
'     Add the name of the action as an argument to the command-line 
'     Run the newly created configuration from the IDE.
'
'  Available actions:
'       * getNewRoundDescription    - Get the round description (call once per round).
'       * testConnectivity          - Test you can connect to the server (call any number of time)
'       * deployToProduction        - Release your code. Real requests will be used to test your solution.
'                                     If your solution is wrong you get a penalty of 10 minutes.
'                                     After you fix the problem, you should deploy a new version into production.
'
'  ~~~~~~~~~~ The workflow ~~~~~~~~~~~~~
'
'  +------+-----------------------------------------+-----------------------------------------------+
'  | Step |          IDE                            |         Web console                           |
'  +------+-----------------------------------------+-----------------------------------------------+
'  |  1.  |                                         | Start a challenge, should display "Started"   |
'  |  2.  | Run "getNewRoundDescription"            |                                               |
'  |  3.  | Read description from ./challenges      |                                               |
'  |  4.  | Implement the required method in        |                                               |
'  |      |   ./src/BeFaster.App/Solutions          |                                               |
'  |  5.  | Run "testConnectivity", observe output  |                                               |
'  |  6.  | If ready, run "deployToProduction"      |                                               |
'  |  7.  |                                         | Type "done"                                   |
'  |  8.  |                                         | Check failed requests                         |
'  |  9.  |                                         | Go to step 2.                                 |
'  +------+-----------------------------------------+-----------------------------------------------+
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
