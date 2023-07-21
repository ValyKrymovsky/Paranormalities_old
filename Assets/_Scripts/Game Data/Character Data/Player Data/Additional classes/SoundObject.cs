using MyCode.Player;
using System.Collections;
using System;
using UnityEngine;

namespace MyCode.Player.Sound
{
    public class SoundObject
    {
        private int _id;

        private float _volume;

        private float _age;
        private float _maxAge;

        private bool _active;

        private GameObject _source;
        private Vector3 _sourceLocation;

        private Coroutine _agingCoroutine;

        public SoundObject(float _volume, float _maxAge, bool _active, GameObject _source, Vector3 _sourceLocation)
        {
            this._id = UnityEngine.Random.Range(0, 1000);

            this._volume = _volume;

            this._age = 0;
            this._maxAge = _maxAge;

            this.Active = _active;

            this._source = _source;
            this._sourceLocation = _sourceLocation;
        }

        public void ResetProperties()
        {
            _volume = 0;
            _age = 0;
            _maxAge = 0;
            _active = false;
            _source = null;
            _sourceLocation = Vector3.zero;
        }

        public IEnumerator Aging()
        {
            while (_age <= _maxAge)
            {
                _age += 1;
                yield return new WaitForSeconds(.25f);
            }

            Debug.Log("Stopped aging");
            _agingCoroutine = null;
            yield break;
        }

        public int Id { get => _id; }
        public float Volume { get => _volume; set => _volume = value; }
        public float Age { get => _age; set => _age = value; }
        public float MaxAge { get => _maxAge; set => _maxAge = value; }
        public bool Active { get => _active; set => _active = value; }
        public GameObject Source { get => _source; set => _source = value; }
        public Vector3 SourceLocation { get => _sourceLocation; set => _sourceLocation = value; }
        public Coroutine AgingCoroutine { get => _agingCoroutine; set => _agingCoroutine = value; }
    }

}
