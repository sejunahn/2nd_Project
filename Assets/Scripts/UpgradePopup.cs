using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
   public List<UpgradeNodeData> nodes= null;

   [SerializeField]private List<UpgradeNode> upgradeNodes;
   
   [SerializeField] private GameObject upgradePopup;
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

   private void Init()
   {
      LoadData();
   }

   public void Show()
   {
      Init();
      StartCoroutine(IE_PopupShow());
   }

   public IEnumerator IE_PopupShow()
   {
      yield return null;
      
      upgradePopup.SetActive(true);
   }

   public IEnumerator ClosePopup()
   {
      yield return null;
      upgradePopup.SetActive(false);
      SaveData();
      // TODO:게임 스타트 연결
   }
}
