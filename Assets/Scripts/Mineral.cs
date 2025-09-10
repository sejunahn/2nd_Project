using UnityEngine;

public class Mineral : MonoBehaviour
{
    [Header("Mineral Properties")]
    public int health = 5;
    public int maxHealth = 5;
    
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Color normalColor = Color.white;
    public Color hitColor = Color.red;
    
    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        health = maxHealth;
    }
    
    public void TakeDamage(int damage = 1)
    {
        health -= damage;
        
        // 피격 효과
        StartCoroutine(HitEffect());
        
        if (health <= 0)
        {
            // 광물 파괴
            FindObjectOfType<MapGenerator>().RemoveMineral(this);
        }
    }
    
    System.Collections.IEnumerator HitEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = normalColor;
    }
}