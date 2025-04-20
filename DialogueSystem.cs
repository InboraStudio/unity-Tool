using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerImage;
    [SerializeField] private Button continueButton;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource audioSource;

    private Queue<string> sentences;
    private string currentSpeaker;
    private Sprite currentSpeakerImage;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            sentences = new Queue<string>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        if (continueButton != null)
            continueButton.onClick.AddListener(DisplayNextSentence);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialoguePanel.SetActive(true);
        
        sentences.Clear();
        
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        
        currentSpeaker = dialogue.speakerName;
        currentSpeakerImage = dialogue.speakerImage;
        
        if (nameText != null)
            nameText.text = currentSpeaker;
            
        if (speakerImage != null && currentSpeakerImage != null)
            speakerImage.sprite = currentSpeakerImage;
            
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // If currently typing, display full text immediately
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
                
            dialogueText.text = dialogueText.text; // Set to full text
            isTyping = false;
            return;
        }
        
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
            
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            
            if (typingSound != null && audioSource != null)
                audioSource.PlayOneShot(typingSound);
                
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}

[System.Serializable]
public class Dialogue
{
    public string speakerName;
    public Sprite speakerImage;
    public string[] sentences;
} 