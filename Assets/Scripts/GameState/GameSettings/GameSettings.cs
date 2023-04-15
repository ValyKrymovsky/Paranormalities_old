using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameSettings
{
    public float masterSoundVolume;
    public float musicSoundVolume;

    public GameSettings(float _masterSoundVolume, float _musicSoundVolume)
    {
        masterSoundVolume = _masterSoundVolume;
        musicSoundVolume = _musicSoundVolume;
    }
}
