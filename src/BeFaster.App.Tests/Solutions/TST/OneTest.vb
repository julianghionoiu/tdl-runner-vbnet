Imports BeFaster.App.Solutions.TST
Imports NUnit.Framework

<TestFixture>
Public Class OneTest

    <TestCase(ExpectedResult := 1)>
    Public Function OneTest()
        Return One.apply()
    End Function

End Class
