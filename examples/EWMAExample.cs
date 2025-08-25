using System;
using System.Linq;
using EWMALibrary;

namespace EWMADemo
{
    /// <summary>
    /// Demonstration program showing how to use the EWMA library with C#
    /// This example covers all major features of the EWMAWrapper class
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EWMA Library Demonstration (C#)");
            Console.WriteLine("===============================");
            Console.WriteLine();

            try
            {
                // Basic Usage Demo
                DemoBasicUsage();
                Console.WriteLine();

                // Batch Processing Demo
                DemoBatchProcessing();
                Console.WriteLine();

                // State Management Demo
                DemoStateManagement();
                Console.WriteLine();

                // Alpha Parameter Demo
                DemoAlphaChanges();
                Console.WriteLine();

                // Performance Demo
                DemoPerformance();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }

            Console.WriteLine();
            Console.WriteLine("Demo completed. Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates basic EWMA usage with individual value updates
        /// </summary>
        private static void DemoBasicUsage()
        {
            Console.WriteLine("1. Basic Usage Demo");
            Console.WriteLine("------------------");

            using (var ewma = new EWMAWrapper(0.3))
            {
                Console.WriteLine("Created EWMA with alpha = 0.3");
                Console.WriteLine();

                // Test individual updates
                double[] testValues = { 10.0, 20.0, 15.0, 25.0, 30.0, 18.0, 22.0 };

                Console.WriteLine("Processing individual values:");
                Console.WriteLine("Input Value\tEWMA Result\tDescription");
                Console.WriteLine("-----------\t-----------\t-----------");

                for (int i = 0; i < testValues.Length; i++)
                {
                    double result = ewma.Update(testValues[i]);
                    string description = i == 0 ? "(Initial value)" : $"(α×{testValues[i]:F1} + (1-α)×previous)";
                    Console.WriteLine($"{testValues[i]:F1}\t\t{result:F3}\t\t{description}");
                }

                Console.WriteLine();
                Console.WriteLine($"Final EWMA value: {ewma.GetValue():F3}");
            }
        }

        /// <summary>
        /// Demonstrates batch processing capabilities
        /// </summary>
        private static void DemoBatchProcessing()
        {
            Console.WriteLine("2. Batch Processing Demo");
            Console.WriteLine("-----------------------");

            using (var ewma = new EWMAWrapper(0.5))
            {
                Console.WriteLine("Created EWMA with alpha = 0.5 for batch processing");
                Console.WriteLine();

                // Batch processing with all results
                double[] batchValues = { 5.0, 10.0, 8.0, 12.0, 15.0, 9.0, 14.0 };
                Console.WriteLine("Processing batch (getting all intermediate results):");

                double[] batchResults = ewma.ProcessBatch(batchValues);

                Console.WriteLine("Input\tEWMA\tRunning Average");
                Console.WriteLine("-----\t----\t---------------");
                for (int i = 0; i < batchValues.Length; i++)
                {
                    double runningAvg = batchValues.Take(i + 1).Average();
                    Console.WriteLine($"{batchValues[i]:F1}\t{batchResults[i]:F3}\t{runningAvg:F3}");
                }

                // Reset and demo final-only processing
                ewma.Reset();
                Console.WriteLine();
                Console.WriteLine("Processing same batch (getting only final result):");
                double finalResult = ewma.ProcessBatchFinal(batchValues);
                Console.WriteLine($"Final EWMA result: {finalResult:F3}");
            }
        }

        /// <summary>
        /// Demonstrates state management (save/restore) functionality
        /// </summary>
        private static void DemoStateManagement()
        {
            Console.WriteLine("3. State Management Demo");
            Console.WriteLine("-----------------------");

            string savedState;

            // Create first EWMA and process some data
            using (var ewma1 = new EWMAWrapper(0.2))
            {
                Console.WriteLine("Original EWMA (alpha = 0.2):");

                double[] values = { 100.0, 110.0, 95.0, 105.0, 120.0 };
                foreach (double value in values)
                {
                    double result = ewma1.Update(value);
                    Console.WriteLine($"  Update({value:F1}) = {result:F3}");
                }

                // Save state
                savedState = ewma1.GetStateAsJSON();
                Console.WriteLine();
                Console.WriteLine($"Saved state: {savedState}");
            }

            // Create new EWMA and restore state
            Console.WriteLine();
            Console.WriteLine("Creating new EWMA and restoring state:");
            using (var ewma2 = new EWMAWrapper(0.1)) // Different alpha initially
            {
                Console.WriteLine($"Before restore - Current value: {ewma2.GetValue():F3}");

                if (ewma2.SetStateFromJSON(savedState))
                {
                    Console.WriteLine("State restored successfully!");
                    Console.WriteLine($"After restore - Current value: {ewma2.GetValue():F3}");

                    // Continue processing with restored state
                    double newResult = ewma2.Update(130.0);
                    Console.WriteLine($"Continued processing - Update(130.0) = {newResult:F3}");
                }
                else
                {
                    Console.WriteLine("Failed to restore state!");
                }
            }

            // Demonstrate cloning
            Console.WriteLine();
            Console.WriteLine("Demonstrating EWMA cloning:");
            using (var original = new EWMAWrapper(0.4))
            {
                original.Update(50.0);
                original.Update(60.0);

                using (var clone = original.Clone())
                {
                    Console.WriteLine($"Original value: {original.GetValue():F3}");
                    Console.WriteLine($"Clone value: {clone.GetValue():F3}");

                    // Update each independently
                    original.Update(70.0);
                    clone.Update(40.0);

                    Console.WriteLine("After different updates:");
                    Console.WriteLine($"  Original: {original.GetValue():F3}");
                    Console.WriteLine($"  Clone: {clone.GetValue():F3}");
                }
            }
        }

        /// <summary>
        /// Demonstrates changing alpha parameter during operation
        /// </summary>
        private static void DemoAlphaChanges()
        {
            Console.WriteLine("4. Alpha Parameter Demo");
            Console.WriteLine("----------------------");

            using (var ewma = new EWMAWrapper(0.1))
            {
                Console.WriteLine("Starting with alpha = 0.1 (heavy smoothing)");
                Console.WriteLine();

                // Process with low alpha (heavy smoothing)
                double baseValue = 100.0;
                for (int i = 1; i <= 3; i++)
                {
                    double result = ewma.Update(baseValue + i * 10);
                    Console.WriteLine($"  Update({baseValue + i * 10:F1}) = {result:F3}");
                }

                Console.WriteLine($"Current value with alpha=0.1: {ewma.GetValue():F3}");
                Console.WriteLine();

                // Change to higher alpha (less smoothing)
                if (ewma.SetAlpha(0.8))
                {
                    Console.WriteLine("Changed alpha to 0.8 (light smoothing)");

                    for (int i = 4; i <= 6; i++)
                    {
                        double result = ewma.Update(baseValue + i * 10);
                        Console.WriteLine($"  Update({baseValue + i * 10:F1}) = {result:F3}");
                    }

                    Console.WriteLine($"Final value with alpha=0.8: {ewma.GetValue():F3}");
                }
                else
                {
                    Console.WriteLine("Failed to change alpha!");
                }

                // Try invalid alpha
                Console.WriteLine();
                Console.WriteLine("Testing invalid alpha values:");
                Console.WriteLine($"  SetAlpha(-0.1): {ewma.SetAlpha(-0.1)}");
                Console.WriteLine($"  SetAlpha(1.5): {ewma.SetAlpha(1.5)}");
                Console.WriteLine($"  SetAlpha(0.5): {ewma.SetAlpha(0.5)}");
            }
        }

        /// <summary>
        /// Demonstrates performance characteristics
        /// </summary>
        private static void DemoPerformance()
        {
            Console.WriteLine("5. Performance Demo");
            Console.WriteLine("------------------");

            using (var ewma = new EWMAWrapper(0.3))
            {
                const int iterations = 100000;
                var random = new Random(42); // Fixed seed for reproducible results

                Console.WriteLine($"Processing {iterations:N0} random values...");

                var startTime = DateTime.Now;

                for (int i = 1; i <= iterations; i++)
                {
                    double randomValue = random.NextDouble() * 100;
                    ewma.Update(randomValue);
                }

                var endTime = DateTime.Now;
                var elapsed = endTime - startTime;

                Console.WriteLine($"Time taken: {elapsed.TotalMilliseconds:F2} ms");
                Console.WriteLine($"Updates per second: {iterations / elapsed.TotalSeconds:N0}");
                Console.WriteLine($"Microseconds per update: {elapsed.TotalMilliseconds * 1000 / iterations:F2}");
                Console.WriteLine($"Final EWMA value: {ewma.GetValue():F6}");

                // Demonstrate large batch processing
                Console.WriteLine();
                Console.WriteLine("Testing large batch processing...");
                ewma.Reset();

                var largeBatch = new double[10000];
                for (int i = 0; i < largeBatch.Length; i++)
                {
                    largeBatch[i] = Math.Sin(i * 0.1) * 50 + 100; // Sine wave data
                }

                startTime = DateTime.Now;
                double batchResult = ewma.ProcessBatchFinal(largeBatch);
                endTime = DateTime.Now;
                elapsed = endTime - startTime;

                Console.WriteLine($"Processed {largeBatch.Length:N0} values in batch");
                Console.WriteLine($"Time taken: {elapsed.TotalMilliseconds:F2} ms");
                Console.WriteLine($"Final result: {batchResult:F6}");
            }
        }
    }
}
