using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Linq;
using MyCode.Data.Player;
using MyCode.Player.Sound;
using MyCode.Data.Settings;
using System;

namespace MyCode.Managers
{
    public class PlayerSoundManager : Manager<PlayerSoundManager>
    {

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

            }
        }

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

        [field: SerializeField] private PlayerSoundData _soundData;
        public PlayerSoundData SoundData { get => _soundData; }
    }
}

