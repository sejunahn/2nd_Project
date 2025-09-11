using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum UpgradeType 
    { 
        Radius, 
        DPS, 
        Respawn, 
        UnlockOre, 
        AttackSpeed, 
        Damage 
    }
    public UpgradeType upgradeType;

    [Header("값 설정")]
    public float value = 1f;   // 증가/감소 수치
    public int oreIndex = 0;   // UnlockOre용

    [Header("UI")]
    public Button upgradeButton;
    public Image frameImage;
    public Color upgradedColor = Color.green;

    [Header("툴팁 정보")]
    public string title = "Upgrade Node";
    [TextArea] public string description;
    public string costInfo = "100 Gold";

    private int upgradeCount = 0;

    void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(ApplyUpgrade);
    }

    private void ApplyUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.Radius:
                StatManager.Instance.UpgradeMiningRadius(value);
                break;
            case UpgradeType.DPS:
                StatManager.Instance.UpgradeDPS(value);
                break;
            case UpgradeType.Respawn:
                StatManager.Instance.ReduceRespawnTime(value);
                break;
            case UpgradeType.UnlockOre:
                StatManager.Instance.UnlockOre(oreIndex);
                break;
            case UpgradeType.AttackSpeed:
                // StatManager.Instance.UpgradeAttackSpeed(value);
                break;
            case UpgradeType.Damage:
                // StatManager.Instance.UpgradeDamage(value);
                break;
        }

        upgradeCount++;

        if (frameImage != null)
            frameImage.color = upgradedColor;
    }

    public int GetUpgradeCount()
    {
        return upgradeCount;
    }

    // ===== 툴팁 이벤트 =====
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            RectTransform rt = GetComponent<RectTransform>();
            Tooltip.Instance.Show(
                title,
                GetUpgradeCount(),
                description,
                costInfo,
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
