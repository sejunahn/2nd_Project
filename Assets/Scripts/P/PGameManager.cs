using UnityEngine;

public class PGameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject objectPrefab;
    public Transform tileParent;
    public Transform enemyParent;

    public int gridX = 20;
    public int gridY = 20;

    // ȭ�鿡 ���� Ÿ�� ũ��
    public float tileWorldSize = 1f; // 1 ���� = 1 Ÿ��

    void Start()
    {
        // 1. �� ����
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                float posX = (x - gridX / 2f) * tileWorldSize;
                float posY = (y - gridY / 2f) * tileWorldSize + tileWorldSize / 2f; // <-- �̷��� Y ��ġ ����
                GameObject tile = Instantiate(tilePrefab, new Vector2(posX, posY), Quaternion.identity, tileParent);

                // ������ ���߱�
                float spriteSize = tile.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
                float scale = tileWorldSize / spriteSize;
                tile.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        // 2. ������Ʈ ����
        for (int i = 0; i < 5; i++)
        {
            int randX = Random.Range(0, gridX);
            int randY = Random.Range(0, gridY);
            float posX = (randX - gridX / 2f) * tileWorldSize;
            float posY = (randY - gridY / 2f) * tileWorldSize + tileWorldSize / 2f;
            Instantiate(objectPrefab, new Vector2(posX, posY), Quaternion.identity, enemyParent);
        }

        // 3. ī�޶� ���� (�� �Ʒ� ƴ ���ֱ�)
        Camera.main.transform.position = new Vector3(0, 0, -10);
        float mapHeight = gridY * tileWorldSize;
        float mapWidth = gridX * tileWorldSize;
        float screenRatio = (float)Screen.width / Screen.height;
        float mapRatio = mapWidth / mapHeight;

        if (screenRatio >= mapRatio)
            Camera.main.orthographicSize = mapHeight / 2f + 0.5f;
        else
        {
            float difference = mapRatio / screenRatio;
            Camera.main.orthographicSize = mapHeight / 2f * difference + 0.5f;
        }
    }

}
