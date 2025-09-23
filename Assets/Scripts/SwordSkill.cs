using UnityEngine;
using System.Collections;

public class SwordSkill : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingInterval = 1f;     // �� �ʸ��� �ߵ�
    public float swingAngle = 120f;      // �ֵθ��� ����
    public float swingDuration = 0.3f;   // �ֵθ��� �ӵ�
    public float hitRadius = 2.5f;       // Ÿ�� ���� �ݰ�
    public float damage = 20000f;        // ������
    public LayerMask targetLayer;        // Ÿ�� ��� ���̾� (Ore)

    [Header("Area Visual")]
    public GameObject areaIndicatorPrefab; // ���� ������ ������ (���� ��)
    private GameObject areaIndicator;

    [Header("Dependencies")]
    public IsoGridGenerator gridGenerator; // �� ũ�� �������� (Inspector ���� �ʿ�)

    private bool isSwinging = false;
    private SpriteRenderer swordRenderer;  // Į ��������Ʈ ������

    void Awake()
    {
        swordRenderer = GetComponent<SpriteRenderer>();
        if (swordRenderer != null) swordRenderer.enabled = false; // ó���� �Ⱥ��̰�
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
            yield return new WaitForSeconds(swingInterval);

            // �� ��ü ���� ��ǥ
            Vector2 spawnPos = GetRandomMapPosition();
            transform.position = spawnPos;

            // Į ���̰�
            if (swordRenderer != null) swordRenderer.enabled = true;

            yield return StartCoroutine(SwingRoutine());

            // Į �����
            if (swordRenderer != null) swordRenderer.enabled = false;
        }
    }

    IEnumerator SwingRoutine()
    {
        if (isSwinging) yield break;
        isSwinging = true;

        // ���� ǥ�� ����
        if (areaIndicatorPrefab != null)
        {
            areaIndicator = Instantiate(areaIndicatorPrefab, transform.position, Quaternion.identity);
            areaIndicator.transform.localScale = Vector3.one * (hitRadius * 2f);
        }

        float elapsed = 0f;
        float startAngle = -swingAngle / 2f;
        float endAngle = swingAngle / 2f;

        transform.rotation = Quaternion.Euler(0, 0, startAngle);

        while (elapsed < swingDuration)
        {
            float t = elapsed / swingDuration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Ÿ�� ó��
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius, targetLayer);
            foreach (var hit in hits)
            {
                OreNode ore = hit.GetComponent<OreNode>();
                if (ore != null)
                    ore.TakeDamage(damage);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���� ǥ�� ����
        if (areaIndicator != null) Destroy(areaIndicator);

        isSwinging = false;
    }

    private Vector2 GetRandomMapPosition()
    {
        if (gridGenerator == null || gridGenerator.tileCenters.Count == 0)
        {
            // �⺻ fallback (ȭ�� �� ����)
            return new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
        }

        // Ÿ�� ��ǥ �� ���� �ϳ� �̱�
        return gridGenerator.tileCenters[Random.Range(0, gridGenerator.tileCenters.Count)];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
