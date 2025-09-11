using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [Header("UI Elements")]
    public RectTransform panel;        // 툴팁 패널
    public Canvas rootCanvas;          // 반드시 Canvas 할당
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI costText;

    [Header("애니메이션 설정")]
    public float fadeDuration = 0.2f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        Instance = this;
        HideImmediate();
    }

    public void Show(string title, int level, string info, string cost, Vector3 worldPos)
    {
        titleText.text = title;
        // levelText.text = $"Lv. {level}";
        infoText.text = info;
        costText.text = $"Cost: {cost}";

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        // Overlay 모드니까 그냥 position 사용
        panel.position = screenPos;

        // 좌/우 피벗
        float pivotX = screenPos.x < Screen.width / 2 ? 0f : 1f;
        panel.pivot = new Vector2(pivotX, 0.5f);

        panel.gameObject.SetActive(true);

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(1f));
    }

    public void Hide()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade(0f));
    }

    public void HideImmediate()
    {
        panel.gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        panel.gameObject.SetActive(true);

        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (Mathf.Approximately(targetAlpha, 0f))
            panel.gameObject.SetActive(false);
    }
}
