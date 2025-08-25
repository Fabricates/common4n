package main

import (
	"math"
	"testing"
)

func TestNewEWMA(t *testing.T) {
	ewma := NewEWMA(0.3)
	if ewma.Alpha != 0.3 {
		t.Errorf("Expected alpha 0.3, got %f", ewma.Alpha)
	}
	if ewma.IsInit {
		t.Error("Expected IsInit to be false")
	}
}

func TestNewEWMAInvalidAlpha(t *testing.T) {
	ewma := NewEWMA(-0.1)
	if ewma.Alpha != 0.1 {
		t.Errorf("Expected default alpha 0.1, got %f", ewma.Alpha)
	}

	ewma2 := NewEWMA(1.5)
	if ewma2.Alpha != 0.1 {
		t.Errorf("Expected default alpha 0.1, got %f", ewma2.Alpha)
	}
}

func TestEWMAUpdate(t *testing.T) {
	ewma := NewEWMA(0.3)

	// First update should set the value directly
	result1 := ewma.Update(10.0)
	if result1 != 10.0 {
		t.Errorf("Expected first value 10.0, got %f", result1)
	}

	// Second update should calculate EWMA
	result2 := ewma.Update(20.0)
	expected := 0.3*20.0 + 0.7*10.0 // = 6 + 7 = 13
	if math.Abs(result2-expected) > 1e-10 {
		t.Errorf("Expected %f, got %f", expected, result2)
	}
}

func TestEWMABatch(t *testing.T) {
	ewma := NewEWMA(0.5)
	values := []float64{10, 20, 15, 25}
	results := ewma.CalculateBatch(values)

	if len(results) != len(values) {
		t.Errorf("Expected %d results, got %d", len(values), len(results))
	}

	// First value should be 10
	if results[0] != 10.0 {
		t.Errorf("Expected first result 10.0, got %f", results[0])
	}

	// Second value should be 0.5*20 + 0.5*10 = 15
	expected := 0.5*20.0 + 0.5*10.0
	if math.Abs(results[1]-expected) > 1e-10 {
		t.Errorf("Expected second result %f, got %f", expected, results[1])
	}
}

func TestEWMAReset(t *testing.T) {
	ewma := NewEWMA(0.3)
	ewma.Update(10.0)
	ewma.Reset()

	if ewma.IsInit {
		t.Error("Expected IsInit to be false after reset")
	}
	if ewma.Value != 0.0 {
		t.Errorf("Expected value 0.0 after reset, got %f", ewma.Value)
	}
}

func TestEWMASetAlpha(t *testing.T) {
	ewma := NewEWMA(0.3)

	// Valid alpha
	if !ewma.SetAlpha(0.5) {
		t.Error("Expected SetAlpha to return true for valid alpha")
	}
	if ewma.Alpha != 0.5 {
		t.Errorf("Expected alpha 0.5, got %f", ewma.Alpha)
	}

	// Invalid alpha
	if ewma.SetAlpha(-0.1) {
		t.Error("Expected SetAlpha to return false for invalid alpha")
	}
	if ewma.Alpha != 0.5 {
		t.Errorf("Expected alpha to remain 0.5, got %f", ewma.Alpha)
	}
}

func TestEWMAJSON(t *testing.T) {
	ewma := NewEWMA(0.3)
	ewma.Update(10.0)
	ewma.Update(20.0)

	// Serialize to JSON
	jsonStr := ewma.ToJSON()
	if jsonStr == "" {
		t.Error("Expected non-empty JSON string")
	}

	// Create new EWMA and deserialize
	ewma2 := NewEWMA(0.1) // Different alpha
	if !ewma2.FromJSON(jsonStr) {
		t.Error("Expected FromJSON to return true")
	}

	// Check values
	if ewma2.Alpha != ewma.Alpha {
		t.Errorf("Expected alpha %f, got %f", ewma.Alpha, ewma2.Alpha)
	}
	if ewma2.Value != ewma.Value {
		t.Errorf("Expected value %f, got %f", ewma.Value, ewma2.Value)
	}
	if ewma2.IsInit != ewma.IsInit {
		t.Errorf("Expected IsInit %t, got %t", ewma.IsInit, ewma2.IsInit)
	}
}

// Benchmark tests
func BenchmarkEWMAUpdate(b *testing.B) {
	ewma := NewEWMA(0.3)
	value := 10.0

	b.ResetTimer()
	for i := 0; i < b.N; i++ {
		ewma.Update(value)
		value += 0.1
	}
}

func BenchmarkEWMABatch(b *testing.B) {
	ewma := NewEWMA(0.3)
	values := make([]float64, 1000)
	for i := range values {
		values[i] = float64(i)
	}

	b.ResetTimer()
	for i := 0; i < b.N; i++ {
		ewma.Reset()
		ewma.CalculateBatch(values)
	}
}
