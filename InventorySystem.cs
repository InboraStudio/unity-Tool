using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxInventorySlots = 20;
    
    // Events
    public UnityEvent<InventoryItem> OnItemAdded;
    public UnityEvent<InventoryItem> OnItemRemoved;
    public UnityEvent<InventoryItem> OnItemUsed;
    public UnityEvent OnInventoryChanged;

    private List<InventoryItem> inventoryItems = new List<InventoryItem>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize events if null
        if (OnItemAdded == null) OnItemAdded = new UnityEvent<InventoryItem>();
        if (OnItemRemoved == null) OnItemRemoved = new UnityEvent<InventoryItem>();
        if (OnItemUsed == null) OnItemUsed = new UnityEvent<InventoryItem>();
        if (OnInventoryChanged == null) OnInventoryChanged = new UnityEvent();
    }
    
    public bool AddItem(InventoryItem item)
    {
        if (inventoryItems.Count >= maxInventorySlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        
        // Check if the item is stackable and already exists
        if (item.isStackable)
        {
            InventoryItem existingItem = inventoryItems.Find(i => i.itemID == item.itemID);
            
            if (existingItem != null)
            {
                existingItem.quantity += item.quantity;
                OnInventoryChanged.Invoke();
                OnItemAdded.Invoke(item);
                return true;
            }
        }
        
        // Add new item
        inventoryItems.Add(item);
        OnInventoryChanged.Invoke();
        OnItemAdded.Invoke(item);
        return true;
    }
    
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemID == itemID);
        
        if (item == null)
        {
            Debug.Log("Item not found in inventory!");
            return false;
        }
        
        if (item.quantity < quantity)
        {
            Debug.Log("Not enough items to remove!");
            return false;
        }
        
        item.quantity -= quantity;
        
        if (item.quantity <= 0)
        {
            inventoryItems.Remove(item);
        }
        
        OnInventoryChanged.Invoke();
        OnItemRemoved.Invoke(item);
        return true;
    }
    
    public bool UseItem(string itemID)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemID == itemID);
        
        if (item == null)
        {
            Debug.Log("Item not found in inventory!");
            return false;
        }
        
        // Handle consumable items
        if (item.isConsumable)
        {
            item.quantity--;
            
            if (item.quantity <= 0)
            {
                inventoryItems.Remove(item);
            }
        }
        
        OnInventoryChanged.Invoke();
        OnItemUsed.Invoke(item);
        return true;
    }
    
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventoryItems);
    }
    
    public InventoryItem GetItem(string itemID)
    {
        return inventoryItems.Find(i => i.itemID == itemID);
    }
    
    public int GetItemCount(string itemID)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemID == itemID);
        return item != null ? item.quantity : 0;
    }
    
    public bool HasItem(string itemID, int quantity = 1)
    {
        return GetItemCount(itemID) >= quantity;
    }
    
    public void ClearInventory()
    {
        inventoryItems.Clear();
        OnInventoryChanged.Invoke();
    }
}

[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public string itemName;
    public string description;
    public Sprite icon;
    public int quantity = 1;
    public bool isStackable = true;
    public bool isConsumable = false;
    public ItemType itemType = ItemType.Misc;
    public Dictionary<string, float> stats = new Dictionary<string, float>();
    
    public InventoryItem(string id, string name, string desc, Sprite itemIcon, int qty = 1)
    {
        itemID = id;
        itemName = name;
        description = desc;
        icon = itemIcon;
        quantity = qty;
    }
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Misc
} 