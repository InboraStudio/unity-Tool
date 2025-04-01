# Unity Tool Scripts

A collection of useful Unity scripts for common game development tasks.

## New Updates (April 2024)

Added five new utility scripts to enhance game development:

### 1. GameManager
- Singleton pattern implementation
- Handles game state management
- Controls game pausing and resuming
- Manages game over state
- Easy to extend with custom game logic

### 2. DataManager
- Handles data persistence using PlayerPrefs
- Supports saving and loading different data types (int, float, string)
- Provides methods for data deletion
- Type-safe data loading with generics
- Automatic data saving

### 3. CameraController
- Smooth camera following behavior
- Configurable offset and boundaries
- Adjustable smooth speed and rotation speed
- Prevents camera from going out of bounds
- Easy to set up with target object

### 4. HealthSystem
- Flexible health management system
- Supports damage and healing
- Includes invulnerability state
- Event-driven system for health changes
- Easy integration with UI elements
- Death handling with custom events

### 5. TooltipManager
- Smooth UI tooltip system
- Fade in/out animations
- Configurable delay and offset
- Follows mouse position
- Uses TextMeshPro for better text rendering
- Canvas group for smooth transitions

## Setup Instructions

1. Create empty GameObjects in your scene for the managers:
   - GameManager
   - DataManager
   - TooltipManager

2. Attach the respective scripts to these GameObjects

3. For CameraController:
   - Attach to your main camera
   - Set the target in the inspector
   - Adjust offset and boundaries as needed

4. For HealthSystem:
   - Attach to any object that needs health management
   - Configure max health and other settings in inspector
   - Set up event listeners for health changes

5. For TooltipManager:
   - Create a UI panel with TextMeshPro component
   - Assign references in the inspector
   - Configure show delay and fade speed

## Usage Examples

### GameManager
```csharp
// Pause game
GameManager.Instance.PauseGame();

// Resume game
GameManager.Instance.ResumeGame();

// Check game state
if (GameManager.Instance.isGamePaused) {
    // Handle pause state
}
```

### DataManager
```csharp
// Save data
DataManager.Instance.SaveData("PlayerScore", 100);

// Load data
int score = DataManager.Instance.LoadData("PlayerScore", 0);

// Delete data
DataManager.Instance.DeleteData("PlayerScore");
```

### HealthSystem
```csharp
// Take damage
healthSystem.TakeDamage(20f);

// Heal
healthSystem.Heal(10f);

// Check health
float healthPercent = healthSystem.GetHealthPercentage();
```

### TooltipManager
```csharp
// Show tooltip
TooltipManager.Instance.ShowTooltip("Press E to interact", Input.mousePosition);

// Hide tooltip
TooltipManager.Instance.HideTooltip();
```

## Requirements

- Unity 2019.4 or later
- TextMeshPro package (for TooltipManager)
- .NET 4.x or later

## License

This project is licensed under the MIT License - see the LICENSE file for details. 