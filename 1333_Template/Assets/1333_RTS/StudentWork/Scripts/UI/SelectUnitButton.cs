using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUnitButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;

    private UnitType unitData;

    public void SetUp(UnitType unitType, Action<UnitType> onClick)
    {
        unitData = unitType;
        buttonText.text = unitData.Name;
        buttonImage.sprite = unitData.Icon;

        // Ensure we clear previous listeners (important for pooling/reuse)
        button.onClick.RemoveAllListeners();

        // Hook up the passed-in delegate
        button.onClick.AddListener(() => onClick?.Invoke(unitData));
    }
}
