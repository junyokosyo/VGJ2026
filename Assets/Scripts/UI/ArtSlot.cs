using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デザイナーの1枚絵を入れる場所。
/// Image に絵（Sprite）を入れると絵が表示され、仮の見た目（Placeholder）が自動で消える。絵を外すと仮の見た目に戻る。
/// エディタ上でもすぐに切り替わる
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ArtSlot : MonoBehaviour
{
    [Header("絵が入ったら消す、仮の見た目")]
    [SerializeField] private GameObject _placeholder;

    /// <summary>
    /// 起動したときに、絵があるかどうかで表示を切り替える
    /// </summary>
    private void Awake()
    {
        Apply();
    }

    /// <summary>
    /// インスペクターで絵を入れた・外したときにも切り替える
    /// （OnValidate の中では表示の切り替えができないので、少し後で行う）
    /// </summary>
    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                Apply();
            }
        };
#endif
    }

    /// <summary>
    /// 絵があれば絵を出して仮の見た目を消す。なければその逆
    /// </summary>
    private void Apply()
    {
        Image image = GetComponent<Image>();
        bool hasArt = image.sprite != null;
        image.enabled = hasArt;

        if (_placeholder != null)
        {
            _placeholder.SetActive(!hasArt);
        }
    }
}
