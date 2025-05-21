using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] Slider m_PlayerHealthSlider;
    [SerializeField] TextMeshProUGUI m_PlayerHealthText;
    [SerializeField] GameObject m_PlayerPanel; //Un panel

    [SerializeField] Slider m_EnemyHealthSlider;
    [SerializeField] TextMeshProUGUI m_EnemyHealthText;
    [SerializeField] GameObject m_EnemyPanel; //Un panel
    public void NotifyPlayerHealth(int NewHealth)
    {
        m_PlayerHealthSlider.value = NewHealth;
        m_PlayerHealthText.text = "Player health: " + NewHealth.ToString();
    }

    public void NotifyEnemyHealth(int NewHealth)
    {
        m_EnemyHealthSlider.value = NewHealth;
        m_EnemyHealthText.text = "Enemy health: " + NewHealth.ToString();
    }

    public void ChangeEnemyPanelState(bool State) //False = invisible, true = visible
    {
        if (State)
        {
            m_EnemyPanel.transform.localScale = Vector3.one; //Changer la scale a (1,1,1)
        } 
        else
        {
            m_EnemyPanel.transform.localScale = Vector3.zero; //Changer la scale a (0,0,0)
        }
    }
}
