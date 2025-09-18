using System;
using UnityEngine;

public class MinerController : MonoBehaviour
{    
    public Camera cam;     
    public LayerMask oreLayer;
    public SpriteRenderer radiusVisual; // 원형 SpriteRenderer
    [Range(0.5f, 2f)]
    public float visualScaleFix = 1f; // 보정 값

    private Collider2D[] buffer = new Collider2D[32];

    void Start()
    {
        if (cam == null) 
            cam = Camera.main;

        if (radiusVisual != null)        
            radiusVisual.color = new Color(1f, 1f, 0f, 0.25f);
        
        UpdateRadiusVisual();
    }

    void Update()
    {
        // 마우스 월드 좌표
        Vector3 mp = Input.mousePosition;
        mp.z = Mathf.Abs(cam.transform.position.z); // 카메라 거리 반영
        Vector3 world = cam.ScreenToWorldPoint(mp);
        world.z = 0f;
        transform.position = world;

        // 반경 내 광석에 데미지
        //int count = Physics2D.OverlapCircleNonAlloc(world, StatManager.Instance.miningRadius, buffer, oreLayer);
        //
        buffer = Physics2D.OverlapCircleAll(world, StatManager.Instance.miningRadius, oreLayer);
        //

        float damage = StatManager.Instance.miningDPS * Time.deltaTime;

        //for (int i = 0; i < count; i++)
        //{
        //    if (buffer[i] == null) continue;
        //    var ore = buffer[i].GetComponent<OreNode>();
        //    if (ore != null) ore.TakeDamage(damage);
        //}

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == null) continue;
            var ore = buffer[i].GetComponent<OreNode>();
            if (ore != null) ore.TakeDamage(damage);
        }


        // 반경 시각화 갱신
        UpdateRadiusVisual();
    }

    void UpdateRadiusVisual()
    {
        if (radiusVisual != null && radiusVisual.sprite != null)
        {
            float spriteSize = radiusVisual.sprite.bounds.size.x;
            float targetSize = StatManager.Instance.miningRadius * 2f;
            float scaleFactor = (targetSize / spriteSize) * visualScaleFix;

            radiusVisual.transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
