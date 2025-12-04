using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static bool HasInstance => _instance != null;
    private static readonly object Lock = new object();
    private static bool _applicationIsQuitting;

    // DontDestroy 여부
    protected virtual bool Persistent => true;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.Log(
                    $"[MonoSingleton]Instance '{typeof(T)}' already destroyed on application quit.");
                return _instance;
            }

            lock (Lock)
            {
                if (_instance)
                    return _instance;

                _instance = (T)FindAnyObjectByType(typeof(T));

                if (!_instance)
                {
                    _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();

                    if (!_instance)
                    {
                        Debug.LogError("[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    }

                    Debug.Log($"[MonoSingleton]An instance of {typeof(T)} is needed in the scene, so '{_instance.name}' was created with DontDestroyOnLoad.");
                }

                if (FindObjectsByType(typeof(T), FindObjectsSortMode.None).Length > 1)
                {
                    Debug.LogError(
                        $"[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                }

                if (_instance.Persistent) DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance &&
            _instance != this)
        {
            Debug.LogWarning($"{typeof(T)} already exist!");
            Destroy(gameObject);
            return;
        }

        if (!_instance)
        {
            _instance = (T)this;
            if (Persistent)
                DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}