package main

/*
#include <stdlib.h>
*/
import "C"
import (
	"encoding/json"
	"fmt"
	"math"
	"unsafe"
)

// EWMA represents an Exponential Weighted Moving Average calculator
type EWMA struct {
	Alpha  float64 `json:"alpha"`
	Value  float64 `json:"value"`
	IsInit bool    `json:"is_init"`
}

// NewEWMA creates a new EWMA instance with the given alpha parameter
// Alpha should be between 0 and 1, where higher values give more weight to recent observations
func NewEWMA(alpha float64) *EWMA {
	if alpha < 0 || alpha > 1 {
		alpha = 0.1 // Default to 0.1 if invalid
	}
	return &EWMA{
		Alpha:  alpha,
		Value:  0.0,
		IsInit: false,
	}
}

// Update adds a new observation to the EWMA and returns the updated average
func (e *EWMA) Update(value float64) float64 {
	if !e.IsInit {
		e.Value = value
		e.IsInit = true
	} else {
		e.Value = e.Alpha*value + (1-e.Alpha)*e.Value
	}
	return e.Value
}

// GetValue returns the current EWMA value
func (e *EWMA) GetValue() float64 {
	return e.Value
}

// Reset resets the EWMA to its initial state
func (e *EWMA) Reset() {
	e.Value = 0.0
	e.IsInit = false
}

// SetAlpha updates the alpha parameter
func (e *EWMA) SetAlpha(alpha float64) bool {
	if alpha < 0 || alpha > 1 {
		return false
	}
	e.Alpha = alpha
	return true
}

// CalculateBatch calculates EWMA for a batch of values
func (e *EWMA) CalculateBatch(values []float64) []float64 {
	results := make([]float64, len(values))
	for i, value := range values {
		results[i] = e.Update(value)
	}
	return results
}

// ToJSON serializes the EWMA state to JSON
func (e *EWMA) ToJSON() string {
	data, err := json.Marshal(e)
	if err != nil {
		return "{\"error\":\"serialization failed\"}"
	}
	return string(data)
}

// FromJSON deserializes EWMA state from JSON
func (e *EWMA) FromJSON(jsonStr string) bool {
	err := json.Unmarshal([]byte(jsonStr), e)
	return err == nil
}

// Global EWMA instances for C interface
var ewmaInstances = make(map[int]*EWMA)
var nextInstanceID = 1

//export CreateEWMA
func CreateEWMA(alpha float64) int {
	id := nextInstanceID
	nextInstanceID++
	ewmaInstances[id] = NewEWMA(alpha)
	return id
}

//export UpdateEWMA
func UpdateEWMA(instanceID int, value float64) float64 {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		return ewma.Update(value)
	}
	return math.NaN()
}

//export GetEWMAValue
func GetEWMAValue(instanceID int) float64 {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		return ewma.GetValue()
	}
	return math.NaN()
}

//export ResetEWMA
func ResetEWMA(instanceID int) bool {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		ewma.Reset()
		return true
	}
	return false
}

//export SetEWMAAlpha
func SetEWMAAlpha(instanceID int, alpha float64) bool {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		return ewma.SetAlpha(alpha)
	}
	return false
}

//export DestroyEWMA
func DestroyEWMA(instanceID int) bool {
	if _, exists := ewmaInstances[instanceID]; exists {
		delete(ewmaInstances, instanceID)
		return true
	}
	return false
}

//export CalculateEWMABatch
func CalculateEWMABatch(instanceID int, values *float64, length int) *float64 {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		// Convert C array to Go slice
		valueSlice := (*[1 << 30]float64)(unsafe.Pointer(values))[:length:length]

		// Calculate EWMA for all values
		results := ewma.CalculateBatch(valueSlice)

		// Convert Go slice to C array
		if len(results) > 0 {
			return &results[0]
		}
	}
	return nil
}

//export GetEWMAStateJSON
func GetEWMAStateJSON(instanceID int) *C.char {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		jsonStr := ewma.ToJSON()
		return C.CString(jsonStr)
	}
	return C.CString("{\"error\":\"instance not found\"}")
}

//export SetEWMAStateJSON
func SetEWMAStateJSON(instanceID int, jsonStr *C.char) bool {
	if ewma, exists := ewmaInstances[instanceID]; exists {
		return ewma.FromJSON(C.GoString(jsonStr))
	}
	return false
}

//export FreeString
func FreeString(str *C.char) {
	C.free(unsafe.Pointer(str))
}

// Example function for testing
//
//export TestEWMA
func TestEWMA() {
	ewma := NewEWMA(0.3)
	values := []float64{10, 20, 15, 25, 30}

	fmt.Println("EWMA Test Results:")
	for i, value := range values {
		result := ewma.Update(value)
		fmt.Printf("Step %d: Input=%f, EWMA=%f\n", i+1, value, result)
	}
}

func main() {
	// This is required for building a shared library
	// The main function will not be executed when used as a library
	fmt.Println("EWMA Library compiled successfully")
}
