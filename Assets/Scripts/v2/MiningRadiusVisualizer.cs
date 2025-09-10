using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MiningRadiusVisualizer : MonoBehaviour
{
    public float radius = 1.2f;
    public int segments = 60; // 원 매끄럽게 만들기

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = segments;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.useWorldSpace = false; // 오브젝트 중심 기준
        UpdateCircle();
    }

    public void SetRadius(float r)
    {
        radius = r;
        UpdateCircle();
    }

    void UpdateCircle()
    {
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0));
            angle += 360f / segments;
        }
    }
}
