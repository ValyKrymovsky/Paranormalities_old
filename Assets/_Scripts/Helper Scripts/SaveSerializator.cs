using UnityEngine;
using Cysharp.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using MyCode.GameData.GameSave;
using System.Collections.Generic;
using System.Reflection;

namespace MyCode.Helper.Serializer
{
    public class SaveSerializer : MonoBehaviour
    {
        public static async UniTask<string> ReadSaveFileAsync(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public static async UniTask<GameSave> DeserializeGameSaveAsync(string _jsonString, GameSave _gameSave)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                _gameSave = JsonConvert.DeserializeObject<GameSave>(_jsonString);

            });

            return _gameSave;
        }

        public static GameSave DeserializeGameSave(string _jsonString, GameSave _gameSave)
        {
            _gameSave = JsonConvert.DeserializeObject<GameSave>(_jsonString);
            return _gameSave;
        }

        public static async UniTask<GameSave> UpdateSaveAsync(GameSave _newGameSave, GameSave _gameSaveToUpdate)
        {
            await UniTask.RunOnThreadPool(() => {
                _gameSaveToUpdate = _newGameSave;
            });

            return _gameSaveToUpdate;
        }

        public static async UniTask<bool> SerializeObjectAsync(JsonSerializer _serializer, GameSave _data, string _savePath)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                StreamWriter file = File.CreateText(_savePath);

                _serializer.Serialize(file, _data);

                file.Close();
            });

            return true;
        }
    }

    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private HashSet<string> _propsToIgnore;

        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            _propsToIgnore = new HashSet<string>(propNamesToIgnore);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (_propsToIgnore.Contains(property.PropertyName))
            {
                property.ShouldSerialize = (x) => { return false; };
            }

            return property;
        }
    }
}
