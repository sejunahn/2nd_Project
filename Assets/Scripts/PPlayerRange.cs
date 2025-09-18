using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PPlayerRange : MonoBehaviour
{
    public float attackRadius = 2f;          // 공격 범위
    public int segments = 64;                // 원 라인 세그먼트
    public LayerMask objectLayer;            // 공격 대상 레이어
    public GameObject effectPrefab;          // 공격 이펙트
    public float attackCooldown = 0.5f;      // 공격 쿨타임 (초)

    private LineRenderer lr;
    private float lastAttackTime = 0f;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = segments;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0f, 0.5f, 1f, 0.5f);
        lr.endColor = new Color(0f, 0.5f, 1f, 0.5f);

        UpdateCircle();
    }

    private void Update()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        // 쿨타임 확인 후 공격
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, attackRadius, objectLayer);
            foreach (var hit in hits)
            {
                PObject obj = hit.GetComponent<PObject>();
                if (obj != null)
                {
                    obj.Hit();
                    if (effectPrefab != null)
                        Instantiate(effectPrefab, hit.transform.position, Quaternion.identity);
                }
            }
        }
    }

    // 라인 렌더러로 원 그리기
    private void UpdateCircle()
    {
        Vector3[] points = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * attackRadius;
            float y = Mathf.Sin(angle) * attackRadius;
            points[i] = new Vector3(x, y, 0);
        }
        lr.SetPositions(points);
    }
}
