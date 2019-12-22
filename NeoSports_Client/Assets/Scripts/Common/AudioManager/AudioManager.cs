using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    AudioStorage soundStorage;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

    }

    public void PlaySound(eSoundId id)
    {
        AudioSource.PlayClipAtPoint(soundStorage.Get(id), Vector3.zero);
    }
}