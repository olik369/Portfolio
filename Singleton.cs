using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //Destroy ���� Ȯ�ο�
    private static bool _ShuttingDown = false;
    private static object _Lock = new object();
    private static T _Instance;

    public static T Instance
    {
        get
        {
            // ���� ���� �� Object ���� �̱����� OnDestroy�� ���� ����� ���� ����
            // �ش� �̱����� gameObject.Ondestroy()������ ������� �ʰų� ����Ѵٸ� null üũ�� ������
            if (_ShuttingDown)
            {
                Debug.Log($"[Singletone] Instance '{ typeof(T) }' already destroyed. Returning null.");
                return null;
            }

            lock (_Lock)
            {
                if (_Instance == null)
                {
                    //�ν��Ͻ� ���� ���� Ȯ��
                    _Instance = (T)FindObjectOfType(typeof(T));

                    //���� �������� �ʾҴٸ� �ν��Ͻ� ����
                    if (_Instance == null)
                    {
                        //���ο� ���ӿ�����Ʈ�� ���� �̱����� ����
                        var singletonObject = new GameObject();
                        _Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singletone)";

                        //������ �ν��Ͻ��� ���� �ٲ� �����ϰԲ� ��
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _Instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        _ShuttingDown = true;
    }

    private void OnDestroy()
    {
        _ShuttingDown = true;
    }
}
