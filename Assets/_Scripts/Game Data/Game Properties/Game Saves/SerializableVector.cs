using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCode.GameData
{
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public static SerializableVector3 Zero = new SerializableVector3(0, 0, 0);

        [JsonConstructor]
        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public SerializableVector3(Vector3 _vector)
        {
            this.x = _vector.x;
            this.y = _vector.y;
            this.z = _vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}


