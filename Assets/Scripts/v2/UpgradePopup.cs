using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
   public List<UpgradeNodeData> nodes= null;

   [SerializeField]private List<UpgradeNode> upgradeNodes;
   
   public void SaveData()
   {
      UpgradeNodeDataIO.Save(null);
   }
   
   
   public void LoadData()
   {
      nodes = null;
      var items = UpgradeNodeDataIO.Load();

      nodes = items;

      for (int i = 0; i < upgradeNodes.Count; i++)
      {
         upgradeNodes[i].InitNode(nodes[i]);
      }
   }

   public void Init()
   {
      
   }
}
