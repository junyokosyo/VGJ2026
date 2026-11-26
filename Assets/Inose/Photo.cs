using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class Photo : MonoBehaviour
{
    //Imageを入れる。12入れられる。
    [SerializeField] private Image[] _images = new Image[12];
    //表示させたい画像を入れる。
    [SerializeField] private Sprite[] _sprites;

    private const int _vertical = 3;
    private const int _horizontal = 4;

    private Image[,] _imageGrid = new Image[_vertical,_horizontal];

    private void Awake()
    {
        BuildGrid();
        ApplyRandomSprites();
    }

    private void BuildGrid()
    {
        if (_images == null || _images.Length < _vertical * _horizontal)
        {
            Debug.LogError("すべてのImageを設定してください。");
            return;
        }

        for (int r = 0; r < _vertical; r++)
        {
            for (int c = 0; c < _horizontal; c++)
            {
                int index = r * _horizontal + c;
                _imageGrid[r, c] = _images[index];
            }
        }
    }

    public void ApplyRandomSprites()
    {
        if (_sprites == null || _sprites.Length == 0)
        {
            Debug.LogWarning("イラストが設定されていません。");
            return;
        }

        for (int r = 0; r < _vertical; r++)
        {
            for (int c = 0; c < _horizontal; c++)
            {
                Image targetImage = _imageGrid[r, c];
                if (targetImage == null) continue;

                int _randomIndex = Random.Range(0, _sprites.Length);
                targetImage.sprite = _sprites[_randomIndex];
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
