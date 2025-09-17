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
        // 초기값 설정
        miningRadius = 12f;//1.2f;
        miningDPS = 15f;
        oreRespawnTime = 1f;
        initCount = 200;
        maxCount = 200;

        goldBarA = 0;
        goldBarB = 0;
        goldBarC = 0;
        goldBarD = 0;
        goldBarE = 0;

        unlockOre1 = true;
        unlockOre2 = true;
        unlockOre3 = true;
        unlockOre4 = true;
        unlockOre5 = true;
    }

    // ===== 스탯 =====
    public float miningRadius;  //마우스 공격 범위
    public float miningDPS; //마우스 공격 dps
    public float oreRespawnTime;    //광석 리스폰 시간
    public int initCount;   //최초 깔리는 돌개수
    public int maxCount;    //최대 개수

    public int goldBarA;
    public int goldBarB;
    public int goldBarC;
    public int goldBarD;
    public int goldBarE;

    public bool unlockOre1;
    public bool unlockOre2;
    public bool unlockOre3;
    public bool unlockOre4;
    public bool unlockOre5;

    // ===== 강화 =====
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
    public void AddGoldBar(int valueA, int valueB, int valueC, int valueD, int valueE)
    {
        goldBarA += valueA;
        goldBarB += valueB;
        goldBarC += valueC;
        goldBarD += valueD;
        goldBarE += valueE;
    }

    public void UnlockOre(int index)
    {
        switch (index)
        {
            case 2: unlockOre2 = true; break;
            case 3: unlockOre3 = true; break;
            case 4: unlockOre4 = true; break;
            case 5: unlockOre5 = true; break;
        }
    }
}

