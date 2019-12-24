using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    AudioStorage soundStorage;

	AudioSource _soundSource;
	void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

		_soundSource = GetComponent<AudioSource>();
	}

    public void PlaySound(eSoundId id)
    {
		if (_soundSource.isPlaying)
			return;
		_soundSource.clip = soundStorage.Get(id);
		_soundSource.Play();
		//AudioSource.PlayClipAtPoint(soundStorage.Get(id), Vector3.zero);
	}

	public void PlayOnLoop(eSoundId id)
	{
		_soundSource.clip = soundStorage.Get(id);
		_soundSource.loop = true;
		_soundSource.Play();
	}

	public void StopOnLoop(eSoundId id)
	{
		if(_soundSource.isPlaying && _soundSource.clip == soundStorage.Get(id))
		_soundSource.Stop();
	}

	public void PlayOnDelay(eSoundId id, float delayTime)
	{
		if (_soundSource.isPlaying)
			return;
		_soundSource.clip = soundStorage.Get(id);
		_soundSource.loop =true;
		_soundSource.Play();
	}
}