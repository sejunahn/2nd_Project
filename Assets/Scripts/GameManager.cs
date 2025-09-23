using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] OreSpawner oreSpawner;
    [SerializeField] IsoGridGenerator gridGenerator;
    [SerializeField] UpgradePopup upgradePopup;
    [SerializeField] MinerController minerController;

    [SerializeField] SwordSkill swordSkill;
    //TIMER
    [SerializeField] float gameTimer ; 
    [SerializeField] TextMeshProUGUI timerText;
    
    private float currentTimer;
    private bool isGameRunning = false;
    private Coroutine timerCoroutine;

    public static event Action<float> OnTimerUpdate;
    public static event Action OnTimerEnd;

    public Action endUpgradeAction;
    
    #region Popups
    
    public void CallUpgradePopup()
    {
        //실제 켜주기
        upgradePopup.Show(endUpgradeAction);
        PauseGame();
    }
    #endregion

    /*
     * 1. gamestart -> dissorve ->  gridGen -> oreGen -> uiSetting-> resolve -> ingame
     * 2. endgame[timer] -> pause -> keepgoing or upgrade -> 1.
     *                                                    -> upgradepopup call -> upgradeing -> startgame -> saveData -> 1.
     * 
     */
    public void Start()
    {
        endUpgradeAction = ()=> { OnUpgradeComplete(); };
        StartGame();
    }

    #region Game Flow

    public void StartGame()
    {
        gridGenerator.Init();
        // 맵 생성 완료 후에 광석 생성 시작
        gridGenerator.OnGridGenerated += () =>
        {
            oreSpawner.Init();
        };

        minerController.SetActiveController();

        InitTimer();
        StartTimer();
        swordSkill.Init();
    }

    public void RestartGame()
    {
        StopTimer();
        StartGame();
    }

    public void PauseGame()
    {
        isGameRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
    }
    #endregion

    #region Timer
    
    private void InitTimer()
    {
        gameTimer = StatManager.Instance.Timer;
        
        currentTimer = gameTimer;
        UpdateTimerUI();
    }
    
    private void StartTimer()
    {
        isGameRunning = true;
        Time.timeScale = 1f;
        
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isGameRunning = false;
    }
    
    private IEnumerator TimerCoroutine()
    {
        while (currentTimer > 0 && isGameRunning)
        {
            if (isGameRunning)
            {
                currentTimer -= Time.unscaledDeltaTime; 
                UpdateTimerUI();
                
                OnTimerUpdate?.Invoke(currentTimer);
            }
            
            yield return null; 
        }
        
        if (currentTimer <= 0)
        {
            currentTimer = 0;
            UpdateTimerUI();
            EndGame();
        }
    }
    
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // 분:초 형태로 표시
            int minutes = Mathf.FloorToInt(currentTimer / 60);
            int seconds = Mathf.FloorToInt(currentTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    private void EndGame()
    {
        isGameRunning = false;
        OnTimerEnd?.Invoke();

        StartCoroutine(DelayedUpgradePopup());
    }
    
    private IEnumerator DelayedUpgradePopup()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        CallUpgradePopup();
    }
    
    public void SetGameTimer(float newTime)
    {
        gameTimer = newTime;
        if (!isGameRunning)
        {
            currentTimer = gameTimer;
            UpdateTimerUI();
        }
    }
    
    
    public float GetRemainingTime()
    {
        return currentTimer;
    }
    
    public float GetTimerProgress()
    {
        return 1f - (currentTimer / gameTimer);
    }
    
    public void AddTime(float additionalTime)
    {
        currentTimer += additionalTime;
        currentTimer = Mathf.Min(currentTimer, gameTimer);
        UpdateTimerUI();
    }
    #endregion

    #region Else
    public void OnUpgradeComplete()
    {
        RestartGame();
    }
    
    public void OnKeepGoing()
    {
        ResumeGame();
    }
    
    private void OnDestroy()
    {
        StopTimer();
    }
    #endregion
}