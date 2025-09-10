using UnityEngine;

public class OreNode : MonoBehaviour
{
    [Header("HP")]
    public float maxHpMin = 20f;
    public float maxHpMax = 60f;

    [Header("Drop")]
    public int yieldAmount = 1;

    float hp;

    void OnEnable()
    {
        hp = Random.Range(maxHpMin, maxHpMax);
    }

    public void TakeDamage(float amount)
    {
        if (hp <= 0f) return;
        hp -= amount;
        if (hp <= 0f) Deplete();
    }

    void Deplete()
    {
        var counter = FindObjectOfType<ResourceCounter>();
        if (counter != null) counter.Add(yieldAmount);
        Destroy(gameObject);
    }
}