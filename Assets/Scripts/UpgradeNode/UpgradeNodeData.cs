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
    // 이후 추가되는건 여기서 쭉쭉
}

[System.Serializable]
public class UpgradeNodeData
{
    //업그레이드 인덱스
    public int Index;
    public UpgradeType upgradeType;
    
    public float value = 1f;   // 증가/감소 수치
    public int oreIndex = 0;   // UnlockOre용
    
    //툴팁정보
    public string title = "";
    public string description;
    public string costInfo = "";
    
    //업그레이드의 카운트값
    public int upgradeCount;
    public int upgradeMaxCount;
    
    //TODO: 추후 value, upgradeType 과 upgradeCount를 조합해서 실제 스탯에 곱해주면 될듯.
}
