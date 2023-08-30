using System.Collections.Generic;
using UnityEngine;
using MyBox;
using MyCode.GameData.Sound;


namespace MyCode.GameData.PlayerData
{
    [CreateAssetMenu(fileName = "NewSoundData", menuName = "DataObjects/Systems/Sound/SoundData")]
    public class PlayerSoundData : ScriptableObject
    {
        [Space]
        [Separator("Footstep Properties", true)]
        [Space]

        [Header("Audio Properties")]
        [Space]
        [SerializeField] private float footstepMinPitch = .8f;
        [SerializeField] private float footstepMaxPitch = 1.2f;
        [SerializeField, Range(0, 10)] private float footstepVolume = 1f;
        private List<AudioClip> walkClips;
        private List<AudioClip> sprintClips;
        private List<AudioClip> usedFootsteps;
        private GameObject leftFootJoint;
        private GameObject rightFootJoint;

        [Header("Sound Properties")]
        [Space]

        [Header("Sound list")]
        [Space]
        [SerializeField] private HashSet<SoundObject> _soundObjects = new HashSet<SoundObject>();
        [SerializeField] private int _listCapacity;

        public HashSet<SoundObject> SoundObjects { get => _soundObjects; set => _soundObjects = value; }
        public int ListCapacity { get => _listCapacity; set => _listCapacity = value; }
    }

}
