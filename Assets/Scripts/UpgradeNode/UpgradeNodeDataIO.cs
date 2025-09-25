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
    
    // public static void Save(List<UpgradeNodeData> nodeDataList)
    // {
    //     NodeDataListWrapper wrapper = new NodeDataListWrapper();
    //     wrapper.items = nodeDataList;
    //
    //     string json = JsonUtility.ToJson(wrapper, true);
    //     File.WriteAllText(SavePath, json);
    //     Debug.Log($"NodeData 저장 완료: {SavePath}");
    // }
    public static void SaveGameData(NodeDataListWrapper wrapper)
    {
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"NodeData 저장 완료: {SavePath}");
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
        Debug.Log($"전체 게임 데이터 저장 완료: {SavePath}");
    }

    // Wrapper 객체를 직접 받아서 저장
    // public static void SaveWrapper(NodeDataListWrapper wrapper)
    // {
    //     string json = JsonUtility.ToJson(wrapper, true);
    //     
    //     string directory = Path.GetDirectoryName(SavePath);
    //     if (!Directory.Exists(directory))
    //     {
    //         Directory.CreateDirectory(directory);
    //     }
    //     
    //     File.WriteAllText(SavePath, json);
    //     Debug.Log($"Wrapper 데이터 저장 완료: {SavePath}");
    // }

    #endregion

    #region Load Methods

    // 기존 방식: 노드 데이터만 로드
    public static List<UpgradeNodeData> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장된 데이터 없음");
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
        {
            Debug.LogWarning("저장된 데이터 없음, 기본값 반환");
            return CreateDefaultWrapper();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            NodeDataListWrapper wrapper = JsonUtility.FromJson<NodeDataListWrapper>(json);
            
            if (wrapper == null)
            {
                Debug.LogError("데이터 파싱 실패, 기본값 반환");
                return CreateDefaultWrapper();
            }
            
            Debug.Log($"게임 데이터 로드 완료: Timer={wrapper.timer}, Radius={wrapper.miningRadius}");
            return wrapper;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"데이터 로드 실패: {e.Message}, 기본값 반환");
            return CreateDefaultWrapper();
        }
    }

    // 특정 데이터만 로드하는 메서드들
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
            true, // Stone은 기본 해금
            wrapper.unlockIron,
            wrapper.unlockCopper,
            wrapper.unlockSilver,
            wrapper.unlockGold
        };
    }

    #endregion

    #region Utility Methods

    // 기본값으로 초기화된 Wrapper 생성
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

    // 저장 파일 존재 여부 확인
    public static bool SaveFileExists()
    {
        return File.Exists(SavePath);
    }

    // 저장 파일 삭제
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("저장 파일 삭제 완료");
        }
        else
        {
            Debug.LogWarning("삭제할 저장 파일이 없습니다");
        }
    }

    // 저장 경로 반환 (디버그용)
    public static string GetSavePath()
    {
        return SavePath;
    }

    #endregion
}