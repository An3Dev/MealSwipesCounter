using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MainUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countText;

    public void UpdateUI(int currentCount, int maxCount)
    {
        countText.text = currentCount + " / " + maxCount;
    }
}
