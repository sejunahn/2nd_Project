using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class NodeDataListWrapper
{
    public List<UpgradeNodeData> items = new List<UpgradeNodeData>();
}

public static class UpgradeNodeDataIO
{
    
    private static string SavePath => Path.Combine(Application.dataPath, "Datas/nodeData.json");

    // 저장
    public static void Save(List<UpgradeNodeData> nodeDataList)
    {
        NodeDataListWrapper wrapper = new NodeDataListWrapper();
        wrapper.items = nodeDataList;

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"NodeData 저장 완료: {SavePath}");
    }

    // 불러오기
    public static List<UpgradeNodeData> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장된 데이터 없음");
            return new List<UpgradeNodeData>();
        }

        string json = File.ReadAllText(SavePath);
        NodeDataListWrapper wrapper = JsonUtility.FromJson<NodeDataListWrapper>(json);
        return wrapper.items;
    }
}