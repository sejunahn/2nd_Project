using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public Button upgradeButton;
    public Text rangeText;
    
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
    }
    
    void UpdateUI()
    {
        if (rangeText != null)
        {

        }
    }
}