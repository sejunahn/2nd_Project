using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
   public List<UpgradeNodeData> nodes= new List<UpgradeNodeData>();
   [SerializeField]private List<UpgradeNode> upgradeNodes;
   [SerializeField] private GameObject upgradePopup;
   
   private Action endAction;
   private Coroutine Co_Popup;
   public void SaveData()
   {
      StatManager.Instance.SaveData();
   }
   
   private void Init()
   {
      if (nodes != null)
         nodes = null;
      nodes = StatManager.Instance.upgradeNodes;

      for (int i = 0; i < upgradeNodes.Count; i++)
      {
         upgradeNodes[i].InitNode(nodes[i]);
      }
   }

   public void Show(Action action)
   {
      endAction = action;
      Init();
      if(Co_Popup != null)
         StopCoroutine(Co_Popup);
      
      Co_Popup = StartCoroutine(IE_PopupShow());
   }

   public void Close()
   {
      if(Co_Popup != null)
         StopCoroutine(Co_Popup);
      
      Co_Popup = StartCoroutine(ClosePopup());
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
      endAction();
   }
}
