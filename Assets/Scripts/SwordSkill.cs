using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSkill : MonoBehaviour
{
    [Header("Stamp Settings")]
    public float stampInterval = 1f;    // �� �ʸ��� �ߵ�
    public float stampDuration = 0.2f;  // ������� �ӵ�
    public float returnDuration = 0.1f; // �ö󰡴� �ӵ�
    public float stampHeight = 2f;      // ������ �󸶳� ����������
    public float hitRadius = 2.5f;      // ����
    public float damage = 20000f;       // ������
    public LayerMask targetLayer;       // Ore ���̾�

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

            // ���� ��ġ ��������
            Vector2? spawnPos = GetRandomOrePosition();
            if (spawnPos == null)
                continue; // ������ ������ �ߵ� �� ��

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

        // ���� �� ǥ��
        if (areaIndicatorPrefab != null)
        {
            areaIndicator = Instantiate(areaIndicatorPrefab, transform.position, Quaternion.identity);
            areaIndicator.transform.localScale = Vector3.one * (hitRadius * 2f);
        }

        Vector3 startPos = transform.position + Vector3.up * stampHeight;
        Vector3 targetPos = transform.position;

        // ������ ����
        transform.position = startPos;

        // �������
        float elapsed = 0f;
        while (elapsed < stampDuration)
        {
            float t = elapsed / stampDuration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;

        // �浹 ó�� (�� ���� �� �� ����)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetLayer);
        foreach (var hit in hits)
        {
            OreNode ore = hit.GetComponent<OreNode>();
            if (ore != null)
                ore.TakeDamage(damage);
        }

        // ��¦ �ٽ� �ö󰬴ٰ� �������
        elapsed = 0f;
        Vector3 returnPos = targetPos + Vector3.up * (stampHeight * 0.3f);
        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            transform.position = Vector3.Lerp(targetPos, returnPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���� �� ����
        if (areaIndicator != null) Destroy(areaIndicator);

        isStamping = false;
    }

    /// <summary>
    /// ����ִ� ���� �� �ϳ��� ��ġ�� �������� ��ȯ.
    /// ������ null ��ȯ.
    /// </summary>
    private Vector2? GetRandomOrePosition()
    {
        OreNode[] ores = FindObjectsOfType<OreNode>();
        List<OreNode> aliveOres = new List<OreNode>();

        foreach (var ore in ores)
        {
            if (ore != null && ore.hp > 0) // OreNode�� currentHp �ʵ� �ִٰ� ����
                aliveOres.Add(ore);
        }

        if (aliveOres.Count == 0)
            return null; // ����ִ� ���� ����

        OreNode targetOre = aliveOres[Random.Range(0, aliveOres.Count)];
        return targetOre.transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
