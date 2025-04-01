using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    
    [Header("UI References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltipRect;
    
    [Header("Settings")]
    public float showDelay = 0.5f;
    public float fadeSpeed = 0.2f;
    public Vector2 offset = new Vector2(10f, 10f);
    
    private CanvasGroup canvasGroup;
    private Coroutine showCoroutine;
    
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
        
        canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tooltipPanel.AddComponent<CanvasGroup>();
            
        HideTooltip();
    }
    
    public void ShowTooltip(string text, Vector2 position)
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
            
        showCoroutine = StartCoroutine(ShowTooltipCoroutine(text, position));
    }
    
    private IEnumerator ShowTooltipCoroutine(string text, Vector2 position)
    {
        yield return new WaitForSeconds(showDelay);
        
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
        
        // Position the tooltip
        Vector2 screenPoint = position + offset;
        tooltipRect.position = screenPoint;
        
        // Fade in
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }
    }
    
    public void HideTooltip()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        
        StartCoroutine(HideTooltipCoroutine());
    }
    
    private IEnumerator HideTooltipCoroutine()
    {
        float alpha = canvasGroup.alpha;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeSpeed;
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        tooltipPanel.SetActive(false);
    }
    
    public void UpdatePosition(Vector2 position)
    {
        if (tooltipPanel.activeSelf)
        {
            tooltipRect.position = position + offset;
        }
    }
} 