using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isInvulnerable = false;
    
    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged;
    public UnityEvent<float> onDamageTaken;
    public UnityEvent<float> onHealed;
    
    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        onDamageTaken?.Invoke(damage);
        onHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealed?.Invoke(amount);
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }
    
    private void Die()
    {
        onDeath?.Invoke();
        // Add additional death logic here
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
} 