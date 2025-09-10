using UnityEngine;
using TMPro;

public class ResourceCounter : MonoBehaviour
{
    public int oreCount = 0;
    public TextMeshProUGUI text;

    public void Add(int amount)
    {
        oreCount += amount;
        if (text != null) text.text = oreCount.ToString();
    }
}