using UnityEngine;
using System.Collections;

public class OreSpawner : MonoBehaviour
{
    public IsoGridGenerator grid;
    public GameObject orePrefab;

    [Header("Spawn Settings")]
    public float minSpacing = 0.3f; // 광석 간 최소 거리

    public LayerMask oreLayer;

    void Start()
    {
        if (grid == null) grid = FindObjectOfType<IsoGridGenerator>();
        SpawnInitial();
        StartCoroutine(RespawnLoop());
    }

    void SpawnInitial()
    {
        int n = Mathf.Min(StatManager.Instance.initCount, grid.tileCenters.Count);
        for (int i = 0; i < n; i++) TrySpawnRandom();
    }

    IEnumerator RespawnLoop()
    {
        var wait = new WaitForSeconds(StatManager.Instance.oreRespawnTime);
        while (true)
        {
            // 자식 수로 현재 스폰량 추적
            if (transform.childCount < StatManager.Instance.maxCount)
                TrySpawnRandom();

            yield return wait;
        }
    }

    void TrySpawnRandom()
    {
        if (orePrefab == null || grid == null) return;

        SpriteRenderer oreSR = orePrefab.GetComponent<SpriteRenderer>();
        float oreWidth = oreSR != null ? oreSR.bounds.size.x : 1f;
        float oreHeight = oreSR != null ? oreSR.bounds.size.y : 1f;

        float margin = 0.01f; // 살짝 여유

        for (int tries = 0; tries < 50; tries++)
        {
            int idx = Random.Range(0, grid.tileCenters.Count);
            Vector2 tileCenter = grid.tileCenters[idx];

            float tileHalfW = grid.tileWidth / 2f - margin;
            float tileHalfH = grid.tileHeight / 2f - margin;
            float maxX = tileHalfW - oreWidth / 2f;
            float maxY = tileHalfH - oreHeight / 2f;

            for (int innerTries = 0; innerTries < 20; innerTries++)
            {
                float x = Random.Range(-maxX, maxX);
                float y = Random.Range(-maxY, maxY);

                Vector2[] corners = new Vector2[]
                {
                new Vector2(x - oreWidth/2f, y - oreHeight/2f),
                new Vector2(x + oreWidth/2f, y - oreHeight/2f),
                new Vector2(x - oreWidth/2f, y + oreHeight/2f),
                new Vector2(x + oreWidth/2f, y + oreHeight/2f)
                };

                bool allInside = true;
                foreach (var corner in corners)
                {
                    float relX = corner.x / tileHalfW;
                    float relY = corner.y / tileHalfH;
                    if (Mathf.Abs(relX) + Mathf.Abs(relY) > 1f)
                    {
                        allInside = false;
                        break;
                    }
                }

                if (!allInside) continue;

                Vector2 pos = tileCenter + new Vector2(x, y);

                if (Physics2D.OverlapCircle(pos, minSpacing, oreLayer) == null)
                {
                    var ore = Instantiate(orePrefab, pos, Quaternion.identity, transform);
                    ore.name = $"Ore_{transform.childCount}";
                    return;
                }
            }
        }
    }
}