using UnityEngine;
using UnityEngine.InputSystem;

public class ClearCounter : MonoBehaviour
{
    private int _clearCount;
    void Awake()
    {
        _clearCount = PlayerPrefs.GetInt("clearCount", 0);
    }

#if UNITY_EDITOR
    private void Update()
    {
        //テスト用
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Clear();
            Debug.Log($"クリア回数: {_clearCount}");
        }
    }
#endif

    /// <summary>
    /// クリアの際に一度だけ呼ぶ
    /// </summary>
    public void Clear()
    {
        ++_clearCount;
        PlayerPrefs.SetInt("clearCount", _clearCount);
    }

    public int GetClearCount()
    {
        return _clearCount;
    }
}
