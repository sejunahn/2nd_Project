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
    public Color unlockedColor = Color.gray;
    
    public void InitNode(UpgradeNodeData nodeData)
    {
        this.nodeData = nodeData;
        SetImages();
    }
    void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(ApplyUpgrade);
    }


    private void ApplyUpgrade()
    {
        switch (nodeData.upgradeType)
        {
            case UpgradeType.Radius:
                StatManager.Instance.UpgradeMiningRadius(nodeData.value);
                break;
            case UpgradeType.DPS:
                StatManager.Instance.UpgradeDPS(nodeData.value);
                break;
            case UpgradeType.Respawn:
                StatManager.Instance.ReduceRespawnTime(nodeData.value);
                break;
            case UpgradeType.UnlockOre:
                StatManager.Instance.UnlockOre(nodeData.oreIndex);
                break;
            case UpgradeType.AttackSpeed:
                // StatManager.Instance.UpgradeAttackSpeed(value);
                break;
            case UpgradeType.Damage:
                // StatManager.Instance.UpgradeDamage(value);
                break;
        }

        nodeData.upgradeCount++;

        SetImages();
    }

    public void SetImages()
    {
        if (frameImage != null)
            iconImage.color = backImage.color = frameImage.color = unlockedColor;

        else if (nodeData.upgradeCount >= nodeData.upgradeMaxCount)
        {
            frameImage.color = Color.yellow;
            iconImage.color = backImage.color= Color.white;
        }
        else
        {
            frameImage.color = upgradedColor;
            iconImage.color = backImage.color= Color.white;
        }
    }

    public int GetUpgradeCount()
    {
        return nodeData.upgradeCount;
    }

    // ===== 툴팁 이벤트 =====
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            RectTransform rt = GetComponent<RectTransform>();
            Tooltip.Instance.Show(
                nodeData.title,
                GetUpgradeCount(),
                nodeData.description,
                nodeData.costInfo,
                rt.position   //transform.position
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
            Tooltip.Instance.Hide();
    }
}
