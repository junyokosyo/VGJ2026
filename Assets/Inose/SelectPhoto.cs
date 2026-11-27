using UnityEngine;
using UnityEngine.UI;

public class SelectPhoto : MonoBehaviour
{
    [Header("表示先のImageの名前入れる")]
    [SerializeField] private string _ImageName = "PreviewImage";

    [Header("対象の親オブジェクト名を指定する場合（空なら直上の親）")]
    [SerializeField] private string _targetParentName = "";

    private Image _targetImage;
    private GameObject _backgroundObject;
    private Button _selfButton;
    private Image _selfImage;

    private void Awake()
    {
        _selfButton = GetComponent<Button>();
        _selfImage = GetComponent<Image>();

        if (_selfButton != null)
        {
            _selfButton.onClick.AddListener(OnButtonClicked);
        }
    }

    private void Start()
    {
        FindTargetImageAndBackground();
    }

    private void FindTargetImageAndBackground()
    {
        _targetImage = null;
        _backgroundObject = null;

        Image[] images = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Image img in images)
        {
            if (img.gameObject.name == _ImageName)
            {
                _targetImage = img;
                _backgroundObject = ResolveBackgroundObject(img.transform);
                return;
            }
        }
        Debug.LogWarning($"シーン内に「{_ImageName}」というImageが見つかりません。");
    }

    private GameObject ResolveBackgroundObject(Transform targetTransform)
    {
        if (targetTransform == null || targetTransform.parent == null)
        {
            return null;
        }
        // 特定の親の名前が指定されている場合はその名前の親を探す
        if (!string.IsNullOrEmpty(_targetParentName))
        {
            Transform current = targetTransform.parent;
            while (current != null)
            {
                if (current.name == _targetParentName)
                {
                    return current.gameObject;
                }
                current = current.parent;
            }
            return null;
        }

        // 指定がない場合は直上の親を背景として扱う
        return targetTransform.parent.gameObject;
    }

    private void OnButtonClicked()
    {
        if (_targetImage == null)
        {
            FindTargetImageAndBackground();
        }

        if (_targetImage == null)
        {
            Debug.LogWarning("表示先のImageを取得できませんでした。");
            return;
        }

        // 押されたボタン自身の画像を貼り付けて表示する
        _targetImage.sprite = _selfImage.sprite;
        _targetImage.preserveAspect = true;
        _targetImage.gameObject.SetActive(true);
        // 背景（親オブジェクト）を表示する
        SetBackgroundActive(true);
    }

    /// <summary>
    /// 閉じるボタンなどから呼び出して、背景ごと非表示にしたい場合に使用
    /// </summary>
    public void HideBackground()
    {
        SetBackgroundActive(false);
    }

    public void SetBackgroundActive(bool isActive)
    {
        if (_backgroundObject == null && _targetImage != null)
        {
            _backgroundObject = ResolveBackgroundObject(_targetImage.transform);
        }

        if (_backgroundObject != null)
        {
            _backgroundObject.SetActive(isActive);
        }
    }

    private void OnDestroy()
    {
        if (_selfButton != null)
        {
            _selfButton.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
