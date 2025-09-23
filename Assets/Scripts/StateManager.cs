using UnityEditor.VisionOS;
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
        miningRadius = 12f;//1.2f;
        miningDPS = 15f;
        oreRespawnTime = 1f;
        initCount = 200;
        maxCount = 200;

        stoneBar = 0;
        ironBar = 0;
        copperBar = 0;
        silverBar = 0;
        goldBar = 0;

        unlockIron = true;
        unlockCopper = true;
        unlockSilver = true;
        unlockGold = true;

        unlockSword = true;
    }

    public float Timer = 15f;
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

    public bool unlockIron;
    public bool unlockCopper;
    public bool unlockSilver;
    public bool unlockGold;

    public bool unlockSword;
    

    
    public void UpgradeMiningRadius(float add)
    {
        miningRadius += add;
    }

    public void UpgradeDPS(float add)
    {
        miningDPS += add;
    }

    public void ReduceRespawnTime(float factor)
    {
        oreRespawnTime = Mathf.Max(0.5f, oreRespawnTime * factor);
    }
    public void AddBar(int valueA, int valueB, int valueC, int valueD, int valueE)
    {
        stoneBar += valueA;
        ironBar += valueB;
        copperBar += valueC;
        silverBar += valueD;
        goldBar += valueE;
    }

    public void UnlockSword()
    {
        unlockSword = true;
    }

    public void UnlockOre(int index)
    {
        switch (index)
        {
            case 1: unlockIron = true; break;
            case 2: unlockCopper = true; break;
            case 3: unlockSilver = true; break;
            case 4: unlockGold = true; break;
        }
    }
}

