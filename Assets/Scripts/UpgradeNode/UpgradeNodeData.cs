using UnityEngine;
public enum UpgradeType
{ 
    Radius, 
    DPS, 
    AttackSpeed, 
    Damage ,
    Respawn, 
    UnlockOre, 
    OreInitCount,
    OreMaxCount,
    //
    UnlockSword,
    SwordChance,
    SwordRange,
    // 이후 추가되는건 여기서
}

[System.Serializable]
public class UpgradeNodeData
{
    //어떤 업그레이드인지
    public int Index;
    public UpgradeType upgradeType;
    
    //private 
    [Header("값 설정")]
    public float value = 1f;   // 증가/감소 수치
    public int oreIndex = 0;   // UnlockOre용
    
    [Header("툴팁 정보")]
    public string title = "";
    public string description;
    public string costInfo = "";
    
    //업그레이드의 횟수
    public int upgradeCount;
    public int upgradeMaxCount;
}
