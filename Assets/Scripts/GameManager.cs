using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Button upgradeButton;
    public Text rangeText;
    
    [Header("References")]
    public MouseMiningController miningController;
    
    void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeRange);
    }
    
    void Update()
    {
        UpdateUI();
    }
    
    void UpgradeRange()
    {
        miningController.UpgradeRange();
    }
    
    void UpdateUI()
    {
        if (rangeText != null)
        {
            rangeText.text = $"감지 범위: {miningController.detectionRange:F1}";
        }
    }
}