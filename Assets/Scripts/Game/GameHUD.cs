using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] Slider m_PlayerHealthSlider;
    [SerializeField] TextMeshProUGUI m_PlayerHealthText;
    public void NotifyPlayerHealth(int NewHealth)
    {
        m_PlayerHealthSlider.value = NewHealth;
        m_PlayerHealthText.text = "Health: " + NewHealth.ToString();
    }
}
