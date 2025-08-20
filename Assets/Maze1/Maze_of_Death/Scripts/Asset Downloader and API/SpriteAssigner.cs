using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
public class SpriteAssigner : MonoBehaviour
{
    public static SpriteAssigner Instance;
    public LevelAssetsDatabase database;
    public List<Sprite> extractImages = new List<Sprite>();
    //public Tile[] tile;


    private void Awake()
    {
        Instance = this;
    }

    public void AssignData()
    {
        extractImages.Clear();

#if UNITY_EDITOR
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("");

        List<Sprite> numericSprites = new List<Sprite>();
        List<Sprite> otherSprites = new List<Sprite>();

        foreach (var spr in loadedSprites)
        {
            if (int.TryParse(spr.name, out _))
                numericSprites.Add(spr);
            else
                otherSprites.Add(spr);
        }

        // ✅ Sort numerics by actual number (not string!)
        numericSprites.Sort((a, b) =>
            int.Parse(a.name).CompareTo(int.Parse(b.name)));

        // Alphabetical for others
        otherSprites.Sort((a, b) =>
            string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));

        extractImages.AddRange(numericSprites);
        extractImages.AddRange(otherSprites);

#else
    string savePath = Application.persistentDataPath;
    Debug.Log("Loading sprites from: " + savePath);

    List<string> files = new List<string>();
    files.AddRange(Directory.GetFiles(savePath, "*.png"));
    files.AddRange(Directory.GetFiles(savePath, "*.jpg"));
    files.AddRange(Directory.GetFiles(savePath, "*.jpeg"));

    List<(int num, string path)> numericFiles = new List<(int, string)>();
    List<string> otherFiles = new List<string>();

    foreach (var file in files)
    {
        string name = Path.GetFileNameWithoutExtension(file);
        if (int.TryParse(name, out int num))
            numericFiles.Add((num, file));
        else
            otherFiles.Add(file);
    }

    // ✅ Numeric sort by int
    numericFiles.Sort((a, b) => a.num.CompareTo(b.num));

    // Alphabetical for non-numeric
    otherFiles.Sort((a, b) =>
        string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b),
        System.StringComparison.OrdinalIgnoreCase));

    // Load numeric first
    foreach (var nf in numericFiles)
    {
        byte[] imgData = File.ReadAllBytes(nf.path);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imgData);

        Sprite sprt = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));
        sprt.name = nf.num.ToString();
        extractImages.Add(sprt);
    }

    // Then non-numeric
    foreach (var file in otherFiles)
    {
        byte[] imgData = File.ReadAllBytes(file);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imgData);

        Sprite sprt = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f));
        sprt.name = Path.GetFileNameWithoutExtension(file);
        extractImages.Add(sprt);
    }
#endif

        Debug.Log($"✅ Loaded {extractImages.Count} sprites");

        // Clear old UI tiles
      

        for (int j = 0; j < extractImages.Count; j++)
        {
            database.tiles[j].sprite = extractImages[j];
        }
    }




    public void DestroyChildObjects(Transform parent)
    {
        for (int i = 0;  i< parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

   
}


