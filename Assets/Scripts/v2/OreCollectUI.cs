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
        public GameObject prefab;         // ���ư��� ������ ������ (UI Image)
        public RectTransform targetIcon;  // ��ǥ UI ��ġ
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
    /// ���� ��ǥ �� ĵ���� ���� ��ǥ ��ȯ
    /// �׻� Canvas ���� ���� ��ǥ ��ȯ
    /// </summary>
    Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // ���� �� ��ũ��
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        // ��ũ�� �� ĵ���� ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 localPoint);

        return localPoint;
    }

    public void SpawnFlyingIcon(OreType type, Vector3 worldPos)
    {
        if (!oreUIDict.ContainsKey(type)) return;
        OreUIData data = oreUIDict[type];
        if (data.prefab == null || data.targetIcon == null || canvas == null) return;

        // 1) ���� ��ġ
        Vector2 startLocal = WorldToCanvasLocal(worldPos);

        // 2) ��ǥ ��ġ (ĵ���� ���� ����)
        Vector3 targetWorldPos = data.targetIcon.position;
        Vector2 targetLocal = canvas.transform.InverseTransformPoint(targetWorldPos);

        // 3) ������ ����
        GameObject icon = Instantiate(data.prefab, canvas.transform);
        RectTransform iconTr = icon.GetComponent<RectTransform>();
        if (iconTr == null)
        {
            Debug.LogError("[OreCollectUI] prefab�� RectTransform�� �����ϴ�.");
            Destroy(icon);
            return;
        }

        iconTr.pivot = new Vector2(0.5f, 0.5f);
        iconTr.anchorMin = iconTr.anchorMax = new Vector2(0.5f, 0.5f);
        iconTr.localScale = Vector3.one;
        iconTr.anchoredPosition = startLocal;

        // UI Image ���� ũ���
        Image img = icon.GetComponent<Image>();
        if (img != null)
        {
            img.SetNativeSize();
            iconTr.sizeDelta = new Vector2(30f, 30f); // ���ϴ� ũ��
        }

        // 4) �̵�
        StartCoroutine(FlyToTarget(iconTr, targetLocal));
    }

    IEnumerator FlyToTarget(RectTransform iconTr, Vector2 end)
    {
        Vector2 start = iconTr.anchoredPosition;
        float t = 0f;

        // ������: ���۰� ���� �߰� + ����
        Vector2 control = (start + end) * 0.5f + Vector2.up * bezierHeight;

        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            // 2�� ������
            Vector2 p1 = Vector2.Lerp(start, control, ease);
            Vector2 p2 = Vector2.Lerp(control, end, ease);
            iconTr.anchoredPosition = Vector2.Lerp(p1, p2, ease);

            yield return null;
        }

        iconTr.anchoredPosition = end;
        Destroy(iconTr.gameObject);
    }
}
