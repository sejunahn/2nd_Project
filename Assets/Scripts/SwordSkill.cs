using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSkill : MonoBehaviour
{
    [Header("Stamp Settings")]
    public float stampInterval = 1f;    // 몇 초마다 발동
    public float stampDuration = 0.2f;  // 내려찍는 속도
    public float returnDuration = 0.1f; // 올라가는 속도
    public float stampHeight = 2f;      // 위에서 얼마나 내려오는지
    public float hitRadius = 2.5f;      // 범위
    public float damage = 20000f;       // 데미지
    public LayerMask targetLayer;       // Ore 레이어

    [Header("Area Visual")]
    public GameObject areaIndicatorPrefab;
    private GameObject areaIndicator;

    [Header("Dependencies")]
    public IsoGridGenerator gridGenerator;

    private bool isStamping = false;
    private SpriteRenderer swordRenderer;

    void Awake()
    {
        swordRenderer = GetComponent<SpriteRenderer>();
        if (swordRenderer != null) swordRenderer.enabled = false;
    }

    public void Init()
    {
        if (StatManager.Instance.unlockSword)
            StartCoroutine(SwordRoutine());
    }

    IEnumerator SwordRoutine()
    {
        while (StatManager.Instance.unlockSword)
        {
            yield return new WaitForSeconds(stampInterval);

            // 광석 위치 가져오기
            Vector2? spawnPos = GetRandomOrePosition();
            if (spawnPos == null)
                continue; // 광석이 없으면 발동 안 함

            transform.position = spawnPos.Value;

            if (swordRenderer != null) swordRenderer.enabled = true;

            yield return StartCoroutine(StampRoutine());

            if (swordRenderer != null) swordRenderer.enabled = false;
        }
    }

    IEnumerator StampRoutine()
    {
        if (isStamping) yield break;
        isStamping = true;

        // 범위 원 표시
        if (areaIndicatorPrefab != null)
        {
            areaIndicator = Instantiate(areaIndicatorPrefab, transform.position, Quaternion.identity);
            areaIndicator.transform.localScale = Vector3.one * (hitRadius * 2f);
        }

        Vector3 startPos = transform.position + Vector3.up * stampHeight;
        Vector3 targetPos = transform.position;

        // 위에서 시작
        transform.position = startPos;

        // 내려찍기
        float elapsed = 0f;
        while (elapsed < stampDuration)
        {
            float t = elapsed / stampDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        // 충돌 처리 (쾅 찍을 때 한 번만)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetLayer);
        foreach (var hit in hits)
        {
            OreNode ore = hit.GetComponent<OreNode>();
            if (ore != null)
                ore.TakeDamage(damage);
        }

        // 살짝 다시 올라갔다가 사라지게
        elapsed = 0f;
        Vector3 returnPos = targetPos + Vector3.up * (stampHeight * 0.3f);
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(targetPos, returnPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 범위 원 제거
        if (areaIndicator != null) Destroy(areaIndicator);

        isStamping = false;
    }

    /// <summary>
    /// 살아있는 광석 중 하나의 위치를 랜덤으로 반환.
    /// 없으면 null 반환.
    /// </summary>
    private Vector2? GetRandomOrePosition()
    {
        OreNode[] ores = FindObjectsOfType<OreNode>();
        List<OreNode> aliveOres = new List<OreNode>();

        foreach (var ore in ores)
        {
            if (ore != null && ore.hp > 0) // OreNode에 currentHp 필드 있다고 가정
                aliveOres.Add(ore);
        }

        if (aliveOres.Count == 0)
            return null; // 살아있는 광석 없음

        OreNode targetOre = aliveOres[Random.Range(0, aliveOres.Count)];
        return targetOre.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
