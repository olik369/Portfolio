#define EXTEND_POOL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ObjectPooling.Multi
{
    [System.Serializable]
    public class ObjectPool
    {
        public GameObject prefab;
        public int size;
    }

    public class PoolManagerMulti : MonoBehaviour
    {
        // ������Ʈ Ǯ ����Ʈ. ������(�ν�����â)���� ä�� �ֽ��ϴ�. �ڵ�� �ʱ�ȭ �� �� ���� 
        public List<ObjectPool> ObjectPoolList;

        // ������ �̸����� ��� ������Ʈ Ǯ�� ����
        private Dictionary<string, Queue<GameObject>> objectPoolDictionary;

        #region SingleTone
        private static PoolManagerMulti _instance = null;

        public static PoolManagerMulti Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (PoolManagerMulti)FindObjectOfType(typeof(PoolManagerMulti));
                    if (_instance == null)
                    {
                        Debug.Log("There's no active Singletone object");
                    }
                }

                return _instance;
            }
        }


        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                _instance = this;
                Init();
            }
        }
        #endregion

        // ������Ʈ�� �����Ѵ�
        GameObject CreateGameObject(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = prefab.name;
            obj.SetActive(false);
            obj.transform.parent = this.transform;
            return obj;
        }

#if EXTEND_POOL
        ObjectPool FindObjectPool(string poolName)
        {
            return ObjectPoolList.Find(x => (x.prefab != null && string.Equals(x.prefab.name, poolName)));
        }
#endif

        private void Init()
        {
            objectPoolDictionary = new Dictionary<string, Queue<GameObject>>();
            foreach (ObjectPool pool in ObjectPoolList)
            {
                if (!pool.prefab)
                {
                    Debug.LogError("invalid prefab");
                    continue;
                }

                // ������Ʈ Ǯ�� ����� ���⿡ ������Ʈ���� �����Ѵ�
                Queue<GameObject> poolQueue = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    poolQueue.Enqueue(CreateGameObject(pool.prefab));
                }

                // Dictionary �� ������Ʈ Ǯ �߰�
                objectPoolDictionary.Add(pool.prefab.name, poolQueue);
            }
        }

        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;

            if (objectPoolDictionary.ContainsKey(poolName))
            {
                Queue<GameObject> poolQueue = objectPoolDictionary[poolName];

                if (poolQueue.Count > 0)
                {
                    // ������Ʈ Ǯ�� ������ ������Ʈ�� ��ȯ�Ѵ�
                    obj = poolQueue.Dequeue();
                }
                else
                {
                    // ������Ʈ Ǯ�� ������� 
#if EXTEND_POOL
                    //���ο� ������Ʈ�� ����(�߰�)�Ѵ�
                    ObjectPool pool = FindObjectPool(poolName);
                    if (pool != null)
                    {
                        obj = CreateGameObject(pool.prefab);
                    }
#else
                    Debug.Log(poolName + " object pool is empty");
#endif
                }

                if (obj != null)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    obj.SetActive(true);
                }
            }
            else
            {
                Debug.LogError(poolName + " object pool is not available");
            }

            return obj;
        }

        private IEnumerator _despawn(GameObject poolObject, float timer)
        {
            yield return new WaitForSeconds(timer);

            if (objectPoolDictionary.ContainsKey(poolObject.name))
            {
                Queue<GameObject> poolQueue = objectPoolDictionary[poolObject.name];

                // ������Ʈ Ǯ�� �̹� ��� �Ǿ��ִ��� �˻�(�߿�)
                if (poolQueue.Contains(poolObject) == false)
                {
                    poolObject.transform.rotation = Quaternion.identity;
                    poolObject.transform.position = Vector3.zero;

                    objectPoolDictionary[poolObject.name].Enqueue(poolObject);
                    poolObject.SetActive(false);
                }
                else
                {
                    // �̹� ó����
                    Debug.LogWarning($"Already despawn {poolObject.name}");
                }
            }
            else
            {
                Debug.Log(poolObject.name + " object pool is not available");
            }
        }

        public void Despawn(GameObject poolObject, float timer = 0f)
        {
            StartCoroutine(_despawn(poolObject, timer));
        }
    }
}