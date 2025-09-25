using UnityEngine;
using System.Collections;

public enum OreType
{
    A, B, C, D, E
}

public class OreNode : MonoBehaviour
{
    [Header("HP")]
    public float hp = 20;

    [Header("Drop")]
    public int yieldAmount = 1;

    [Header("Pickaxe Effect")]
    public GameObject pickaxePrefab;
    public Vector2 pickaxeOffset = new Vector2(1f, 1f);
    public float swingAngle = 120f;
    public float downDuration = 0.05f;
    public float hitPause = 0.05f;
    public float upDuration = 0.15f;

    private GameObject pickaxeObj;
    private Transform pickaxeTr;
    private bool isSwinging = false;    

    public OreType oreType;

    public void TakeDamage(float amount)
    {
        if (hp <= 0f) return;

        hp -= amount;

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
