using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField, ReadOnly] private TextMeshProUGUI[] fields;

    // загрузка параметров юнита в интерфейс
    public void LoadStats(int[] stats)
    {
        for (int i = 0; i < stats.Length; i++)
        {
            fields[i].text = stats[i].ToString();
        }
    }
}

