using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] OreSpawner oreSpawner;
    [SerializeField] IsoGridGenerator gridGenerator;

    [SerializeField] UpgradePopup upgradePopup;

    [SerializeField] MinerController minerController;
    #region Popups
    
    public void CallUpgradePopup()
    {
        //실제 켜주기
        upgradePopup.Show();
    }
    #endregion


/*
 * 1. gamestart -> dissorve ->  gridGen -> oreGen -> uiSetting-> resolve -> ingame
 * 2. endgame[timer] -> pause -> keepgoing or upgrade -> 1.
 *                                                    -> upgradepopup call -> upgradeing -> startgame -> saveData -> 1.
 * 
 */
    public void StartGame()
    {
        //뭔가 데이터를 먼저 로드할거면 방식 추가해야됨.
        
        gridGenerator.Init();
        oreSpawner.Init();
        minerController.SetActiveController();
    }
}