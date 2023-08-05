using UnityEngine;
using MyBox;
using System;
using System.IO;
using Newtonsoft.Json;

namespace MyCode.GameData.GameSettings
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewGameSettingsData", menuName = "DataObjects/GameSettings/Settings")]
    public class GameSettingsData : ScriptableObject
    {
        [Space]
        [Separator("Game Settings")]
        [Space]

        [Header("General")]
        [Space]

        [Header("Audio")]
        [Space]
        [SerializeField] private float _masterVolume = 100;
        [SerializeField] private float _musicVolume = 100;
        [SerializeField] private float _effectsVolume = 100;
        [Space]

        [Header("Resolution")]
        [Space]
        [SerializeField] private XYStructure _aspectRatio = new XYStructure(16, 9);
        [SerializeField] private XYStructure _resolution = new XYStructure(1280, 720);
        [SerializeField] private VideoSettingsState _vsync = VideoSettingsState.off;
        [Space]
        
        [Header("Video")]
        [Space]
        [SerializeField] private VideoSettings _shadowDetail = VideoSettings.medium;
        [SerializeField] private VideoSettings _textureDetail = VideoSettings.medium;
        [SerializeField] private VideoSettings _lightingDetail = VideoSettings.medium;

        [SerializeField] private float _gamma = 1;

        [SerializeField] private float _fieldOfView = 60;

        public float MasterVolume { get => _masterVolume; set => _masterVolume = value; }
        public float MusicVolume { get => _musicVolume; set => _musicVolume = value; }
        public float EffectsVolume { get => _effectsVolume; set => _effectsVolume = value; }
        public XYStructure AspectRatio { get => _aspectRatio; set => _aspectRatio = value; }
        public XYStructure Resolution { get => _resolution; set => _resolution = value; }
        public VideoSettingsState Vsync { get => _vsync; set => _vsync = value; }
        public VideoSettings ShadowDetail { get => _shadowDetail; set => _shadowDetail = value; }
        public VideoSettings TextureDetail { get => _textureDetail; set => _textureDetail = value; }
        public VideoSettings LightingDetail { get => _lightingDetail; set => _lightingDetail = value; }
        public float Gamma { get => _gamma; set => _gamma = value; }
        public float FieldOfView { get => _fieldOfView; set => _fieldOfView = value; }

    }
}


