using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    private AudioSource m_AudioSource;
    [SerializeField] private AudioClip[] m_PunchSounds;
    [SerializeField] private AudioClip[] m_RunSounds;
    [SerializeField] private AudioClip m_ShootSound;
    
    //Puisque je planifie d'utiliser plusieurs audios, je me suis fait un script pour que ca soit plus facile a gérer.
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    void PlaySound(AudioClip Sound)
    {
        m_AudioSource.PlayOneShot(Sound);
    }

    AudioClip GetRandomSoundFromArray(AudioClip[] SoundsArray)
    {
        int ArrayLength = SoundsArray.Length;
        return SoundsArray[Random.Range(0, ArrayLength)];
    }

    public void PlayShootSound()
    {
        PlaySound(m_ShootSound);
    }

    public void PlayPunchSound()
    {
        PlaySound(GetRandomSoundFromArray(m_PunchSounds));
    }

    public void PlayRunSound()
    {
        PlaySound(GetRandomSoundFromArray(m_RunSounds));
    }
}
