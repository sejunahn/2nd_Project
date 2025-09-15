using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OreCollectUI : MonoBehaviour
{
    public static OreCollectUI Instance;

    [System.Serializable]
    public class OreUIData
    {
        public OreType type;
        public GameObject prefab;         // 날아가는 아이콘 프리팹 (UI Image)
        public RectTransform targetIcon;  // 목표 UI 위치
    }

    [Header("Ore UI Data")]
    public List<OreUIData> oreUIList = new List<OreUIData>();
    private Dictionary<OreType, OreUIData> oreUIDict;

    [Header("Canvas")]
    public Canvas canvas;

    [Header("Fly Settings")]
    public float flyDuration = 0.35f;
    public float bezierHeight = 120f;

    void Awake()
    {
        Instance = this;
        oreUIDict = new Dictionary<OreType, OreUIData>();
        foreach (var data in oreUIList)
        {
            if (!oreUIDict.ContainsKey(data.type))
                oreUIDict.Add(data.type, data);
        }
    }

    /// <summary>
    /// 월드 좌표 → 캔버스 로컬 좌표 변환
    /// 항상 Canvas 기준 로컬 좌표 반환
    /// </summary>
    Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // 월드 → 스크린
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        // 스크린 → 캔버스 로컬
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 localPoint);

        return localPoint;
    }

    public void SpawnFlyingIcon(OreType type, Vector3 worldPos)
    {
        if (!oreUIDict.ContainsKey(type)) return;
        OreUIData data = oreUIDict[type];
        if (data.prefab == null || data.targetIcon == null || canvas == null) return;

        // 1) 시작 위치
        Vector2 startLocal = WorldToCanvasLocal(worldPos);

        // 2) 목표 위치 (캔버스 기준 로컬)
        Vector3 targetWorldPos = data.targetIcon.position;
        Vector2 targetLocal = canvas.transform.InverseTransformPoint(targetWorldPos);

        // 3) 아이콘 생성
        GameObject icon = Instantiate(data.prefab, canvas.transform);
        RectTransform iconTr = icon.GetComponent<RectTransform>();
        if (iconTr == null)
        {
            Debug.LogError("[OreCollectUI] prefab에 RectTransform이 없습니다.");
            Destroy(icon);
            return;
        }

        iconTr.pivot = new Vector2(0.5f, 0.5f);
        iconTr.anchorMin = iconTr.anchorMax = new Vector2(0.5f, 0.5f);
        iconTr.localScale = Vector3.one;
        iconTr.anchoredPosition = startLocal;

        // UI Image 원래 크기로
        Image img = icon.GetComponent<Image>();
        if (img != null)
        {
            img.SetNativeSize();
            iconTr.sizeDelta = new Vector2(30f, 30f); // 원하는 크기
        }

        // 4) 이동
        StartCoroutine(FlyToTarget(iconTr, targetLocal));
    }

    IEnumerator FlyToTarget(RectTransform iconTr, Vector2 end)
    {
        Vector2 start = iconTr.anchoredPosition;
        float t = 0f;

        // 제어점: 시작과 끝의 중간 + 높이
        Vector2 control = (start + end) * 0.5f + Vector2.up * bezierHeight;

        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            // 2차 베지어
            Vector2 p1 = Vector2.Lerp(start, control, ease);
            Vector2 p2 = Vector2.Lerp(control, end, ease);
            iconTr.anchoredPosition = Vector2.Lerp(p1, p2, ease);

            yield return null;
        }

        iconTr.anchoredPosition = end;
        Destroy(iconTr.gameObject);
    }
}
