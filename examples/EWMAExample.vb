Imports System
Imports System.Linq

''' <summary>
''' Demonstration program showing how to use the EWMA library with VB.NET
''' This example covers all major features of the EWMAWrapper class
''' </summary>
Module EWMAExample
    Sub Main()
        Console.WriteLine("EWMA Library Demonstration (VB.NET)")
        Console.WriteLine("====================================")
        Console.WriteLine()

        Try
            ' Basic Usage Demo
            DemoBasicUsage()
            Console.WriteLine()

            ' Batch Processing Demo
            DemoBatchProcessing()
            Console.WriteLine()

            ' State Management Demo
            DemoStateManagement()
            Console.WriteLine()

            ' Alpha Parameter Demo
            DemoAlphaChanges()
            Console.WriteLine()

            ' Performance Demo
            DemoPerformance()

        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
            Console.WriteLine($"Details: {ex}")
        End Try

        Console.WriteLine()
        Console.WriteLine("Demo completed. Press any key to exit...")
        Console.ReadKey()
    End Sub

    ''' <summary>
    ''' Demonstrates basic EWMA usage with individual value updates
    ''' </summary>
    Private Sub DemoBasicUsage()
        Console.WriteLine("1. Basic Usage Demo")
        Console.WriteLine("------------------")

        Using ewma As New EWMAWrapper(0.3)
            Console.WriteLine("Created EWMA with alpha = 0.3")
            Console.WriteLine()

            ' Test individual updates
            Dim testValues As Double() = {10.0, 20.0, 15.0, 25.0, 30.0, 18.0, 22.0}
            
            Console.WriteLine("Processing individual values:")
            Console.WriteLine("Input Value" & vbTab & "EWMA Result" & vbTab & "Description")
            Console.WriteLine("-----------" & vbTab & "-----------" & vbTab & "-----------")

            For i As Integer = 0 To testValues.Length - 1
                Dim result As Double = ewma.Update(testValues(i))
                Dim description As String = If(i = 0, "(Initial value)", $"(α×{testValues(i):F1} + (1-α)×previous)")
                Console.WriteLine($"{testValues(i):F1}" & vbTab & vbTab & $"{result:F3}" & vbTab & vbTab & description)
            Next

            Console.WriteLine()
            Console.WriteLine($"Final EWMA value: {ewma.GetValue():F3}")
        End Using
    End Sub

    ''' <summary>
    ''' Demonstrates batch processing capabilities
    ''' </summary>
    Private Sub DemoBatchProcessing()
        Console.WriteLine("2. Batch Processing Demo")
        Console.WriteLine("-----------------------")

        Using ewma As New EWMAWrapper(0.5)
            Console.WriteLine("Created EWMA with alpha = 0.5 for batch processing")
            Console.WriteLine()

            ' Batch processing with all results
            Dim batchValues As Double() = {5.0, 10.0, 8.0, 12.0, 15.0, 9.0, 14.0}
            Console.WriteLine("Processing batch (getting all intermediate results):")
            
            Dim batchResults As Double() = ewma.ProcessBatch(batchValues)
            
            Console.WriteLine("Input" & vbTab & "EWMA" & vbTab & "Running Average")
            Console.WriteLine("-----" & vbTab & "----" & vbTab & "---------------")
            For i As Integer = 0 To batchValues.Length - 1
                Console.WriteLine($"{batchValues(i):F1}" & vbTab & $"{batchResults(i):F3}" & vbTab & $"{batchValues.Take(i + 1).Average():F3}")
            Next

            ' Reset and demo final-only processing
            ewma.Reset()
            Console.WriteLine()
            Console.WriteLine("Processing same batch (getting only final result):")
            Dim finalResult As Double = ewma.ProcessBatchFinal(batchValues)
            Console.WriteLine($"Final EWMA result: {finalResult:F3}")
        End Using
    End Sub

    ''' <summary>
    ''' Demonstrates state management (save/restore) functionality
    ''' </summary>
    Private Sub DemoStateManagement()
        Console.WriteLine("3. State Management Demo")
        Console.WriteLine("-----------------------")

        Dim savedState As String

        ' Create first EWMA and process some data
        Using ewma1 As New EWMAWrapper(0.2)
            Console.WriteLine("Original EWMA (alpha = 0.2):")
            
            Dim values As Double() = {100.0, 110.0, 95.0, 105.0, 120.0}
            For Each value In values
                Dim result As Double = ewma1.Update(value)
                Console.WriteLine($"  Update({value:F1}) = {result:F3}")
            Next

            ' Save state
            savedState = ewma1.GetStateAsJSON()
            Console.WriteLine()
            Console.WriteLine($"Saved state: {savedState}")
        End Using

        ' Create new EWMA and restore state
        Console.WriteLine()
        Console.WriteLine("Creating new EWMA and restoring state:")
        Using ewma2 As New EWMAWrapper(0.1) ' Different alpha initially
            Console.WriteLine($"Before restore - Current value: {ewma2.GetValue():F3}")
            
            If ewma2.SetStateFromJSON(savedState) Then
                Console.WriteLine("State restored successfully!")
                Console.WriteLine($"After restore - Current value: {ewma2.GetValue():F3}")
                
                ' Continue processing with restored state
                Dim newResult As Double = ewma2.Update(130.0)
                Console.WriteLine($"Continued processing - Update(130.0) = {newResult:F3}")
            Else
                Console.WriteLine("Failed to restore state!")
            End If
        End Using

        ' Demonstrate cloning
        Console.WriteLine()
        Console.WriteLine("Demonstrating EWMA cloning:")
        Using original As New EWMAWrapper(0.4)
            original.Update(50.0)
            original.Update(60.0)
            
            Using clone As EWMAWrapper = original.Clone()
                Console.WriteLine($"Original value: {original.GetValue():F3}")
                Console.WriteLine($"Clone value: {clone.GetValue():F3}")
                
                ' Update each independently
                original.Update(70.0)
                clone.Update(40.0)
                
                Console.WriteLine($"After different updates:")
                Console.WriteLine($"  Original: {original.GetValue():F3}")
                Console.WriteLine($"  Clone: {clone.GetValue():F3}")
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Demonstrates changing alpha parameter during operation
    ''' </summary>
    Private Sub DemoAlphaChanges()
        Console.WriteLine("4. Alpha Parameter Demo")
        Console.WriteLine("----------------------")

        Using ewma As New EWMAWrapper(0.1)
            Console.WriteLine("Starting with alpha = 0.1 (heavy smoothing)")
            Console.WriteLine()

            ' Process with low alpha (heavy smoothing)
            Dim baseValue As Double = 100.0
            For i As Integer = 1 To 3
                Dim result As Double = ewma.Update(baseValue + i * 10)
                Console.WriteLine($"  Update({baseValue + i * 10:F1}) = {result:F3}")
            Next

            Console.WriteLine($"Current value with alpha=0.1: {ewma.GetValue():F3}")
            Console.WriteLine()

            ' Change to higher alpha (less smoothing)
            If ewma.SetAlpha(0.8) Then
                Console.WriteLine("Changed alpha to 0.8 (light smoothing)")
                
                For i As Integer = 4 To 6
                    Dim result As Double = ewma.Update(baseValue + i * 10)
                    Console.WriteLine($"  Update({baseValue + i * 10:F1}) = {result:F3}")
                Next

                Console.WriteLine($"Final value with alpha=0.8: {ewma.GetValue():F3}")
            Else
                Console.WriteLine("Failed to change alpha!")
            End If

            ' Try invalid alpha
            Console.WriteLine()
            Console.WriteLine("Testing invalid alpha values:")
            Console.WriteLine($"  SetAlpha(-0.1): {ewma.SetAlpha(-0.1)}")
            Console.WriteLine($"  SetAlpha(1.5): {ewma.SetAlpha(1.5)}")
            Console.WriteLine($"  SetAlpha(0.5): {ewma.SetAlpha(0.5)}")
        End Using
    End Sub

    ''' <summary>
    ''' Demonstrates performance characteristics
    ''' </summary>
    Private Sub DemoPerformance()
        Console.WriteLine("5. Performance Demo")
        Console.WriteLine("------------------")

        Using ewma As New EWMAWrapper(0.3)
            Const iterations As Integer = 100000
            Dim random As New Random(42) ' Fixed seed for reproducible results

            Console.WriteLine($"Processing {iterations:N0} random values...")
            
            Dim startTime As DateTime = DateTime.Now
            
            For i As Integer = 1 To iterations
                Dim randomValue As Double = random.NextDouble() * 100
                ewma.Update(randomValue)
            Next
            
            Dim endTime As DateTime = DateTime.Now
            Dim elapsed As TimeSpan = endTime - startTime
            
            Console.WriteLine($"Time taken: {elapsed.TotalMilliseconds:F2} ms")
            Console.WriteLine($"Updates per second: {iterations / elapsed.TotalSeconds:N0}")
            Console.WriteLine($"Microseconds per update: {elapsed.TotalMilliseconds * 1000 / iterations:F2}")
            Console.WriteLine($"Final EWMA value: {ewma.GetValue():F6}")
            
            ' Demonstrate large batch processing
            Console.WriteLine()
            Console.WriteLine("Testing large batch processing...")
            ewma.Reset()
            
            Dim largeBatch(9999) As Double
            For i As Integer = 0 To largeBatch.Length - 1
                largeBatch(i) = Math.Sin(i * 0.1) * 50 + 100 ' Sine wave data
            Next
            
            startTime = DateTime.Now
            Dim batchResult As Double = ewma.ProcessBatchFinal(largeBatch)
            endTime = DateTime.Now
            elapsed = endTime - startTime
            
            Console.WriteLine($"Processed {largeBatch.Length:N0} values in batch")
            Console.WriteLine($"Time taken: {elapsed.TotalMilliseconds:F2} ms")
            Console.WriteLine($"Final result: {batchResult:F6}")
        End Using
    End Sub
End Module
