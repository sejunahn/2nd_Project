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
    public float spawnHeightOffset = 1f;
    public float dropDuration = 0.2f;
    public float bounceHeight = 0.2f;
    public float bounceDuration = 0.1f;
    public float totalDuration = 1f;

    public void Init()
    {
        if (tileCenters.Count != 0)
        {
            ClearRemainingTiles();
        }
        
        StartCoroutine(GenerateCenterOut());
    }

    public void ClearRemainingTiles()
    {
        if (tilesParent != null)
        {
            for (int i = tilesParent.childCount - 1; i >= 0; i--)
            {
                Destroy(tilesParent.GetChild(i).gameObject);
            }
        }
    }

    public System.Action OnGridGenerated;

    IEnumerator GenerateCenterOut()
    {
        if (tilesParent == null)
            tilesParent = new GameObject("Tiles").transform;

        tileCenters.Clear();
        int totalTiles = rows * cols;
        float waveDelay = totalDuration / totalTiles;

        List<Vector2Int> coords = new();
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                coords.Add(new Vector2Int(r, c));

        Vector2 center = new Vector2(rows / 2f, cols / 2f);
        coords.Sort((a, b) => Vector2.Distance(a, center).CompareTo(Vector2.Distance(b, center)));

        foreach (var coord in coords)
        {
            Vector2 pos = GridToWorld(coord.x, coord.y);
            tileCenters.Add(pos);

            if (tilePrefab != null)
            {
                var tile = Instantiate(tilePrefab, new Vector3(pos.x, pos.y + spawnHeightOffset, 0), Quaternion.identity, tilesParent);
                tile.name = $"Tile_{coord.x}_{coord.y}";
                StartCoroutine(SpawnAnimation(tile.transform, pos));
            }

            yield return new WaitForSeconds(waveDelay);
        }

        OnGridGenerated?.Invoke();
    }


    IEnumerator SpawnAnimation(Transform tile, Vector2 targetPos)
    {
        tile.gameObject.SetActive(true);

        Vector3 startPos = new Vector3(targetPos.x, targetPos.y + spawnHeightOffset, 0f);
        Vector3 endPos = new Vector3(targetPos.x, targetPos.y, 0f);
        tile.position = startPos;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;
            tile.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        Vector3 bounceUp = endPos + Vector3.up * bounceHeight;
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / bounceDuration;
            tile.position = Vector3.Lerp(endPos, bounceUp, Mathf.Sin(t * Mathf.PI));
            yield return null;
        }

        tile.position = endPos;
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
