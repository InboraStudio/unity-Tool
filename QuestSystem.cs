using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();
    
    // Events
    public UnityEvent<Quest> OnQuestStarted;
    public UnityEvent<Quest> OnQuestCompleted;
    public UnityEvent<Quest> OnQuestFailed;
    public UnityEvent<QuestObjective> OnObjectiveCompleted;
    public UnityEvent<QuestObjective> OnObjectiveUpdated;
    
    private List<Quest> activeQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();
    private List<Quest> failedQuests = new List<Quest>();
    
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
            return;
        }
        
        // Initialize events if null
        if (OnQuestStarted == null) OnQuestStarted = new UnityEvent<Quest>();
        if (OnQuestCompleted == null) OnQuestCompleted = new UnityEvent<Quest>();
        if (OnQuestFailed == null) OnQuestFailed = new UnityEvent<Quest>();
        if (OnObjectiveCompleted == null) OnObjectiveCompleted = new UnityEvent<QuestObjective>();
        if (OnObjectiveUpdated == null) OnObjectiveUpdated = new UnityEvent<QuestObjective>();
    }
    
    public void StartQuest(string questID)
    {
        Quest quest = availableQuests.Find(q => q.questID == questID);
        
        if (quest == null)
        {
            Debug.LogWarning("Quest with ID " + questID + " not found!");
            return;
        }
        
        if (activeQuests.Contains(quest) || completedQuests.Contains(quest))
        {
            Debug.Log("Quest already active or completed!");
            return;
        }
        
        // Check quest requirements
        if (!CheckQuestRequirements(quest))
        {
            Debug.Log("Quest requirements not met!");
            return;
        }
        
        // Clone the quest to avoid modifying the original
        Quest activeQuest = Instantiate(quest);
        activeQuest.status = QuestStatus.InProgress;
        activeQuest.InitializeObjectives();
        
        activeQuests.Add(activeQuest);
        OnQuestStarted.Invoke(activeQuest);
        
        Debug.Log("Started quest: " + activeQuest.questName);
    }
    
    public void CompleteObjective(string questID, string objectiveID, int amount = 1)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        
        if (quest == null)
        {
            Debug.LogWarning("Active quest with ID " + questID + " not found!");
            return;
        }
        
        QuestObjective objective = quest.objectives.Find(o => o.objectiveID == objectiveID);
        
        if (objective == null)
        {
            Debug.LogWarning("Objective with ID " + objectiveID + " not found in quest " + questID);
            return;
        }
        
        if (objective.isCompleted)
        {
            return;
        }
        
        objective.currentAmount += amount;
        OnObjectiveUpdated.Invoke(objective);
        
        if (objective.currentAmount >= objective.requiredAmount)
        {
            objective.isCompleted = true;
            OnObjectiveCompleted.Invoke(objective);
            
            Debug.Log("Completed objective: " + objective.description);
            
            // Check if all objectives are completed
            if (quest.AreAllObjectivesCompleted())
            {
                CompleteQuest(quest);
            }
        }
    }
    
    private void CompleteQuest(Quest quest)
    {
        quest.status = QuestStatus.Completed;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        
        // Award quest rewards
        GiveQuestRewards(quest);
        
        OnQuestCompleted.Invoke(quest);
        Debug.Log("Completed quest: " + quest.questName);
    }
    
    public void FailQuest(string questID)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        
        if (quest == null)
        {
            Debug.LogWarning("Active quest with ID " + questID + " not found!");
            return;
        }
        
        quest.status = QuestStatus.Failed;
        activeQuests.Remove(quest);
        failedQuests.Add(quest);
        
        OnQuestFailed.Invoke(quest);
        Debug.Log("Failed quest: " + quest.questName);
    }
    
    private bool CheckQuestRequirements(Quest quest)
    {
        // Check for required completed quests
        foreach (string requiredQuestID in quest.requiredQuests)
        {
            bool found = false;
            
            foreach (Quest completedQuest in completedQuests)
            {
                if (completedQuest.questID == requiredQuestID)
                {
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                return false;
            }
        }
        
        // Check for level requirement
        if (quest.requiredLevel > 0)
        {
            // This is where you would check player level
            // For demo purposes, always return true
            // int playerLevel = GameManager.Instance.GetPlayerLevel();
            // if (playerLevel < quest.requiredLevel)
            // {
            //     return false;
            // }
        }
        
        return true;
    }
    
    private void GiveQuestRewards(Quest quest)
    {
        // This is where you would give the player rewards
        // For example:
        // if (quest.rewardGold > 0)
        // {
        //     GameManager.Instance.AddPlayerGold(quest.rewardGold);
        // }
        // 
        // if (quest.rewardExperience > 0)
        // {
        //     GameManager.Instance.AddPlayerExperience(quest.rewardExperience);
        // }
        // 
        // foreach (ItemReward itemReward in quest.rewardItems)
        // {
        //     InventorySystem.Instance.AddItem(itemReward.itemID, itemReward.quantity);
        // }
    }
    
    public List<Quest> GetAllQuests()
    {
        return availableQuests;
    }
    
    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }
    
    public List<Quest> GetCompletedQuests()
    {
        return completedQuests;
    }
    
    public List<Quest> GetFailedQuests()
    {
        return failedQuests;
    }
    
    public Quest GetQuest(string questID)
    {
        Quest quest = availableQuests.Find(q => q.questID == questID);
        
        if (quest == null)
        {
            Debug.LogWarning("Quest with ID " + questID + " not found!");
        }
        
        return quest;
    }
    
    public QuestStatus GetQuestStatus(string questID)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.questID == questID)
            {
                return QuestStatus.InProgress;
            }
        }
        
        foreach (Quest quest in completedQuests)
        {
            if (quest.questID == questID)
            {
                return QuestStatus.Completed;
            }
        }
        
        foreach (Quest quest in failedQuests)
        {
            if (quest.questID == questID)
            {
                return QuestStatus.Failed;
            }
        }
        
        return QuestStatus.NotStarted;
    }
}

[System.Serializable]
public class Quest
{
    public string questID;
    public string questName;
    [TextArea(3, 5)]
    public string description;
    public QuestType questType = QuestType.Main;
    public QuestStatus status = QuestStatus.NotStarted;
    
    [Header("Requirements")]
    public int requiredLevel;
    public List<string> requiredQuests = new List<string>();
    
    [Header("Objectives")]
    public List<QuestObjective> objectives = new List<QuestObjective>();
    
    [Header("Rewards")]
    public int rewardGold;
    public int rewardExperience;
    public List<ItemReward> rewardItems = new List<ItemReward>();
    
    [Header("Other")]
    public bool isAutoComplete = false;
    public bool isTimed = false;
    public float timeLimit = 0f;
    
    public void InitializeObjectives()
    {
        foreach (QuestObjective objective in objectives)
        {
            objective.currentAmount = 0;
            objective.isCompleted = false;
        }
    }
    
    public bool AreAllObjectivesCompleted()
    {
        foreach (QuestObjective objective in objectives)
        {
            if (!objective.isCompleted)
            {
                return false;
            }
        }
        
        return true;
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID;
    [TextArea(2, 3)]
    public string description;
    public QuestObjectiveType type = QuestObjectiveType.Collect;
    public int requiredAmount = 1;
    public int currentAmount = 0;
    public bool isCompleted = false;
    public bool isOptional = false;
    
    public float GetProgress()
    {
        return (float)currentAmount / requiredAmount;
    }
}

[System.Serializable]
public class ItemReward
{
    public string itemID;
    public int quantity = 1;
}

public enum QuestType
{
    Main,
    Side,
    Daily,
    World,
    Repeatable
}

public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

public enum QuestObjectiveType
{
    Kill,
    Collect,
    Interact,
    Escort,
    Talk,
    Reach,
    Defend,
    Craft,
    Discover
} 