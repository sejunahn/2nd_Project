using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NodeDataListWrapper
{
    public float timer;
    public float miningRadius;
    public float miningDPS;
    public float oreRespawnTime;
    public int initCount;
    public int maxCount;
    public int stoneBarCount;
    public int ironBarCount;
    public int copperBarCount;
    public int silverBarCount;
    public int goldBarCount;
    public bool unlockIron;
    public bool unlockCopper;
    public bool unlockSilver;
    public bool unlockGold;
    public bool unlockSword;
    public List<UpgradeNodeData> items = new List<UpgradeNodeData>();
}

public static class UpgradeNodeDataIO
{
    private static string SavePath => Path.Combine(Application.dataPath, "Datas/nodeData.json");

    #region Save Methods
    
    public static void SaveGameData(NodeDataListWrapper wrapper)
    {
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
    }
    public static void SaveGameData(
        List<UpgradeNodeData> nodeDataList,
        float timer = 60f,
        float miningRadius = 2f,
        float miningDPS = 10f,
        float oreRespawnTime = 3f,
        int initCount = 5,
        int maxCount = 20,
        int stoneBarCount = 0,
        int ironBarCount = 0,
        int copperBarCount = 0,
        int silverBarCount = 0,
        int goldBarCount = 0,
        bool unlockIron = false,
        bool unlockCopper = false,
        bool unlockSilver = false,
        bool unlockGold = false,
        bool unlockSword = false
    )
    {
        NodeDataListWrapper wrapper = new NodeDataListWrapper
        {
            timer = timer,
            miningRadius = miningRadius,
            miningDPS = miningDPS,
            oreRespawnTime = oreRespawnTime,
            initCount = initCount,
            maxCount = maxCount,
            stoneBarCount = stoneBarCount,
            ironBarCount = ironBarCount,
            copperBarCount = copperBarCount,
            silverBarCount = silverBarCount,
            goldBarCount = goldBarCount,
            unlockIron = unlockIron,
            unlockCopper = unlockCopper,
            unlockSilver = unlockSilver,
            unlockGold = unlockGold,
            unlockSword = unlockSword,
            items = nodeDataList
        };

        string json = JsonUtility.ToJson(wrapper, true);
        
        // 디렉토리가 없으면 생성
        string directory = Path.GetDirectoryName(SavePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllText(SavePath, json);
    }
    #endregion

    #region Load Methods

    public static List<UpgradeNodeData> Load()
    {
        if (!File.Exists(SavePath))
        {
            return new List<UpgradeNodeData>();
        }

        string json = File.ReadAllText(SavePath);
        NodeDataListWrapper wrapper = JsonUtility.FromJson<NodeDataListWrapper>(json);
        return wrapper?.items ?? new List<UpgradeNodeData>();
    }

    // 새로운 방식: 전체 Wrapper 로드
    public static NodeDataListWrapper LoadWrapper()
    {
        if (!File.Exists(SavePath))
            return CreateDefaultWrapper();
        
        try
        {
            string json = File.ReadAllText(SavePath);
            NodeDataListWrapper wrapper = JsonUtility.FromJson<NodeDataListWrapper>(json);
            
            if (wrapper == null)
                return CreateDefaultWrapper();
            
            return wrapper;
        }
        catch (System.Exception e)
        {
            return CreateDefaultWrapper();
        }
    }
    
    public static float LoadTimer()
    {
        var wrapper = LoadWrapper();
        return wrapper.timer;
    }

    public static float LoadMiningRadius()
    {
        var wrapper = LoadWrapper();
        return wrapper.miningRadius;
    }

    public static int[] LoadOreBars()
    {
        var wrapper = LoadWrapper();
        return new int[] 
        {
            wrapper.stoneBarCount,
            wrapper.ironBarCount,
            wrapper.copperBarCount,
            wrapper.silverBarCount,
            wrapper.goldBarCount
        };
    }

    public static bool[] LoadUnlockStates()
    {
        var wrapper = LoadWrapper();
        return new bool[]
        {
            true,
            wrapper.unlockIron,
            wrapper.unlockCopper,
            wrapper.unlockSilver,
            wrapper.unlockGold
        };
    }

    #endregion

    public static NodeDataListWrapper CreateDefaultWrapper()
    {
        return new NodeDataListWrapper
        {
            timer = 60f,
            miningRadius = 2f,
            miningDPS = 10f,
            oreRespawnTime = 3f,
            initCount = 5,
            maxCount = 20,
            stoneBarCount = 0,
            ironBarCount = 0,
            copperBarCount = 0,
            silverBarCount = 0,
            goldBarCount = 0,
            unlockIron = false,
            unlockCopper = false,
            unlockSilver = false,
            unlockGold = false,
            unlockSword = false,
            items = new List<UpgradeNodeData>()
        };
    }

    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }

    // 저장 파일 삭제
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
    
    public static string GetSavePath()
    {
        return SavePath;
    }
}