#include <stdio.h>
#include <stdlib.h>
#include "libewma.h"

int main() {
    printf("Testing EWMA C Library\n");
    printf("======================\n");

    // Create EWMA instance
    int ewmaID = CreateEWMA(0.3);
    if (ewmaID == 0) {
        printf("Failed to create EWMA instance\n");
        return 1;
    }
    printf("Created EWMA instance with ID: %d\n", ewmaID);

    // Test individual updates
    printf("\nTesting individual updates:\n");
    printf("Value\tEWMA\n");
    printf("-----\t----\n");
    
    double values[] = {10.0, 20.0, 15.0, 25.0, 30.0};
    int numValues = sizeof(values) / sizeof(values[0]);
    
    for (int i = 0; i < numValues; i++) {
        double result = UpdateEWMA(ewmaID, values[i]);
        printf("%.1f\t%.3f\n", values[i], result);
    }

    // Test getting current value
    double currentValue = GetEWMAValue(ewmaID);
    printf("\nCurrent EWMA value: %.3f\n", currentValue);

    // Test reset
    printf("\nTesting reset...\n");
    ResetEWMA(ewmaID);
    double afterReset = GetEWMAValue(ewmaID);
    printf("Value after reset: %.3f\n", afterReset);

    // Test alpha change
    printf("\nTesting alpha change...\n");
    if (SetEWMAAlpha(ewmaID, 0.7)) {
        printf("Successfully changed alpha to 0.7\n");
        UpdateEWMA(ewmaID, 100.0);
        double newValue = GetEWMAValue(ewmaID);
        printf("Value after alpha change and update(100): %.3f\n", newValue);
    } else {
        printf("Failed to change alpha\n");
    }

    // Test JSON state
    printf("\nTesting JSON state...\n");
    char* jsonState = GetEWMAStateJSON(ewmaID);
    if (jsonState != NULL) {
        printf("Current state: %s\n", jsonState);
        FreeString(jsonState);
    }

    // Clean up
    if (DestroyEWMA(ewmaID)) {
        printf("\nSuccessfully destroyed EWMA instance\n");
    } else {
        printf("\nFailed to destroy EWMA instance\n");
    }

    printf("\nTest completed successfully!\n");
    return 0;
}
