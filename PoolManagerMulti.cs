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
        // 오브젝트 풀 리스트. 에디터(인스펙터창)에서 채워 넣습니다. 코드로 초기화 할 수 있음 
        public List<ObjectPool> ObjectPoolList;

        // 프리팹 이름으로 모든 오브젝트 풀을 저장
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

        // 오브젝트를 생성한다
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

                // 오브젝트 풀을 만들고 여기에 오브젝트들을 저장한다
                Queue<GameObject> poolQueue = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    poolQueue.Enqueue(CreateGameObject(pool.prefab));
                }

                // Dictionary 에 오브젝트 풀 추가
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
                    // 오브젝트 풀이 있을때 오브젝트를 반환한다
                    obj = poolQueue.Dequeue();
                }
                else
                {
                    // 오브젝트 풀이 비었을때 
#if EXTEND_POOL
                    //새로운 오브젝트를 생성(추가)한다
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

                // 오브젝트 풀에 이미 등록 되어있는지 검사(중요)
                if (poolQueue.Contains(poolObject) == false)
                {
                    poolObject.transform.rotation = Quaternion.identity;
                    poolObject.transform.position = Vector3.zero;

                    objectPoolDictionary[poolObject.name].Enqueue(poolObject);
                    poolObject.SetActive(false);
                }
                else
                {
                    // 이미 처리됨
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