using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] SoundEffectGroup[] soundEffectGroups;
    Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        this.InitualizeDictionary();
    }

    void InitualizeDictionary()
    {
        this.soundDictionary = new();
        foreach (SoundEffectGroup soundEffectGroup in this.soundEffectGroups)
        {
            soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetAudioClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }
}

[Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;

}
