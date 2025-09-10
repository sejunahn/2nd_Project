using UnityEngine;
using System.Collections.Generic;

public class IsoGridGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int rows = 14;
    public int cols = 14;
    [Tooltip("한 타일의 월드 단위 가로/세로")]
    public float tileWidth = 1f;
    public float tileHeight = 0.5f;
    public GameObject tilePrefab;
    public Vector2 origin = Vector2.zero;

    [HideInInspector] public List<Vector2> tileCenters = new();
    public Transform tilesParent;

    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        if (tilesParent == null) tilesParent = new GameObject("Tiles").transform;
        tileCenters.Clear();

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
                }
            }
        }
    }

    public Vector2 GridToWorld(int r, int c)
    {
        float x = origin.x + (c - r) * (tileWidth * 0.5f);
        float y = origin.y + (c + r) * (tileHeight * 0.5f);
        return new Vector2(x, y);
    }

    // 선택: 역변환이 필요하면 사용
    public Vector2Int WorldToGrid(Vector2 world)
    {
        float cx = (world.x - origin.x) / (tileWidth * 0.5f);
        float cy = (world.y - origin.y) / (tileHeight * 0.5f);
        int c = Mathf.RoundToInt((cx + cy) * 0.5f);
        int r = Mathf.RoundToInt((cy - cx) * 0.5f);
        return new Vector2Int(r, c);
    }
}