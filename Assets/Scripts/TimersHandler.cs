using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//J'ai créé ce script car j'ai remarqué que j'aurai besoin d'utiliser beaucoup de timers donc je prefere m'organiser.
public class TimersHandler : MonoBehaviour
{
    public float m_Duration; //La seule variable publique.
    private float m_TimeElapsed = 0;
    private bool m_IsActive = false;

    //Résumé de la fonction:
    //Ca sert a update le timer et te donner le resultat si il a atteint son but ou non.
    //Si il a atteint son but, le timer se reset et va attendre a son prochain start.
    public bool UpdateTimer()
    {
        if (!m_IsActive)
            return false;

        m_TimeElapsed += Time.deltaTime;

        if (m_TimeElapsed >= m_Duration)
        {
            m_TimeElapsed = 0;
            m_IsActive = false;
            return true;
        }

        return false;
    }

    public void StartTimer()
    {
        m_TimeElapsed = 0;
        m_IsActive = true;
    }

    public void ResetTimer()
    {
        m_TimeElapsed = 0;
        m_IsActive = false;
    }

    public bool IsActive()
    {
        return m_IsActive;
    }

    public float GetTimeElapsed()
    {
        return m_TimeElapsed;
    }
}
