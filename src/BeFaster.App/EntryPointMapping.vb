Imports BeFaster.App.Solutions.AMZ
Imports BeFaster.App.Solutions.CHK
Imports BeFaster.App.Solutions.DMO
Imports BeFaster.App.Solutions.FIZ
Imports BeFaster.App.Solutions.HLO
Imports BeFaster.App.Solutions.RBT
Imports BeFaster.App.Solutions.HOC
Imports BeFaster.App.Solutions.SUM
Imports BeFaster.App.Solutions.ULT

Imports System.Collections.Generic
Imports TDL.Client.Queue.Abstractions

Namespace BeFaster.App
    ''' <summary>
    ''' Maps RPC events to instance method calls with correctly typed parameters.
    ''' </summary>
    Class EntryPointMapping
        Private ReadOnly sumSolution As SumSolution
        Private ReadOnly helloSolution As HelloSolution
        Private ReadOnly fizzBuzzSolution As FizzBuzzSolution
        Private ReadOnly checkoutSolution As CheckoutSolution
        Private ReadOnly rabbitHoleSolution As RabbitHoleSolution
        Private ReadOnly houseOfCardsSolution As HouseOfCardsSolution
        Private ReadOnly amazingSolution As AmazingSolution
        Private ReadOnly ultimateSolution As UltimateSolution
        Private ReadOnly demoRound1Solution As DemoRound1Solution
        Private ReadOnly demoRound2Solution As DemoRound2Solution
        Private ReadOnly demoRound3Solution As DemoRound3Solution
        Private ReadOnly demoRound4n5Solution As DemoRound4n5Solution

        Public Sub New()
            sumSolution = New SumSolution()
            helloSolution = New HelloSolution()
            fizzBuzzSolution = New FizzBuzzSolution()
            checkoutSolution = New CheckoutSolution()
            rabbitHoleSolution = New RabbitHoleSolution()
            houseOfCardsSolution = New HouseOfCardsSolution()
            amazingSolution = New AmazingSolution()
            ultimateSolution = New UltimateSolution()
            demoRound1Solution = New DemoRound1Solution()
            demoRound2Solution = New DemoRound2Solution()
            demoRound3Solution = New DemoRound3Solution()
            demoRound4n5Solution = New DemoRound4n5Solution()
        End Sub

        Public Function Sum(p As List(Of ParamAccessor)) As Object
            Return sumSolution.Sum(p(0).GetAsInteger(), p(1).GetAsInteger())
        End Function

        Public Function Hello(p As List(Of ParamAccessor)) As Object
            Return helloSolution.Hello(p(0).GetAsString())
        End Function

        Public Function FizzBuzz(p As List(Of ParamAccessor)) As Object
            Return fizzBuzzSolution.FizzBuzz(p(0).GetAsInteger())
        End Function

        Public Function Checkout(p As List(Of ParamAccessor)) As Object
            Return checkoutSolution.Checkout(p(0).GetAsString())
        End Function

        Public Function RabbitHole(p As List(Of ParamAccessor)) As Object
            Return rabbitHoleSolution.RabbitHole(
                p(0).GetAsInteger(),
                p(1).GetAsInteger(),
                p(2).GetAsString(),
                p(3).GetAsMapOf(Of String)()
            )
        End Function

        Public Function RenderHouse(p As List(Of ParamAccessor)) As Object
            Return houseOfCardsSolution.RenderHouse(
                p(0).GetAsString(),
                p(1).GetAsMapOf(Of String)()
            )
        End Function

        Public Function AmazingMaze(p As List(Of ParamAccessor)) As Object
            Return amazingSolution.AmazingMaze(
                p(0).GetAsInteger(),
                p(1).GetAsInteger(),
                p(2).GetAsMapOf(Of String)()
            )
        End Function

        Public Function UltimateMaze(p As List(Of ParamAccessor)) As Object
            Return ultimateSolution.UltimateMaze(
                p(0).GetAsInteger(),
                p(1).GetAsInteger(),
                p(2).GetAsMapOf(Of String)()
            )
        End Function

        ' Demo Round 1

        Public Function Increment(p As List(Of ParamAccessor)) As Object
            Return demoRound1Solution.Increment(p(0).GetAsInteger())
        End Function

        Public Function ToUppercase(p As List(Of ParamAccessor)) As Object
            Return demoRound1Solution.ToUppercase(p(0).GetAsString())
        End Function

        Public Function LetterToSanta(p As List(Of ParamAccessor)) As Object
            Return demoRound1Solution.LetterToSanta()
        End Function

        Public Function CountLines(p As List(Of ParamAccessor)) As Object
            Return demoRound1Solution.CountLines(p(0).GetAsString())
        End Function

        ' Demo Round 2

        Public Function ArraySum(p As List(Of ParamAccessor)) As Object
            Return demoRound2Solution.ArraySum(p(0).GetAsListOf(Of Integer)())
        End Function

        Public Function IntRange(p As List(Of ParamAccessor)) As Object
            Return demoRound2Solution.IntRange(p(0).GetAsInteger(), p(1).GetAsInteger())
        End Function

        Public Function FilterPass(p As List(Of ParamAccessor)) As Object
            Return demoRound2Solution.FilterPass(p(0).GetAsListOf(Of Integer)(), p(1).GetAsInteger())
        End Function

        ' Demo Round 3

        Public Function InventoryAdd(p As List(Of ParamAccessor)) As Object
            Return demoRound3Solution.InventoryAdd(p(0).GetAsObject(Of InventoryItem)(), p(1).GetAsInteger())
        End Function

        Public Function InventorySize(p As List(Of ParamAccessor)) As Object
            Return demoRound3Solution.InventorySize()
        End Function

        Public Function InventoryGet(p As List(Of ParamAccessor)) As Object
            Return demoRound3Solution.InventoryGet(p(0).GetAsString())
        End Function

        ' Demo Round 4 and 5

        Public Function Waves(p As List(Of ParamAccessor)) As Object
            Return demoRound4n5Solution.Waves(p(0).GetAsInteger())
        End Function
    End Class
End Namespace
