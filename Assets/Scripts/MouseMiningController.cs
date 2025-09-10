using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseMiningController : MonoBehaviour
{
    [Header("Mining Settings")]
    public float detectionRange = 2f; // 감지 범위
    public float miningInterval = 0.5f; // 0.5초마다 채굴
    
    [Header("Upgrade")]
    public float rangeUpgradeAmount = 0.5f;
    
    [Header("Visual")]
    public LineRenderer rangeIndicator;
    public int circleSegments = 50;
    
    private Camera mainCamera;
    private Vector3 mouseWorldPos;
    private List<Mineral> mineralsInRange = new List<Mineral>();
    private bool isMining = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        SetupRangeIndicator();
        StartCoroutine(MiningRoutine());
    }
    
    void Update()
    {
        UpdateMousePosition();
        UpdateRangeIndicator();
        CheckMineralsInRange();
    }
    
    void UpdateMousePosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.nearClipPlane));
        mouseWorldPos.z = 0;
    }
    
    void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.positionCount = circleSegments + 1;
            
            for (int i = 0; i <= circleSegments; i++)
            {
                float angle = (float)i / circleSegments * 2f * Mathf.PI;
                Vector3 pos = mouseWorldPos + new Vector3(
                    Mathf.Cos(angle) * detectionRange,
                    Mathf.Sin(angle) * detectionRange,
                    0
                );
                rangeIndicator.SetPosition(i, pos);
            }
        }
    }
    
    void CheckMineralsInRange()
    {
        mineralsInRange.Clear();
        
        Mineral[] allMinerals = FindObjectsOfType<Mineral>();
        foreach (Mineral mineral in allMinerals)
        {
            if (mineral != null)
            {
                float distance = Vector3.Distance(mouseWorldPos, mineral.transform.position);
                if (distance <= detectionRange)
                {
                    mineralsInRange.Add(mineral);
                }
            }
        }
        
        isMining = mineralsInRange.Count > 0;
    }
    
    IEnumerator MiningRoutine()
    {
        while (true)
        {
            if (isMining && mineralsInRange.Count > 0)
            {
                // 범위 내 모든 광물을 동시에 채굴
                foreach (Mineral mineral in mineralsInRange.ToArray())
                {
                    if (mineral != null)
                    {
                        mineral.TakeDamage(1);
                    }
                }
            }
            
            yield return new WaitForSeconds(miningInterval);
        }
    }
    
    void SetupRangeIndicator()
    {
        if (rangeIndicator == null)
        {
            GameObject rangeObj = new GameObject("RangeIndicator");
            rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        }
        
        rangeIndicator.material = new Material(Shader.Find("Sprites/Default"));
        rangeIndicator.material.color = new Color(1f, 1f, 0f, 0.5f); // 노란색 반투명
        rangeIndicator.startWidth = 0.1f;
        rangeIndicator.endWidth = 0.1f;
        rangeIndicator.useWorldSpace = true;
        rangeIndicator.sortingOrder = 10;
    }
    
    // 업그레이드 함수
    public void UpgradeRange()
    {
        detectionRange += rangeUpgradeAmount;
        Debug.Log($"감지 범위가 {detectionRange}로 증가했습니다!");
    }
    
    // 디버그용
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(mouseWorldPos, detectionRange);
    }
}