using System;
using System.Runtime.InteropServices;

namespace EWMALibrary
{
    /// <summary>
    /// C# wrapper for the EWMA Go library
    /// Provides a clean, object-oriented interface for calculating Exponential Weighted Moving Averages
    /// </summary>
    public class EWMAWrapper : IDisposable
    {
        // Import the DLL functions
        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CreateEWMA(double alpha);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double UpdateEWMA(int instanceID, double value);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double GetEWMAValue(int instanceID);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ResetEWMA(int instanceID);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetEWMAAlpha(int instanceID, double alpha);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool DestroyEWMA(int instanceID);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetEWMAStateJSON(int instanceID);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool SetEWMAStateJSON(int instanceID, string jsonStr);

        [DllImport("libewma.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeString(IntPtr str);

        private int instanceID;
        private bool disposed = false;

        /// <summary>
        /// Gets the current alpha (smoothing factor) value
        /// Note: This is a placeholder - would require additional Go function to implement properly
        /// </summary>
        public double Alpha => 0.0; // Placeholder

        /// <summary>
        /// Gets whether the EWMA has been initialized with at least one value
        /// </summary>
        public bool IsInitialized => Math.Abs(GetValue()) > double.Epsilon;

        /// <summary>
        /// Creates a new EWMA instance
        /// </summary>
        /// <param name="alpha">The smoothing factor (0 < alpha <= 1)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when alpha is not in valid range</exception>
        /// <exception cref="InvalidOperationException">Thrown when failed to create EWMA instance</exception>
        public EWMAWrapper(double alpha)
        {
            if (alpha <= 0 || alpha > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be between 0 and 1 (exclusive of 0, inclusive of 1)");
            }
            
            instanceID = CreateEWMA(alpha);
            if (instanceID == 0)
            {
                throw new InvalidOperationException("Failed to create EWMA instance");
            }
        }

        /// <summary>
        /// Updates the EWMA with a new value
        /// </summary>
        /// <param name="value">The new observation</param>
        /// <returns>The updated EWMA value</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public double Update(double value)
        {
            CheckDisposed();
            return UpdateEWMA(instanceID, value);
        }

        /// <summary>
        /// Gets the current EWMA value without updating it
        /// </summary>
        /// <returns>The current EWMA value</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public double GetValue()
        {
            CheckDisposed();
            return GetEWMAValue(instanceID);
        }

        /// <summary>
        /// Resets the EWMA to its initial state (uninitialized)
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public void Reset()
        {
            CheckDisposed();
            ResetEWMA(instanceID);
        }

        /// <summary>
        /// Sets a new alpha (smoothing factor) value
        /// </summary>
        /// <param name="alpha">The new smoothing factor (0 < alpha <= 1)</param>
        /// <returns>True if successful, false if alpha is invalid</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public bool SetAlpha(double alpha)
        {
            CheckDisposed();
            return SetEWMAAlpha(instanceID, alpha);
        }

        /// <summary>
        /// Gets the EWMA state as a JSON string
        /// This includes alpha, current value, and initialization status
        /// </summary>
        /// <returns>JSON representation of the EWMA state</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public string GetStateAsJSON()
        {
            CheckDisposed();
            IntPtr ptr = GetEWMAStateJSON(instanceID);
            try
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
            finally
            {
                FreeString(ptr);
            }
        }

        /// <summary>
        /// Sets the EWMA state from a JSON string
        /// This allows restoring a previously saved state
        /// </summary>
        /// <param name="jsonStr">JSON representation of the EWMA state</param>
        /// <returns>True if successful, false if JSON is invalid</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public bool SetStateFromJSON(string jsonStr)
        {
            CheckDisposed();
            return SetEWMAStateJSON(instanceID, jsonStr);
        }

        /// <summary>
        /// Processes a batch of values and returns all intermediate EWMA values
        /// This is more efficient than calling Update() multiple times when you need all results
        /// </summary>
        /// <param name="values">Array of values to process</param>
        /// <returns>Array of EWMA values corresponding to each input</returns>
        /// <exception cref="ArgumentNullException">Thrown when values array is null</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public double[] ProcessBatch(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
                
            CheckDisposed();
            double[] results = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                results[i] = Update(values[i]);
            }
            return results;
        }

        /// <summary>
        /// Processes a batch of values but only returns the final EWMA value
        /// This is more efficient when you only need the final result
        /// </summary>
        /// <param name="values">Array of values to process</param>
        /// <returns>The final EWMA value after processing all inputs</returns>
        /// <exception cref="ArgumentNullException">Thrown when values array is null</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public double ProcessBatchFinal(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
                
            CheckDisposed();
            double result = 0.0;
            foreach (double value in values)
            {
                result = Update(value);
            }
            return result;
        }

        /// <summary>
        /// Creates a copy of the current EWMA instance
        /// </summary>
        /// <returns>A new EWMAWrapper instance with the same state</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the object has been disposed</exception>
        public EWMAWrapper Clone()
        {
            CheckDisposed();
            string jsonState = GetStateAsJSON();
            var newEwma = new EWMAWrapper(0.1); // Temporary alpha, will be overwritten
            newEwma.SetStateFromJSON(jsonState);
            return newEwma;
        }

        /// <summary>
        /// Checks if the object has been disposed and throws an exception if it has
        /// </summary>
        private void CheckDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(EWMAWrapper));
            }
        }

        /// <summary>
        /// Disposes the EWMA instance and releases native resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected disposal method
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (instanceID != 0)
                {
                    DestroyEWMA(instanceID);
                    instanceID = 0;
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Finalizer - ensures native resources are cleaned up
        /// </summary>
        ~EWMAWrapper()
        {
            Dispose(false);
        }
    }
}
