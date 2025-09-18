using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsoGridGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 14;
    public int cols = 14;
    public float tileWidth = 1f;
    public float tileHeight = 0.5f;
    public GameObject tilePrefab;
    public Vector2 origin = Vector2.zero;

    [HideInInspector] public List<Vector2> tileCenters = new();
    public Transform tilesParent;

    [Header("Animation")]
    public float spawnHeightOffset = 1f;   // 시작 높이
    public float dropDuration = 0.2f;      // 내려오는 시간
    public float bounceHeight = 0.2f;      // 덜컹 높이
    public float bounceDuration = 0.1f;    // 덜컹 시간
    public float waveDelay = 0.05f;        // 줄 간 파도 간격

    void Awake()
    {
        StartCoroutine(GenerateWave());
    }

    IEnumerator GenerateWave()
    {
        if (tilesParent == null)
            tilesParent = new GameObject("Tiles").transform;

        tileCenters.Clear();

        // 👉 한 줄 단위 파도 효과
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector2 pos = GridToWorld(r, c);
                tileCenters.Add(pos);

                if (tilePrefab != null)
                {
                    var tile = Instantiate(tilePrefab, pos, Quaternion.identity, tilesParent);
                    tile.name = $"Tile_{r}_{c}";

                    // 생성 애니메이션 시작
                    StartCoroutine(SpawnAnimation(tile.transform, pos));
                }
            }

            // 다음 행으로 넘어가기 전에 딜레이 → 파도처럼 보이게
            yield return new WaitForSeconds(waveDelay);
        }
    }

    IEnumerator SpawnAnimation(Transform tile, Vector2 targetPos)
    {
        Vector3 startPos = new Vector3(targetPos.x, targetPos.y + spawnHeightOffset, 0f);
        Vector3 endPos = new Vector3(targetPos.x, targetPos.y, 0f);

        tile.position = startPos;

        // 내려오는 구간
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;
            tile.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // 덜컹 (튀어오르기)
        Vector3 bounceUp = endPos + Vector3.up * bounceHeight;
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / bounceDuration;
            tile.position = Vector3.Lerp(endPos, bounceUp, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        tile.position = endPos; // 최종 고정
    }

    public Vector2 GridToWorld(int r, int c)
    {
        float x = origin.x + (c - r) * (tileWidth * 0.5f);
        float y = origin.y + (c + r) * (tileHeight * 0.5f);
        return new Vector2(x, y);
    }

    public Vector2Int WorldToGrid(Vector2 world)
    {
        float cx = (world.x - origin.x) / (tileWidth * 0.5f);
        float cy = (world.y - origin.y) / (tileHeight * 0.5f);
        int c = Mathf.RoundToInt((cx + cy) * 0.5f);
        int r = Mathf.RoundToInt((cy - cx) * 0.5f);
        return new Vector2Int(r, c);
    }
}
