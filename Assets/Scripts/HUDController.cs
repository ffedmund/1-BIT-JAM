using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject hpBar;
    public TextMeshProUGUI interactText;

    public void SetInteractText(string text)
    {
        interactText.SetText(text);
    }
}
