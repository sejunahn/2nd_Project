using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorter : MonoBehaviour
{
    public int offset = 0;
    SpriteRenderer sr;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void LateUpdate()
    {
        if (sr == null) return;
        sr.sortingOrder = offset - Mathf.RoundToInt(transform.position.y * 100f);
    }
}