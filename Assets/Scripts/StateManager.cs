using System.Collections.Generic;
using UnityEngine;

public class StatManager
{
    private static StatManager _instance;
    public static StatManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new StatManager();
            return _instance;
        }
    }

    private StatManager()
    {
        // 기본값만 설정 (업그레이드는 Init에서 계산)
        SetBaseValues();
    }

    // === 베이스 값들 (업그레이드 전 기본값) ===
    private readonly float baseMiningRadius = 1.2f;
    private readonly float baseMiningDPS = 15f;
    private readonly float baseOreRespawnTime = 3f;
    private readonly int baseInitCount = 5;
    private readonly int baseMaxCount = 20;
    private readonly float baseTimer = 60f;

    // === 현재 값들 (베이스 + 업그레이드 적용된 최종값) ===
    public float Timer;
    public float miningRadius;
    public float miningDPS;
    public float oreRespawnTime;
    public int initCount;
    public int maxCount;

    // === 자원 및 해금 상태 (저장/로드 대상) ===
    public int stoneBar;
    public int ironBar;
    public int copperBar;
    public int silverBar;
    public int goldBar;

    public bool unlockIron;
    public bool unlockCopper;
    public bool unlockSilver;
    public bool unlockGold;
    public bool unlockSword;

    public List<UpgradeNodeData> upgradeNodes = new List<UpgradeNodeData>();

    private void SetBaseValues()
    {
        Timer = baseTimer;
        miningRadius = baseMiningRadius;
        miningDPS = baseMiningDPS;
        oreRespawnTime = baseOreRespawnTime;
        initCount = baseInitCount;
        maxCount = baseMaxCount;

        // 자원은 0으로 시작
        stoneBar = 0;
        ironBar = 0;
        copperBar = 0;
        silverBar = 0;
        goldBar = 0;

        // 해금 상태는 false로 시작
        unlockIron = false;
        unlockCopper = false;
        unlockSilver = false;
        unlockGold = false;
        unlockSword = false;

        upgradeNodes.Clear();
    }

    /// <summary>
    /// 저장된 데이터 로드 및 StatManager에 적용
    /// </summary>
    public void Init()
    {
        NodeDataListWrapper wrapper = UpgradeNodeDataIO.LoadWrapper();
        
        // 1. 자원과 해금 상태만 로드
        LoadResourcesAndUnlocks(wrapper);
        
        // 2. 노드 데이터 로드
        upgradeNodes = wrapper.items ?? new List<UpgradeNodeData>();
        
        // 3. 베이스 값에서 시작해서 모든 업그레이드 다시 계산
        RecalculateAllStats();
        
        Debug.Log($"StatManager 초기화 완료 - Radius: {miningRadius}, DPS: {miningDPS}");
    }

    /// <summary>
    /// 자원과 해금 상태만 wrapper에서 로드
    /// </summary>
    private void LoadResourcesAndUnlocks(NodeDataListWrapper wrapper)
    {
        stoneBar = wrapper.stoneBarCount;
        ironBar = wrapper.ironBarCount;
        copperBar = wrapper.copperBarCount;
        silverBar = wrapper.silverBarCount;
        goldBar = wrapper.goldBarCount;
        
        unlockIron = wrapper.unlockIron;
        unlockCopper = wrapper.unlockCopper;
        unlockSilver = wrapper.unlockSilver;
        unlockGold = wrapper.unlockGold;
        unlockSword = wrapper.unlockSword;
    }

    /// <summary>
    /// 베이스 값에서 시작해서 모든 업그레이드 효과를 다시 계산
    /// </summary>
    private void RecalculateAllStats()
    {
        // 1. 베이스 값으로 리셋
        Timer = baseTimer;
        miningRadius = baseMiningRadius;
        miningDPS = baseMiningDPS;
        oreRespawnTime = baseOreRespawnTime;
        initCount = baseInitCount;
        maxCount = baseMaxCount;

        // 2. 모든 업그레이드된 노드의 효과를 순차적으로 적용
        foreach (var node in upgradeNodes)
        {
            if (node.upgradeCount > 0)
            {
                ApplyNodeEffectMultipleTimes(node);
            }
        }

        Debug.Log($"스탯 재계산 완료 - DPS: {miningDPS}, Radius: {miningRadius}, Respawn: {oreRespawnTime}");
    }

    /// <summary>
    /// 특정 노드의 효과를 업그레이드 횟수만큼 적용
    /// </summary>
    private void ApplyNodeEffectMultipleTimes(UpgradeNodeData node)
    {
        for (int i = 0; i < node.upgradeCount; i++)
        {
            ApplyNodeEffect(node);
        }
    }

    /// <summary>
    /// 개별 노드 효과 적용 (한 번)
    /// </summary>
    private void ApplyNodeEffect(UpgradeNodeData node)
    {
        switch (node.upgradeType)
        {
            case UpgradeType.Radius:
                miningRadius += node.value;
                break;
                
            case UpgradeType.DPS:
                miningDPS += node.value;
                break;
                
            case UpgradeType.Damage:
                // 데미지 로직 (구현 필요)
                break;
                
            case UpgradeType.Respawn:
                oreRespawnTime += node.value; // value는 음수
                break;
                
            case UpgradeType.InitCount:
                initCount += (int)node.value;
                break;
                
            case UpgradeType.MaxCount:
                maxCount += (int)node.value;
                break;
                
            case UpgradeType.AttackSpeed:
                // 공격 속도 로직 (구현 필요)
                break;
                
            // UnlockOre, UnlockSword 등은 이미 로드할 때 적용됨
        }
    }

    /// <summary>
    /// 현재 StatManager 상태를 저장
    /// </summary>
    public void SaveData()
    {
        NodeDataListWrapper wrapper = CreateWrapperFromCurrentData();
        UpgradeNodeDataIO.SaveGameData(wrapper);
        Debug.Log("StatManager 데이터 저장 완료");
    }

    /// <summary>
    /// 현재 StatManager 데이터로 Wrapper 생성
    /// </summary>
    private NodeDataListWrapper CreateWrapperFromCurrentData()
    {
        return new NodeDataListWrapper
        {
            // 자원과 해금 상태만 저장 (계산된 스탯은 저장하지 않음)
            stoneBarCount = stoneBar,
            ironBarCount = ironBar,
            copperBarCount = copperBar,
            silverBarCount = silverBar,
            goldBarCount = goldBar,
            unlockIron = unlockIron,
            unlockCopper = unlockCopper,
            unlockSilver = unlockSilver,
            unlockGold = unlockGold,
            unlockSword = unlockSword,
            items = new List<UpgradeNodeData>(upgradeNodes),
            
            // 베이스 값들은 저장하지 않음 (항상 고정)
            timer = baseTimer,
            miningRadius = baseMiningRadius,
            miningDPS = baseMiningDPS,
            oreRespawnTime = baseOreRespawnTime,
            initCount = baseInitCount,
            maxCount = baseMaxCount
        };
    }

    /// <summary>
    /// 게임 리셋 (기본값으로 초기화)
    /// </summary>
    public void ResetToDefault()
    {
        SetBaseValues();
        Debug.Log("StatManager 기본값으로 리셋 완료");
    }

    #region Resource Methods (변경 없음)

    public void AddBar(int stone = 0, int iron = 0, int copper = 0, int silver = 0, int gold = 0)
    {
        stoneBar += stone;
        ironBar += iron;
        copperBar += copper;
        silverBar += silver;
        goldBar += gold;
    }

    public bool SpendResources(int stone = 0, int iron = 0, int copper = 0, int silver = 0, int gold = 0)
    {
        if (stoneBar >= stone && ironBar >= iron && copperBar >= copper && 
            silverBar >= silver && goldBar >= gold)
        {
            stoneBar -= stone;
            ironBar -= iron;
            copperBar -= copper;
            silverBar -= silver;
            goldBar -= gold;
            return true;
        }
        return false;
    }

    public int GetResourceCount(int oreIndex)
    {
        return oreIndex switch
        {
            0 => stoneBar,
            1 => ironBar,
            2 => copperBar,
            3 => silverBar,
            4 => goldBar,
            _ => 0
        };
    }

    #endregion

    #region Unlock
    private void HandleSpecialUpgrades(UpgradeNodeData nodeData)
    {
        // 최근 업그레이드된 것만 처리 (마지막 1회만)
        if (nodeData.upgradeCount == 0) return;

        switch (nodeData.upgradeType)
        {
            case UpgradeType.UnlockOre:
                HandleOreUnlock(nodeData);
                break;
                
            case UpgradeType.UnlockSword:
                unlockSword = true;
                Debug.Log("검 해금!");
                break;
        }
    }

    /// <summary>
    /// 광석 해금 처리 - 명확하게!
    /// </summary>
    private void HandleOreUnlock(UpgradeNodeData nodeData)
    {
        // 노드 인덱스로 어떤 광석인지 판단하고 해금 상태만 변경
        switch (nodeData.Index)
        {
            case 7:  // "Unlock Ore: Iron" 노드
                unlockIron = true;
                Debug.Log("Iron 광석 해금!");
                break;
                
            case 13: // "Unlock Ore: Copper" 노드  
                unlockCopper = true;
                Debug.Log("Copper 광석 해금!");
                break;
                
            case 20: // "Unlock Ore: Silver" 노드
                unlockSilver = true; 
                Debug.Log("Silver 광석 해금!");
                break;
                
            case 26: // "Unlock Ore: Gold" 노드
                unlockGold = true;
                Debug.Log("Gold 광석 해금!");
                break;
        }
        
        // 자동 저장 (선택사항)
        // SaveData();
    }
    #endregion

    #region Upgrades


    #endregion

    #region Node Data Methods

    public void UpdateNode(UpgradeNodeData nodeData)
    {
        int existingIndex = upgradeNodes.FindIndex(n => n.Index == nodeData.Index);
        if (existingIndex >= 0)
        {
            upgradeNodes[existingIndex] = nodeData;
        }
        else
            return;
        
        upgradeNodes[existingIndex].upgradeCount++;
        
        RecalculateAllStats();
    }

    public void UpdateNodeData(List<UpgradeNodeData> newNodeData)
    {
        upgradeNodes = new List<UpgradeNodeData>(newNodeData);
        RecalculateAllStats();
    }

    #endregion
}