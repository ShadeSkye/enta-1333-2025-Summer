using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIEntry : MonoBehaviour
{
    public ResourceType Type;
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public void UpdateAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }
}
