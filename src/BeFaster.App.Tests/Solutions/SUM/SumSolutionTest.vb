Imports BeFaster.App.Solutions.SUM
Imports NUnit.Framework

<TestFixture>
Public Class SumSolutionTest

    <TestCase(1, 1, ExpectedResult := 2)>
    Public Function ComputeSum(x As Int32, y As Int32)
        Return Sum.Sum(x, y)
    End Function

End Class
