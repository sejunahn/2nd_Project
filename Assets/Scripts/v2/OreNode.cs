using UnityEngine;
using System.Collections;

public enum OreType
{
    A, B, C, D, E
}

public class OreNode : MonoBehaviour
{
    [Header("HP")]
    public float maxHpMin = 20f;
    public float maxHpMax = 60f;

    [Header("Drop")]
    public int yieldAmount = 1;

    [Header("Pickaxe Effect")]
    public GameObject pickaxePrefab;
    public Vector2 pickaxeOffset = new Vector2(1f, 1f);
    public float swingAngle = 120f;
    public float downDuration = 0.05f;
    public float hitPause = 0.05f;
    public float upDuration = 0.15f;

    private float hp;
    private GameObject pickaxeObj;
    private Transform pickaxeTr;
    private bool isSwinging = false;

    

    public OreType oreType;  // 이 광석이 어떤 종류인지 선택

    void Awake()
    {
        hp = Random.Range(maxHpMin, maxHpMax);
    }

    void Update()
    {
        // 마우스 범위 체크는 이제 공격 시 TakeDamage 호출에서 처리하므로 여기선 제거 가능
    }

    public void TakeDamage(float amount)
    {
        if (hp <= 0f) return;

        hp -= amount;

        // 공격을 받고 있으면 곡괭이 생성/활성화
        if (pickaxePrefab != null)
        {
            if (pickaxeObj == null)
            {
                pickaxeObj = Instantiate(pickaxePrefab, transform);
                pickaxeObj.name = "Pickaxe";
                pickaxeTr = pickaxeObj.transform;
                pickaxeTr.localPosition = pickaxeOffset;
            }

            pickaxeObj.SetActive(true);
        }

        if (pickaxeObj != null && pickaxeObj.activeSelf && !isSwinging)
        {
            StartCoroutine(SwingPickaxe());
        }

        if (hp <= 0f) Deplete();
    }

    IEnumerator SwingPickaxe()
    {
        isSwinging = true;

        float timer = 0f;
        float targetAngle = -swingAngle;

        // 내려찍기
        while (timer < downDuration)
        {
            float t = timer / downDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            float angle = Mathf.Lerp(0f, targetAngle, easedT);
            pickaxeTr.localEulerAngles = new Vector3(0f, 0f, angle);
            timer += Time.deltaTime;
            yield return null;
        }
        pickaxeTr.localEulerAngles = new Vector3(0f, 0f, targetAngle);

        yield return new WaitForSeconds(hitPause);

        // 복귀
        timer = 0f;
        while (timer < upDuration)
        {
            float t = timer / upDuration;
            float easedT = t * t;
            float angle = Mathf.Lerp(targetAngle, 0f, easedT);
            pickaxeTr.localEulerAngles = new Vector3(0f, 0f, angle);
            timer += Time.deltaTime;
            yield return null;
        }

        pickaxeTr.localEulerAngles = Vector3.zero;
        isSwinging = false;

        // 공격 종료 시 곡괭이 사라짐
        pickaxeObj.SetActive(false);
    }

    void Deplete()
    {
        if (OreCollectUI.Instance != null)
            OreCollectUI.Instance.SpawnFlyingIcon(oreType, transform.position);

        var counter = FindObjectOfType<ResourceCounter>();
        if (counter != null) counter.Add(yieldAmount);

        Destroy(gameObject);
    }
}
