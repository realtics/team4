using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649 //serializeField Warning

[CreateAssetMenu]
public class AudioStorage : ScriptableObject
{
    [SerializeField]
    SoundSrc[] soundSrcs;

    Dictionary<eSoundId, AudioClip> dicSounds = new Dictionary<eSoundId, AudioClip>();

    void GenerateDictionary()
    {
        for (int i = 0; i < soundSrcs.Length; i++)
        {
            dicSounds.Add(soundSrcs[i].Id, soundSrcs[i].SoundFile);
        }
    }

    public AudioClip Get(eSoundId id)
    {
        Debug.Assert(soundSrcs.Length > 0, "No soundSource data!");
        if (dicSounds.Count == 0)
        {
            GenerateDictionary();
        }

        return dicSounds[id];
    }
}

[Serializable]
public struct SoundSrc
{
    [SerializeField]
    AudioClip soundFile;

    [SerializeField]
    eSoundId soundId;

    public AudioClip SoundFile { get { return soundFile; } }
    public eSoundId Id { get { return soundId; } }
}

public enum eSoundId
{
    Shoot, Walk, Start, GameEnd, Hit, Flap, Fall, Score, Pull, BGM,
}
