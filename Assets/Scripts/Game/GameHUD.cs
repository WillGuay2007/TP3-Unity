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

    [SerializeField] GameObject m_Fader;
    [SerializeField] Animation m_FaderAnimation;

    [SerializeField] TextMeshProUGUI m_EnemiesLeftText;
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

    public void FadeToBlack()
    {
        m_FaderAnimation.Play();
    }

    public void NotifyEnemiesLeft (int EnemiesLeft)
    {
        if (EnemiesLeft == 0) { m_EnemiesLeftText.text = "All enemies are dead."; return; }
        m_EnemiesLeftText.text = "Enemies left: " + EnemiesLeft.ToString();
    }
}
