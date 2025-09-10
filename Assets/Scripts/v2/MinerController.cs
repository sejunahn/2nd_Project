using UnityEngine;

public class MinerController : MonoBehaviour
{
    public Camera cam;
    [Header("Mining")]
    public float miningRadius = 1.2f;
    public float dps = 15f; // 초당 채굴량
    public LayerMask oreLayer;

    [Header("Visual")]
    public Transform cursorVisual; // 원형 스프라이트(선택)

    Collider2D[] buffer = new Collider2D[32];

    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // 마우스 월드 좌표
        Vector3 mp = Input.mousePosition;
        mp.z = 10f; // Orthographic이면 아무 값이나 OK, Perspective면 카메라-평면 거리
        Vector3 world = cam.ScreenToWorldPoint(mp);
        world.z = 0f;
        transform.position = world;

        // 반경 내 광석에 데미지
        int count = Physics2D.OverlapCircleNonAlloc(world, miningRadius, buffer, oreLayer);
        float damage = dps * Time.deltaTime;

        for (int i = 0; i < count; i++)
        {
            if (buffer[i] == null) continue;
            var ore = buffer[i].GetComponent<OreNode>();
            if (ore != null) ore.TakeDamage(damage);
        }

        // 비주얼 반경 갱신
        if (cursorVisual != null)
        {
            cursorVisual.position = world;
            cursorVisual.localScale = Vector3.one * (miningRadius * 2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, miningRadius);
    }
}