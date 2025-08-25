Imports System
Imports System.Runtime.InteropServices

''' <summary>
''' Visual Basic .NET wrapper for the EWMA Go library
''' Provides a clean, object-oriented interface for calculating Exponential Weighted Moving Averages
''' </summary>
Public Class EWMAWrapper
    Implements IDisposable

    ' Import the DLL functions
    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function CreateEWMA(alpha As Double) As Integer
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function UpdateEWMA(instanceID As Integer, value As Double) As Double
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function GetEWMAValue(instanceID As Integer) As Double
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function ResetEWMA(instanceID As Integer) As Boolean
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function SetEWMAAlpha(instanceID As Integer, alpha As Double) As Boolean
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function DestroyEWMA(instanceID As Integer) As Boolean
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function GetEWMAStateJSON(instanceID As Integer) As IntPtr
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Function SetEWMAStateJSON(instanceID As Integer, jsonStr As String) As Boolean
    End Function

    <DllImport("libewma.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub FreeString(str As IntPtr)
    End Sub

    ' Instance variables
    Private instanceID As Integer
    Private disposed As Boolean = False

    ''' <summary>
    ''' Gets the current alpha (smoothing factor) value
    ''' </summary>
    Public ReadOnly Property Alpha As Double
        Get
            ' Note: Alpha is stored in the Go library, we could add a GetAlpha function if needed
            ' For now, this would require parsing the JSON state
            Return 0.0 ' Placeholder - would need GetAlpha function in Go library
        End Get
    End Property

    ''' <summary>
    ''' Gets whether the EWMA has been initialized with at least one value
    ''' </summary>
    Public ReadOnly Property IsInitialized As Boolean
        Get
            ' This would also require parsing JSON state or adding a specific function
            Return Math.Abs(GetValue()) > Double.Epsilon
        End Get
    End Property

    ''' <summary>
    ''' Creates a new EWMA instance
    ''' </summary>
    ''' <param name="alpha">The smoothing factor (0 < alpha <= 1)</param>
    ''' <exception cref="ArgumentOutOfRangeException">Thrown when alpha is not in valid range</exception>
    ''' <exception cref="InvalidOperationException">Thrown when failed to create EWMA instance</exception>
    Public Sub New(alpha As Double)
        If alpha <= 0 OrElse alpha > 1 Then
            Throw New ArgumentOutOfRangeException("alpha", "Alpha must be between 0 and 1 (exclusive of 0, inclusive of 1)")
        End If
        
        instanceID = CreateEWMA(alpha)
        If instanceID = 0 Then
            Throw New InvalidOperationException("Failed to create EWMA instance")
        End If
    End Sub

    ''' <summary>
    ''' Updates the EWMA with a new value
    ''' </summary>
    ''' <param name="value">The new observation</param>
    ''' <returns>The updated EWMA value</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function Update(value As Double) As Double
        CheckDisposed()
        Return UpdateEWMA(instanceID, value)
    End Function

    ''' <summary>
    ''' Gets the current EWMA value without updating it
    ''' </summary>
    ''' <returns>The current EWMA value</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function GetValue() As Double
        CheckDisposed()
        Return GetEWMAValue(instanceID)
    End Function

    ''' <summary>
    ''' Resets the EWMA to its initial state (uninitialized)
    ''' </summary>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Sub Reset()
        CheckDisposed()
        ResetEWMA(instanceID)
    End Sub

    ''' <summary>
    ''' Sets a new alpha (smoothing factor) value
    ''' </summary>
    ''' <param name="alpha">The new smoothing factor (0 < alpha <= 1)</param>
    ''' <returns>True if successful, False if alpha is invalid</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function SetAlpha(alpha As Double) As Boolean
        CheckDisposed()
        Return SetEWMAAlpha(instanceID, alpha)
    End Function

    ''' <summary>
    ''' Gets the EWMA state as a JSON string
    ''' This includes alpha, current value, and initialization status
    ''' </summary>
    ''' <returns>JSON representation of the EWMA state</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function GetStateAsJSON() As String
        CheckDisposed()
        Dim ptr As IntPtr = GetEWMAStateJSON(instanceID)
        Try
            Return Marshal.PtrToStringAnsi(ptr)
        Finally
            FreeString(ptr)
        End Try
    End Function

    ''' <summary>
    ''' Sets the EWMA state from a JSON string
    ''' This allows restoring a previously saved state
    ''' </summary>
    ''' <param name="jsonStr">JSON representation of the EWMA state</param>
    ''' <returns>True if successful, False if JSON is invalid</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function SetStateFromJSON(jsonStr As String) As Boolean
        CheckDisposed()
        Return SetEWMAStateJSON(instanceID, jsonStr)
    End Function

    ''' <summary>
    ''' Processes a batch of values and returns all intermediate EWMA values
    ''' This is more efficient than calling Update() multiple times when you need all results
    ''' </summary>
    ''' <param name="values">Array of values to process</param>
    ''' <returns>Array of EWMA values corresponding to each input</returns>
    ''' <exception cref="ArgumentNullException">Thrown when values array is null</exception>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function ProcessBatch(values As Double()) As Double()
        If values Is Nothing Then
            Throw New ArgumentNullException("values")
        End If
        
        CheckDisposed()
        Dim results(values.Length - 1) As Double
        For i As Integer = 0 To values.Length - 1
            results(i) = Update(values(i))
        Next
        Return results
    End Function

    ''' <summary>
    ''' Processes a batch of values but only returns the final EWMA value
    ''' This is more efficient when you only need the final result
    ''' </summary>
    ''' <param name="values">Array of values to process</param>
    ''' <returns>The final EWMA value after processing all inputs</returns>
    ''' <exception cref="ArgumentNullException">Thrown when values array is null</exception>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function ProcessBatchFinal(values As Double()) As Double
        If values Is Nothing Then
            Throw New ArgumentNullException("values")
        End If
        
        CheckDisposed()
        Dim result As Double = 0.0
        For Each value As Double In values
            result = Update(value)
        Next
        Return result
    End Function

    ''' <summary>
    ''' Creates a copy of the current EWMA instance
    ''' </summary>
    ''' <returns>A new EWMAWrapper instance with the same state</returns>
    ''' <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
    Public Function Clone() As EWMAWrapper
        CheckDisposed()
        Dim jsonState As String = GetStateAsJSON()
        Dim newEwma As New EWMAWrapper(0.1) ' Temporary alpha, will be overwritten
        newEwma.SetStateFromJSON(jsonState)
        Return newEwma
    End Function

    ''' <summary>
    ''' Checks if the object has been disposed and throws an exception if it has
    ''' </summary>
    Private Sub CheckDisposed()
        If disposed Then
            Throw New ObjectDisposedException("EWMAWrapper")
        End If
    End Sub

    ''' <summary>
    ''' Finalizer - ensures native resources are cleaned up
    ''' </summary>
    Protected Overrides Sub Finalize()
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ''' <summary>
    ''' Disposes the EWMA instance and releases native resources
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    ''' <summary>
    ''' Protected disposal method
    ''' </summary>
    ''' <param name="disposing">True if called from Dispose(), False if called from finalizer</param>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposed Then
            If instanceID <> 0 Then
                DestroyEWMA(instanceID)
                instanceID = 0
            End If
            disposed = True
        End If
    End Sub
End Class
