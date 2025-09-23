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

    private Vector2 lastLocalPoint;
    private Vector2 velocity;
    private bool dragging;
    private RectTransform canvasRectTransform;
    private Canvas canvas;

    private void Awake()
    {
        // 캔버스 찾기
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
        
        // target이 설정되지 않았다면 자동으로 부모를 찾기
        if (target == null)
        {
            target = transform.parent as RectTransform;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (target == null || canvas == null) return;

        // 스크린 좌표를 로컬 좌표로 변환
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, 
            eventData.position, 
            canvas.worldCamera, 
            out localPoint))
        {
            lastLocalPoint = localPoint;
            dragging = true;
            velocity = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target == null || canvas == null || !dragging) return;

        // 현재 마우스 위치를 로컬 좌표로 변환
        Vector2 currentLocalPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, 
            eventData.position, 
            canvas.worldCamera, 
            out currentLocalPoint))
        {
            // 드래그 거리 계산 (로컬 좌표계에서)
            Vector2 delta = currentLocalPoint - lastLocalPoint;
            
            // 캔버스 스케일 고려
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || 
                canvas.renderMode == RenderMode.WorldSpace)
            {
                delta *= canvas.scaleFactor;
            }
            
            target.anchoredPosition += delta;

            // 속도 기록 (관성용)
            if (Time.deltaTime > 0)
            {
                velocity = delta / Time.deltaTime;
            }

            // 위치 제한
            ClampPosition();

            lastLocalPoint = currentLocalPoint;
        }
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
        if (target == null) return;
        
        Vector2 pos = target.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        target.anchoredPosition = pos;
    }

    // 디버그용: 현재 위치 확인
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void OnGUI()
    {
        if (target != null && Application.isEditor)
        {
            GUILayout.Label($"Target Position: {target.anchoredPosition}");
            GUILayout.Label($"Velocity: {velocity}");
            GUILayout.Label($"Dragging: {dragging}");
        }
    }
}