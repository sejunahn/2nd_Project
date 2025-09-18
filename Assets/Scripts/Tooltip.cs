using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [Header("UI Elements")]
    public RectTransform panel;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI costText;

    [Header("애니메이션 설정")]
    public float fadeDuration = 0.2f;

    [Header("위치 설정")]
    public Vector2 offset = new Vector2(30f, 20f); //테스트용 기본값

    private Coroutine fadeCoroutine;

    void Awake()
    {
        Instance = this;
        HideImmediate();
    }

    public void Show(string title, int level, string info, string cost, Vector3 worldPos)
    {
        titleText.text = title;
        infoText.text = info;
        costText.text = $"Cost: {cost}";

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        bool isLeftSide = screenPos.x < Screen.width / 2;
        float pivotX = isLeftSide ? 0f : 1f;
        panel.pivot = new Vector2(pivotX, 0.5f);

        Vector2 finalPos = screenPos;
        finalPos.x += isLeftSide ? offset.x : -offset.x;
        finalPos.y += offset.y;

        panel.position = finalPos;

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
