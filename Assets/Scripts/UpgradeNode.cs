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
        if (!CanUpgrade())
            return;
        if (!CheckAndSpendCost())
            return;

        nodeData.upgradeCount++;
        StatManager.Instance.UpdateNode(nodeData);
        
        UpdateVisuals();
    }

    private bool CanUpgrade()
    {
        // 최대치 도달 확인
        if (nodeData.upgradeCount >= nodeData.upgradeMaxCount)
            return false;
        return true;
    }

    private bool CheckAndSpendCost()
    {
        if (nodeData.upgradeValues == null || nodeData.upgradeValues.Length == 0)
            return true;
        int[] requiredResources = new int[5]; // stone, iron, copper, silver, gold
        
        foreach (var cost in nodeData.upgradeValues)
        {
            if (cost.oreIndex >= 0 && cost.oreIndex < requiredResources.Length)
            {
                requiredResources[cost.oreIndex] = cost.oreValue;
            }
        }

        return StatManager.Instance.SpendResources(
            requiredResources[0],
            requiredResources[1],
            requiredResources[2],
            requiredResources[3],
            requiredResources[4]
        );
    }

    private void ApplyUpgrade()
    {
        StatManager.Instance.UpdateNode(nodeData);
    }

    public void UpdateVisuals()
    {
        if (frameImage == null || backImage == null || iconImage == null) return;

        Color frameColor = lockedColor;
        Color contentColor = lockedColor;

        if (CanUpgrade())
        {
            if (HasEnoughResources())
            {
                frameColor = availableColor;
                contentColor = availableColor;
            }
            else
            {
                frameColor = Color.red;
                contentColor = availableColor;
            }
        }
        else if (nodeData.upgradeCount > 0)
        {
            if (nodeData.upgradeCount >= nodeData.upgradeMaxCount)
            {
                frameColor = maxedColor;
                contentColor = availableColor;
            }
            else
            {
                frameColor = upgradedColor;
                contentColor = availableColor;
            }
        }

        frameImage.color = frameColor;
        backImage.color = contentColor;
        iconImage.color = contentColor;

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

    public void SyncWithStatManager()
    {
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