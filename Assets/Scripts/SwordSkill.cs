using UnityEngine;
using System.Collections;

public class SwordSkill : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingInterval = 1f;     // 몇 초마다 발동
    public float swingAngle = 120f;      // 휘두르는 각도
    public float swingDuration = 0.3f;   // 휘두르는 속도
    public float hitRadius = 2.5f;       // 타격 범위 반경
    public float damage = 20000f;        // 데미지
    public LayerMask targetLayer;        // 타격 대상 레이어 (Ore)

    [Header("Area Visual")]
    public GameObject areaIndicatorPrefab; // 범위 보여줄 프리팹 (투명 원)
    private GameObject areaIndicator;

    [Header("Dependencies")]
    public IsoGridGenerator gridGenerator; // 맵 크기 가져오기 (Inspector 연결 필요)

    private bool isSwinging = false;
    private SpriteRenderer swordRenderer;  // 칼 스프라이트 숨기기용

    void Awake()
    {
        swordRenderer = GetComponent<SpriteRenderer>();
        if (swordRenderer != null) swordRenderer.enabled = false; // 처음엔 안보이게
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

            // 맵 전체 랜덤 좌표
            Vector2 spawnPos = GetRandomMapPosition();
            transform.position = spawnPos;

            // 칼 보이게
            if (swordRenderer != null) swordRenderer.enabled = true;

            yield return StartCoroutine(SwingRoutine());

            // 칼 숨기기
            if (swordRenderer != null) swordRenderer.enabled = false;
        }
    }

    IEnumerator SwingRoutine()
    {
        if (isSwinging) yield break;
        isSwinging = true;

        // 범위 표시 생성
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

            // 타격 처리
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

        // 범위 표시 제거
        if (areaIndicator != null) Destroy(areaIndicator);

        isSwinging = false;
    }

    private Vector2 GetRandomMapPosition()
    {
        if (gridGenerator == null || gridGenerator.tileCenters.Count == 0)
        {
            // 기본 fallback (화면 내 랜덤)
            return new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f));
        }

        // 타일 좌표 중 랜덤 하나 뽑기
        return gridGenerator.tileCenters[Random.Range(0, gridGenerator.tileCenters.Count)];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, hitRadius);
    }
}
