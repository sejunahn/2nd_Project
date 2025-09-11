using UnityEngine;

public class OreNode : MonoBehaviour
{
    [Header("HP")]
    public float maxHpMin = 20f;
    public float maxHpMax = 60f;

    [Header("Drop")]
    public int yieldAmount = 1;

    [Header("Hit Effect")]
    public Color hitFlashColor = Color.white;
    public float flashDuration = 0.1f;
    public float hitShakeAmount = 0.5f; // 🔥 0.5~1 정도면 눈에 잘 보임
    public float hitShakeTime = 0.15f;

    private float hp;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalPos;
    private float flashTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        // ✅ localPosition 대신 worldPosition 저장
        originalPos = transform.position;
    }

    void OnEnable()
    {
        hp = Random.Range(maxHpMin, maxHpMax);

        if (sr != null)
            sr.color = originalColor;

        transform.position = originalPos;
        flashTimer = 0f;
    }

    public void TakeDamage(float amount)
    {
        if (hp <= 0f) return;

        hp -= amount;

        // 반짝 효과
        if (sr != null)
            sr.color = hitFlashColor;
        flashTimer = flashDuration;

        // 흔들림 효과 코루틴
        StopAllCoroutines();
        StartCoroutine(HitShake());

        if (hp <= 0f) Deplete();
    }

    System.Collections.IEnumerator HitShake()
    {
        float timer = hitShakeTime;
        while (timer > 0f)
        {
            // 🔥 World 좌표 기준으로 크게 흔들림
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * hitShakeAmount;
            timer -= Time.deltaTime;
            yield return null;
        }

        // 원래 위치 복귀
        transform.position = originalPos;
    }

    void Update()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f && sr != null)
            {
                sr.color = originalColor;
            }
        }
    }

    void Deplete()
    {
        var counter = FindObjectOfType<ResourceCounter>();
        if (counter != null) counter.Add(yieldAmount);

        Destroy(gameObject);
    }
}
