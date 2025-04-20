# Unity Tool Scripts

A collection of useful Unity scripts for common game development tasks.

## New Updates (May 2025)

Added six powerful systems to enhance your Unity projects:

### 1. DialogueSystem
- Typed text effect with configurable speed
- Support for character portraits and names
- Event-driven dialogue progression
- Audio support for typing sounds
- Easy to integrate with UI elements
- Handles dialogue sequences with multiple sentences

### 2. InventorySystem
- Flexible item management with stacking support
- Events for item addition, removal, and usage
- Support for different item types (weapons, consumables, etc.)
- Easy to integrate with UI through events
- Custom item properties with dictionary-based stats

### 3. ObjectPooler
- Efficient object instantiation and recycling
- Reduces garbage collection and improves performance
- Supports multiple object pools with different prefabs
- Expandable pools for dynamic content
- Interface for custom initialization on object spawn

### 4. SaveSystem
- Binary serialization for game data
- Optional encryption for secure saves
- Auto-save functionality with configurable intervals
- Interface for objects that need to save/load data
- Easy to use API for saving custom data types

### 5. QuestSystem
- Comprehensive quest management
- Support for multiple quest types and objectives
- Quest requirements and prerequisites
- Reward system for quest completion
- Progress tracking for objectives
- Event-driven updates for UI integration

### 6. AudioManager
- Centralized audio playback system
- Separate channels for music, SFX, ambience, and UI sounds
- Smooth fade transitions between tracks
- 3D positional audio support
- Volume control with settings persistence
- Audio mixer integration
- Sound collection management

## Previous Updates (April 2024)

Added five utility scripts to enhance game development:

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
   - DialogueSystem
   - InventorySystem
   - ObjectPooler
   - SaveSystem
   - QuestSystem
   - AudioManager

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

6. For DialogueSystem:
   - Create UI elements for dialogue panel, name text, and dialogue text
   - Assign references in the inspector
   - Set up TextMeshPro components

7. For ObjectPooler:
   - Define pools in the inspector with prefabs, sizes, and tags
   - Use the pooling API to spawn and recycle objects

8. For SaveSystem:
   - Configure save settings like filename and encryption
   - Implement ISaveable interface on objects that need saving

9. For QuestSystem:
   - Define quests in the inspector with objectives and rewards
   - Connect UI elements to events for updates

10. For AudioManager:
    - Create audio sources or let the system create them automatically
    - Define sound collections in the inspector
    - Create an Audio Mixer with appropriate groups (optional)
    - Set up volume sliders in the UI connected to the volume methods

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

### DialogueSystem
```csharp
// Create dialogue
Dialogue dialogue = new Dialogue();
dialogue.speakerName = "NPC";
dialogue.speakerImage = npcSprite;
dialogue.sentences = new string[] {
    "Hello there!",
    "How can I help you today?",
    "Feel free to ask me anything."
};

// Start dialogue
DialogueSystem.Instance.StartDialogue(dialogue);

// Display next sentence (usually connected to button)
DialogueSystem.Instance.DisplayNextSentence();
```

### InventorySystem
```csharp
// Create item
InventoryItem potion = new InventoryItem(
    "potion001", 
    "Health Potion", 
    "Restores 50 health points", 
    potionSprite, 
    5
);
potion.isConsumable = true;

// Add to inventory
InventorySystem.Instance.AddItem(potion);

// Use item
InventorySystem.Instance.UseItem("potion001");

// Check if player has item
if (InventorySystem.Instance.HasItem("potion001", 2)) {
    // Player has at least 2 potions
}
```

### ObjectPooler
```csharp
// Spawn object from pool
GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", firePoint.position, firePoint.rotation);

// Return object to pool when done
ObjectPooler.Instance.ReturnToPool(bullet);
```

### SaveSystem
```csharp
// Save game
SaveSystem.Instance.SaveGame();

// Load game
SaveSystem.Instance.LoadGame();

// Save specific data
SaveSystem.Instance.SaveData("PlayerPosition", transform.position);

// Load specific data
Vector3 position = SaveSystem.Instance.LoadData<Vector3>("PlayerPosition", Vector3.zero);
```

### QuestSystem
```csharp
// Start quest
QuestSystem.Instance.StartQuest("quest001");

// Complete objective
QuestSystem.Instance.CompleteObjective("quest001", "objective001", 1);

// Get quest status
QuestStatus status = QuestSystem.Instance.GetQuestStatus("quest001");
if (status == QuestStatus.Completed) {
    // Quest is completed
}
```

### AudioManager
```csharp
// Play background music with fade
AudioManager.Instance.PlayMusic("MainTheme", 2.0f);

// Play sound effect
AudioManager.Instance.PlaySFX("Jump");

// Play 3D sound at position
AudioManager.Instance.PlaySFXAtPosition("Explosion", explosionPosition);

// Play ambient sound
AudioManager.Instance.PlayAmbience("ForestAmbience");

// Adjust volume
AudioManager.Instance.SetMusicVolume(0.5f);
```

## Requirements

- Unity 2019.4 or later
- TextMeshPro package (for UI components)
- .NET 4.x or later

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
