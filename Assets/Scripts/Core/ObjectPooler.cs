using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter.Core
{
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;

            string key = prefab.name.Replace("(Clone)", "").Trim();
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<GameObject>();
            }

            GameObject obj = null;
            while (_pools[key].Count > 0 && obj == null)
            {
                obj = _pools[key].Dequeue();
            }

            if (obj == null)
            {
                obj = Instantiate(prefab, position, rotation);
            }
            else
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }

            obj.SetActive(true);
            return obj;
        }

        public void Despawn(GameObject obj, string poolKey)
        {
            if (obj == null) return;

            obj.SetActive(false);
            if (!_pools.ContainsKey(poolKey))
            {
                _pools[poolKey] = new Queue<GameObject>();
            }
            _pools[poolKey].Enqueue(obj);
        }
    }
}
