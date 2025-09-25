using System;
using UnityEngine;

public class MinerController : MonoBehaviour
{    
    public Camera cam;     
    public LayerMask oreLayer;
    public SpriteRenderer radiusVisual;
    public float visualScaleFix = 1f;
    private Collider2D[] buffer = new Collider2D[32];

    public bool isActive = false;

    public void ActiveController()
    {
        isActive = true;
    }

    public void DeactiveController()
    {
        isActive = false;
    }
    
    void Awake()
    {
        if (cam == null) 
            cam = Camera.main;

        if (radiusVisual != null)        
            radiusVisual.color = new Color(1f, 1f, 0f, 0.25f);
        
        UpdateRadiusVisual();
    }

    void Update()
    {
        if (isActive)
        {
            Vector3 mp = Input.mousePosition;
            mp.z = Mathf.Abs(cam.transform.position.z);
            Vector3 world = cam.ScreenToWorldPoint(mp);
            world.z = 0f;
            transform.position = world;

            buffer = Physics2D.OverlapCircleAll(world, StatManager.Instance.miningRadius, oreLayer);
           
            float damage = StatManager.Instance.miningDPS * Time.deltaTime;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == null) continue;
                var ore = buffer[i].GetComponent<OreNode>();
                if (ore != null) ore.TakeDamage(damage);
            }

            UpdateRadiusVisual();   
        }
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
