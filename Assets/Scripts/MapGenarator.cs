// 1. MapGenerator.cs - 맵과 광물 생성 관리
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 40;
    public int mapHeight = 40;
    public Tilemap tilemap;
    public TileBase grassTile;
    
    [Header("Mineral Settings")]
    public int mineralCount = 15; // 스테이지당 광물 수
    public GameObject mineralPrefab;
    public List<Mineral> spawnedMinerals = new List<Mineral>();
    
    void Start()
    {
        GenerateMap();
        SpawnMinerals();
    }
    
    void GenerateMap()
    {
        // 40x40 타일맵 생성
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                tilemap.SetTile(position, grassTile);
            }
        }
    }
    
    void SpawnMinerals()
    {
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        
        for (int i = 0; i < mineralCount; i++)
        {
            Vector2Int randomPos;
            do
            {
                randomPos = new Vector2Int(
                    Random.Range(0, mapWidth),
                    Random.Range(0, mapHeight)
                );
            } while (usedPositions.Contains(randomPos));
            
            usedPositions.Add(randomPos);
            
            // 아이소메트릭 월드 좌표로 변환
            Vector3 worldPos = tilemap.CellToWorld(new Vector3Int(randomPos.x, randomPos.y, 0));
            
            GameObject mineralObj = Instantiate(mineralPrefab, worldPos, Quaternion.identity);
            Mineral mineral = mineralObj.GetComponent<Mineral>();
            spawnedMinerals.Add(mineral);
        }
    }
    
    public void RemoveMineral(Mineral mineral)
    {
        spawnedMinerals.Remove(mineral);
        Destroy(mineral.gameObject);
    }
}