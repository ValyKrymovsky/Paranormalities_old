using UnityEngine;
using System.Linq;
using MyCode.GameData.Sound;
using MyCode.GameData.PlayerData;

namespace MyCode.Managers
{
    public class PlayerSoundManager : Manager<PlayerSoundManager>
    {
        public void LoadSoundObjects()
        {
            for (int i = 0; i < _soundData.ListCapacity; i++)
            {
                _soundData.SoundObjects.Add(new SoundObject(0, 10, false, null, Vector3.zero));
            }
        }

        public SoundObject UseSoundObject()
        {
            if (_soundData.SoundObjects.Count == 0) return null;

            SoundObject sound = _soundData.SoundObjects.First();
            _soundData.SoundObjects.Remove(sound);
            sound.Active = true;

            return sound;
        }

        public void ReturnSoundObject(SoundObject _soundObject)
        {
            if (_soundObject == null) return;

            _soundObject.ResetProperties();
            _soundData.SoundObjects.Add(_soundObject);
        }

        [field: SerializeField] private PlayerSound _soundData;
        public PlayerSound SoundData { get => _soundData; }
    }
}

