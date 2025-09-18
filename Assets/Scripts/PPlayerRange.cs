using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PPlayerRange : MonoBehaviour
{
    public float attackRadius = 2f;          // ���� ����
    public int segments = 64;                // �� ���� ���׸�Ʈ
    public LayerMask objectLayer;            // ���� ��� ���̾�
    public GameObject effectPrefab;          // ���� ����Ʈ
    public float attackCooldown = 0.5f;      // ���� ��Ÿ�� (��)

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
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        // ��Ÿ�� Ȯ�� �� ����
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

    // ���� �������� �� �׸���
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
