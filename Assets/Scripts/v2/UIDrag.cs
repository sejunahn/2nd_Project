using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Target Panel")]
    public RectTransform target; // 움직일 전체 패널 (스킬트리 내용)

    [Header("Bounds (public int 조절)")]
    public int minX = -1000;
    public int maxX = 1000;
    public int minY = -1000;
    public int maxY = 1000;

    [Header("Inertia")]
    public float inertiaDamp = 5f; // 감속 비율

    private Vector2 lastMousePos;
    private Vector2 velocity;
    private bool dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePos = eventData.position;
        dragging = true;
        velocity = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target == null) return;

        // 드래그 거리 계산
        Vector2 delta = eventData.position - lastMousePos;
        target.anchoredPosition += delta;

        // 속도 기록 (관성)
        velocity = delta / Time.deltaTime;

        // 위치 제한
        ClampPosition();

        lastMousePos = eventData.position;{}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }

    void Update()
    {
        if (!dragging && velocity.magnitude > 0.1f)
        {
            // 관성 이동
            target.anchoredPosition += velocity * Time.deltaTime;
            velocity = Vector2.Lerp(velocity, Vector2.zero, inertiaDamp * Time.deltaTime);

            // 위치 제한
            ClampPosition();
        }
    }

    private void ClampPosition()
    {
        Vector2 pos = target.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        target.anchoredPosition = pos;
    }

  
}