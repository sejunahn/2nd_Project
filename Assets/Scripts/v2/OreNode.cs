using UnityEngine;
using System.Collections;

public class OreNode : MonoBehaviour
{
    [Header("HP")]
    public float maxHpMin = 20f;
    public float maxHpMax = 60f;

    [Header("Drop")]
    public int yieldAmount = 1;

    [Header("Pickaxe Effect")]
    public GameObject pickaxePrefab;
    public Vector2 pickaxeOffset = new Vector2(1f, 1f); // 광석 기준 위치
    public float swingAngle = 90f;       // 내려찍는 각도
    public float downDuration = 0.05f;   // 내려찍는 속도
    public float hitPause = 0.05f;       // 찍고 멈추는 시간
    public float upDuration = 0.15f;     // 복귀 속도

    private float hp;
    private GameObject pickaxeObj;
    private Transform pickaxeTr;
    private bool isMouseOver = false;
    private bool isSwinging = false;

    void Awake()
    {
        if (pickaxePrefab != null)
        {
            pickaxeObj = Instantiate(pickaxePrefab, transform);
            pickaxeObj.transform.localPosition = pickaxeOffset;
            pickaxeTr = pickaxeObj.transform;
            pickaxeObj.SetActive(false);
        }
    }

    void OnEnable()
    {
        hp = Random.Range(maxHpMin, maxHpMax);

        if (pickaxeObj != null)
        {
            pickaxeObj.SetActive(false);
            pickaxeTr.localEulerAngles = Vector3.zero;
        }
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        float distance = Vector2.Distance(transform.position, mouseWorld);

        if (distance <= StatManager.Instance.miningRadius)
        {
            if (!isMouseOver)
            {
                isMouseOver = true;
                if (pickaxeObj != null) pickaxeObj.SetActive(true);
            }

            TakeDamage(StatManager.Instance.miningDPS * Time.deltaTime);
        }
        else
        {
            if (isMouseOver)
            {
                isMouseOver = false;
                if (pickaxeObj != null) pickaxeObj.SetActive(false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (hp <= 0f) return;

        hp -= amount;

        if (pickaxeObj != null && pickaxeObj.activeSelf && !isSwinging)
        {
            StartCoroutine(SwingPickaxe());
        }

        if (hp <= 0f) Deplete();
    }

    IEnumerator SwingPickaxe()
    {
        isSwinging = true;

        float targetAngle = -swingAngle; // -90 → -120 같은 값 추천

        // 🔽 빠르게 내려찍기
        float timer = 0f;
        while (timer < downDuration)
        {
            float t = timer / downDuration;
            // 곡선 느낌: Slerp보다 Lerp + EaseOut 느낌
            float easedT = 1f - Mathf.Pow(1f - t, 3f); // easeOut cubic
            float angle = Mathf.Lerp(0f, targetAngle, easedT);
            pickaxeTr.localEulerAngles = new Vector3(0f, 0f, angle);

            timer += Time.deltaTime;
            yield return null;
        }
        pickaxeTr.localEulerAngles = new Vector3(0f, 0f, targetAngle);

        // ⏸ 꽝! 찍고 잠깐 멈춤
        yield return new WaitForSeconds(hitPause);

        // 🔼 원위치로 복귀
        timer = 0f;
        while (timer < upDuration)
        {
            float t = timer / upDuration;
            float easedT = t * t; // easeIn quadratic
            float angle = Mathf.Lerp(targetAngle, 0f, easedT);
            pickaxeTr.localEulerAngles = new Vector3(0f, 0f, angle);

            timer += Time.deltaTime;
            yield return null;
        }
        pickaxeTr.localEulerAngles = Vector3.zero;

        isSwinging = false;
    }


    void Deplete()
    {
        var counter = FindObjectOfType<ResourceCounter>();
        if (counter != null) counter.Add(yieldAmount);

        Destroy(gameObject);
    }
}
