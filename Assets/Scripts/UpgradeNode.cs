using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UpgradeNodeData nodeData;
    
    [Header("UI")]
    public Button upgradeButton;
    public Image frameImage;
    public Image backImage;
    public Image iconImage;
    public Color upgradedColor = Color.green;
    public Color lockedColor = Color.gray;
    public Color availableColor = Color.white;
    public Color maxedColor = Color.yellow;
    
    public void InitNode(UpgradeNodeData nodeData)
    {
        this.nodeData = nodeData;
        UpdateVisuals();
    }

    void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(TryUpgrade);
    }

    private void TryUpgrade()
    {
        // 업그레이드 가능 여부 체크
        if (!CanUpgrade())
        {
            Debug.LogWarning($"업그레이드 불가능: {nodeData.title}");
            return;
        }

        // 비용 확인 및 차감
        if (!CheckAndSpendCost())
        {
            Debug.LogWarning($"자원 부족: {nodeData.title}");
            return;
        }

        // 업그레이드 적용
        // ApplyUpgrade();
        
        // 업그레이드 카운트 증가
        nodeData.upgradeCount++;
        
        // StatManager에 노드 데이터 업데이트
        StatManager.Instance.UpdateNode(nodeData);
        
        // 시각적 업데이트
        UpdateVisuals();
        
        Debug.Log($"{nodeData.title} 업그레이드 완료! ({nodeData.upgradeCount}/{nodeData.upgradeMaxCount})");
    }

    private bool CanUpgrade()
    {
        // 최대치 도달 확인
        if (nodeData.upgradeCount >= nodeData.upgradeMaxCount)
            return false;

        // // 언락 조건 확인 (UnlockOre 타입의 경우)
        // if (nodeData.upgradeType == UpgradeType.UnlockOre)
        // {
        //     // 이전 광석이 해금되어야 함
        //     if (nodeData.oreIndex > 0 && !StatManager.Instance.IsOreUnlocked(nodeData.oreIndex - 1))
        //         return false;
        // }

        return true;
    }

    private bool CheckAndSpendCost()
    {
        // upgradeValues 배열을 통해 비용 확인
        if (nodeData.upgradeValues == null || nodeData.upgradeValues.Length == 0)
            return true;

        // 필요한 자원들 확인
        int[] requiredResources = new int[5]; // stone, iron, copper, silver, gold
        
        foreach (var cost in nodeData.upgradeValues)
        {
            if (cost.oreIndex >= 0 && cost.oreIndex < requiredResources.Length)
            {
                requiredResources[cost.oreIndex] = cost.oreValue;
            }
        }

        // 자원 소모 시도
        return StatManager.Instance.SpendResources(
            requiredResources[0], // stone
            requiredResources[1], // iron
            requiredResources[2], // copper
            requiredResources[3], // silver
            requiredResources[4]  // gold
        );
    }

    private void ApplyUpgrade()
    {
        StatManager.Instance.UpdateNode(nodeData);
        /*
        switch (nodeData.upgradeType)
        {
            case UpgradeType.Radius:
                StatManager.Instance.UpgradeMiningRadius(nodeData);
                break;
                
            case UpgradeType.DPS:
                StatManager.Instance.UpgradeDPS(nodeData.value);
                break;
                
            case UpgradeType.AttackSpeed:
                // 공격 속도 업그레이드 로직
                break;
                
            case UpgradeType.Damage:
                // 데미지 업그레이드 로직
                break;
                
            case UpgradeType.Respawn:
                StatManager.Instance.ReduceRespawnTime(-nodeData.value); // value가 음수이므로 -를 곱함
                break;
                
            case UpgradeType.UnlockOre:
                StatManager.Instance.UnlockOre(nodeData.oreIndex);
                break;
                
            case UpgradeType.InitCount:
                StatManager.Instance.UpgradeInitCount((int)nodeData.value);
                break;
                
            case UpgradeType.MaxCount:
                StatManager.Instance.UpgradeMaxCount((int)nodeData.value);
                break;
                
            case UpgradeType.UnlockSword:
                StatManager.Instance.UnlockSword();
                break;
                
            case UpgradeType.SwordChance:
                // 검 발동 확률 업그레이드 로직
                break;
                
            case UpgradeType.SwordRange:
                // 검 범위 업그레이드 로직
                break;
        }
        */
    }

    public void UpdateVisuals()
    {
        if (frameImage == null || backImage == null || iconImage == null) return;

        // 기본 상태: 잠김
        Color frameColor = lockedColor;
        Color contentColor = lockedColor;

        if (CanUpgrade())
        {
            // 업그레이드 가능
            if (HasEnoughResources())
            {
                frameColor = availableColor;
                contentColor = availableColor;
            }
            else
            {
                // 자원 부족
                frameColor = Color.red;
                contentColor = availableColor;
            }
        }
        else if (nodeData.upgradeCount > 0)
        {
            if (nodeData.upgradeCount >= nodeData.upgradeMaxCount)
            {
                // 최대치 달성
                frameColor = maxedColor;
                contentColor = availableColor;
            }
            else
            {
                // 업그레이드 중
                frameColor = upgradedColor;
                contentColor = availableColor;
            }
        }

        frameImage.color = frameColor;
        backImage.color = contentColor;
        iconImage.color = contentColor;

        // 버튼 활성화/비활성화
        if (upgradeButton != null)
        {
            upgradeButton.interactable = CanUpgrade() && HasEnoughResources();
        }
    }

    private bool HasEnoughResources()
    {
        if (nodeData.upgradeValues == null || nodeData.upgradeValues.Length == 0)
            return true;

        foreach (var cost in nodeData.upgradeValues)
        {
            if (StatManager.Instance.GetResourceCount(cost.oreIndex) < cost.oreValue)
            {
                return false;
            }
        }
        return true;
    }

    public int GetUpgradeCount()
    {
        return nodeData.upgradeCount;
    }

    // 외부에서 노드 데이터 동기화할 때 사용
    public void SyncWithStatManager()
    {
        // StatManager에서 해당 노드 데이터를 찾아서 동기화
        var savedNode = StatManager.Instance.upgradeNodes.Find(n => n.Index == nodeData.Index);
        if (savedNode != null)
        {
            nodeData.upgradeCount = savedNode.upgradeCount;
            UpdateVisuals();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            RectTransform rt = GetComponent<RectTransform>();
            
            // 현재 비용 정보 생성
            string costInfo = GenerateCostInfo();
            
            Tooltip.Instance.Show(
                nodeData.title,
                GetUpgradeCount(),
                nodeData.description,
                costInfo,
                rt.position
            );
        }
    }

    private string GenerateCostInfo()
    {
        if (nodeData.upgradeValues == null || nodeData.upgradeValues.Length == 0)
            return "무료";

        string[] oreNames = { "stone", "iron", "copper", "silver", "gold" };
        string costText = "";
        
        for (int i = 0; i < nodeData.upgradeValues.Length; i++)
        {
            var cost = nodeData.upgradeValues[i];
            if (i > 0) costText += ", ";
            
            string oreName = cost.oreIndex < oreNames.Length ? oreNames[cost.oreIndex] : "unknown";
            costText += $"{cost.oreValue} {oreName}";
        }
        
        return costText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
            Tooltip.Instance.Hide();
    }
}