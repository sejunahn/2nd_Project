using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OreSpawner : MonoBehaviour
{
    public IsoGridGenerator grid;
    [Header("Ore Prefabs in order (Ore1, Ore2, Ore3, Ore4)")]
    public GameObject[] orePrefabs;  // 배열로 관리

    [Header("Spawn Settings")]
    public float minSpacing = 0.3f; // 광석 간 최소 거리
    public LayerMask oreLayer;

    public void Init()
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
            if (transform.childCount < StatManager.Instance.maxCount)
                TrySpawnRandom();

            yield return wait;
        }
    }

    void TrySpawnRandom()
    {
        GameObject orePrefab = GetRandomUnlockedOre();
        if (orePrefab == null || grid == null) return;

        SpriteRenderer oreSR = orePrefab.GetComponent<SpriteRenderer>();
        float oreWidth = oreSR != null ? oreSR.bounds.size.x : 1f;
        float oreHeight = oreSR != null ? oreSR.bounds.size.y : 1f;

        float margin = 0.01f;

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

    /// <summary>
    /// StatManager의 unlock 상태를 확인해서 랜덤으로 오레 프리팹 하나 반환
    /// </summary>
    GameObject GetRandomUnlockedOre()
    {
        List<GameObject> unlocked = new List<GameObject>();

        if (StatManager.Instance.unlockIron && orePrefabs.Length > 0) unlocked.Add(orePrefabs[0]);
        if (StatManager.Instance.unlockCopper && orePrefabs.Length > 1) unlocked.Add(orePrefabs[1]);
        if (StatManager.Instance.unlockSilver && orePrefabs.Length > 2) unlocked.Add(orePrefabs[2]);
        if (StatManager.Instance.unlockGold && orePrefabs.Length > 3) unlocked.Add(orePrefabs[3]);
        if (StatManager.Instance.unlockGold && orePrefabs.Length > 4) unlocked.Add(orePrefabs[4]);

        if (unlocked.Count == 0) return null;

        return unlocked[Random.Range(0, unlocked.Count)];
    }
}
