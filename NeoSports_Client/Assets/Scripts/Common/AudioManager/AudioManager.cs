using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    AudioStorage soundStorage;

	AudioSource _soundSource;
	AudioSource _bgmSource;
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
		PlayBackGround();
	}

	void Start()
	{
		
	}

	public void PlaySound(eSoundId id)
    {
		//if (_soundSource.isPlaying)
		//	return;
		//_soundSource.clip = soundStorage.Get(id);
		//_soundSource.Play();
		AudioSource.PlayClipAtPoint(soundStorage.Get(id), Vector3.zero);
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

	public void PlayBackGround()
	{
		_bgmSource = _soundSource;
		_bgmSource.clip = soundStorage.Get(eSoundId.BGM);
		_bgmSource.volume = 0.1f;
		_bgmSource.loop = true;
		_bgmSource.playOnAwake = true;
		_bgmSource.Play();
	}
}