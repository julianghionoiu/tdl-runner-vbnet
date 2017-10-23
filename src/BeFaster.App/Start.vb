Imports BeFaster.App.Solutions
Imports BeFaster.Runner
Imports BeFaster.Runner.Extensions

Module Start

    Sub Main(args As String())
        ClientRunner.
            ForUsername(CredentialsConfigFile.Get("tdl_username")).
            WithServerHostname("run.befaster.io").
            WithActionIfNoArgs(RunnerAction.TestConnectivity).
            WithSolutionFor("sum", Function(p() As String) Sum.Sum(p(0).AsInt(), p(1).AsInt())).
            WithSolutionFor("hello", Function(p() As String) Hello.Hello(p(0))).
            WithSolutionFor("fizz_buzz", Function(p() As String) FizzBuzz.FizzBuzz(p(0).AsInt())).
            WithSolutionFor("checkout", Function(p() As String) Checkout.Checkout(p(0))).
            Start(args)
    End Sub

End Module
