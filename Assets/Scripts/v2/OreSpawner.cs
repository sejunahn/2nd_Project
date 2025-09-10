using UnityEngine;
using System.Collections;

public class OreSpawner : MonoBehaviour
{
    public IsoGridGenerator grid;
    public GameObject orePrefab;

    [Header("Spawn Settings")]
    public int initialCount = 35;
    public int targetCount = 35;
    public float respawnEvery = 4f;
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
        int n = Mathf.Min(initialCount, grid.tileCenters.Count);
        for (int i = 0; i < n; i++) TrySpawnRandom();
    }

    IEnumerator RespawnLoop()
    {
        var wait = new WaitForSeconds(respawnEvery);
        while (true)
        {
            // 자식 수로 현재 스폰량 추적
            if (transform.childCount < targetCount)
                TrySpawnRandom();

            yield return wait;
        }
    }

    void TrySpawnRandom()
    {
        for (int tries = 0; tries < 20; tries++)
        {
            int idx = Random.Range(0, grid.tileCenters.Count);
            Vector2 pos = grid.tileCenters[idx];

            if (Physics2D.OverlapCircle(pos, minSpacing, oreLayer) == null)
            {
                var ore = Instantiate(orePrefab, pos, Quaternion.identity, transform);
                ore.name = $"Ore_{transform.childCount}";
                return;
            }
        }
    }
}