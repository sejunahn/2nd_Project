using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class OreCollectUI : MonoBehaviour
{
    public static OreCollectUI Instance;

    [System.Serializable]
    public class OreUIData
    {
        public OreType type;
        public GameObject prefab;
        public RectTransform targetIcon;
        public TMP_Text countText;
    }

    [Header("Ore UI Data")]
    public List<OreUIData> oreUIList = new List<OreUIData>();
    private Dictionary<OreType, OreUIData> oreUIDict;

    [Header("Canvas")]
    public Canvas canvas;

    [Header("Fly Settings")]
    public float flyDuration = 0.35f;
    public float bezierHeight = 120f;
    public Vector2 iconSize = new Vector2(30f, 30f);

    private Dictionary<OreType, int> oreCounts = new Dictionary<OreType, int>();

    void Awake()
    {
        Instance = this;
        oreUIDict = new Dictionary<OreType, OreUIData>();

        foreach (var data in oreUIList)
        {
            if (!oreUIDict.ContainsKey(data.type))
            {
                oreUIDict.Add(data.type, data);
                oreCounts[data.type] = 0;
                if (data.countText != null)
                    data.countText.text = "0";
            }
        }
    }

    Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, cam, out Vector2 localPoint);

        return localPoint;
    }

    public void SpawnFlyingIcon(OreType type, Vector3 worldPos)
    {
        if (!oreUIDict.ContainsKey(type)) return;
        OreUIData data = oreUIDict[type];

        Vector2 startLocal = WorldToCanvasLocal(worldPos);
        Vector2 targetLocal = canvas.transform.InverseTransformPoint(data.targetIcon.position);

        GameObject icon = Instantiate(data.prefab, canvas.transform);
        RectTransform iconTr = icon.GetComponent<RectTransform>();
        iconTr.pivot = new Vector2(0.5f, 0.5f);
        iconTr.anchorMin = iconTr.anchorMax = new Vector2(0.5f, 0.5f);
        iconTr.localScale = Vector3.one;
        iconTr.anchoredPosition = startLocal;
        iconTr.sizeDelta = iconSize;

        StartCoroutine(FlyToTarget(iconTr, targetLocal, type));
    }

    IEnumerator FlyToTarget(RectTransform iconTr, Vector2 end, OreType type)
    {
        Vector2 start = iconTr.anchoredPosition;
        float t = 0f;
        Vector2 control = (start + end) * 0.5f + Vector2.up * bezierHeight;

        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            Vector2 p1 = Vector2.Lerp(start, control, ease);
            Vector2 p2 = Vector2.Lerp(control, end, ease);
            iconTr.anchoredPosition = Vector2.Lerp(p1, p2, ease);

            yield return null;
        }

        iconTr.anchoredPosition = end;
        Destroy(iconTr.gameObject);

        oreCounts[type]++;
        OreUIData data = oreUIDict[type];
        if (data.countText != null)
        {
            data.countText.text = oreCounts[type].ToString();
        }
    }
}
