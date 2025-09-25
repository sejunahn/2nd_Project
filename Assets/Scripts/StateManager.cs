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
        SetBaseValues();
    }
    private readonly float baseMiningRadius = 1.2f;
    private readonly float baseMiningDPS = 15f;
    private readonly float baseOreRespawnTime = 3f;
    private readonly int baseInitCount = 5;
    private readonly int baseMaxCount = 20;
    private readonly float baseTimer = 60f;

    public float Timer;
    public float miningRadius;
    public float miningDPS;
    public float oreRespawnTime;
    public int initCount;
    public int maxCount;

    public int stoneBar;
    public int ironBar;
    public int copperBar;
    public int silverBar;
    public int goldBar;

    public bool unlockStone;
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

        stoneBar = 0;
        ironBar = 0;
        copperBar = 0;
        silverBar = 0;
        goldBar = 0;
        
        unlockStone = true;
        unlockIron = false;
        unlockCopper = false;
        unlockSilver = false;
        unlockGold = false;
        unlockSword = false;

        upgradeNodes.Clear();
    }

    public void Init()
    {
        NodeDataListWrapper wrapper = UpgradeNodeDataIO.LoadWrapper();
        LoadResourcesAndUnlocks(wrapper);
        upgradeNodes = wrapper.items ?? new List<UpgradeNodeData>();
        RecalculateAllStats();
    }

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

    private void RecalculateAllStats()
    {
        Timer = baseTimer;
        miningRadius = baseMiningRadius;
        miningDPS = baseMiningDPS;
        oreRespawnTime = baseOreRespawnTime;
        initCount = baseInitCount;
        maxCount = baseMaxCount;

        foreach (var node in upgradeNodes)
        {
            if (node.upgradeCount > 0)
            {
                ApplyNodeEffectMultipleTimes(node);
            }
        }
    }

    private void ApplyNodeEffectMultipleTimes(UpgradeNodeData node)
    {
        for (int i = 0; i < node.upgradeCount; i++)
        {
            ApplyNodeEffect(node);
        }
    }

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
                
                break;
                
            case UpgradeType.Respawn:
                oreRespawnTime -= node.value;
                break;
                
            case UpgradeType.InitCount:
                initCount += (int)node.value;
                break;
                
            case UpgradeType.MaxCount:
                maxCount += (int)node.value;
                break;
                
            case UpgradeType.AttackSpeed:
                
                break;
        }
    }

    public void SaveData()
    {
        NodeDataListWrapper wrapper = CreateWrapperFromCurrentData();
        UpgradeNodeDataIO.SaveGameData(wrapper);
    }

    private NodeDataListWrapper CreateWrapperFromCurrentData()
    {
        return new NodeDataListWrapper
        {
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
            
            timer = baseTimer,
            miningRadius = baseMiningRadius,
            miningDPS = baseMiningDPS,
            oreRespawnTime = baseOreRespawnTime,
            initCount = baseInitCount,
            maxCount = baseMaxCount
        };
    }

    public void ResetToDefault()
    {
        SetBaseValues();
    }

    #region Ores

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
    
    private void HandleOreUnlock(UpgradeNodeData nodeData)
    {
        switch (nodeData.Index)
        {
            case 7:
                unlockIron = true;
                break;
                
            case 13:
                unlockCopper = true;
                break;
                
            case 20:
                unlockSilver = true;
                break;
                
            case 26:
                unlockGold = true;
                break;
        }
    }
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