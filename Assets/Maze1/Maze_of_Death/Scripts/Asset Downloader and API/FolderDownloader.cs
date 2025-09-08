


using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FolderDownloader : MonoBehaviour
{
    public static FolderDownloader Instance;

    public string[] assetName;

    public static string S3Url = "https://mazeofdeath.s3.ap-south-1.amazonaws.com/ASSETS";

    public Sprite assignedImage;
    public static Sprite bgSprite;
    public static Sprite doorSprite;
    public Tile bgTile;
    //string url = S3Url + "/Level"+ LevelController.Instance.selectedLevel + "/"+assetName[i] + ext;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       
       
    }

    public async Task StartDownLoad()
    {
        
        print("Current Lvl : " + LobbyManager.currentLevel);
        await DownloadImage();
        GridLoader.Instance.LoadLevel();
    }

    public async Task DownloadImage()
    {
        ClearSaveDirectory();

        for (int i = 0; i < assetName.Length; i++)
        {
            bool success = false;
            string[] extensions = { ".png"/*, ".jpg", ".jpeg"*/ };

            foreach (string ext in extensions)
            {
                string url = S3Url + "/Level" + LobbyManager.currentLevel + "/" + assetName[i] + ext;

                if (await TryDownload(url))
                {
                    success = true;
                    if(assetName[i] == "101")
                    {
                        GridLoader.Instance.doorBgSprite = assignedImage;
                        doorSprite = assignedImage;
                    }
                    else if(assetName[i] == "bg")
                    {
                        GridLoader.Instance.bgSprite = assignedImage;
                        bgSprite = assignedImage;
                        bgTile.sprite = bgSprite;
                    }
                    break; // ✅ stop trying other extensions
                }
            }

            if (!success)
            {
                Debug.LogWarning("❌ No valid image found for " + assetName[i]);
            }

           
        }

        LobbyManager.loadedLevel = LobbyManager.currentLevel;

        if (SpriteAssigner.Instance != null)
            SpriteAssigner.Instance.AssignData();
    }


    private async Task<bool> TryDownload(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            var op = www.SendWebRequest();

            while (!op.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Not found: " + url);
                return false;
            }

            // ✅ Got the image
            Texture2D tex = DownloadHandlerTexture.GetContent(www);
            byte[] data = tex.EncodeToPNG();

            string fileName = Path.GetFileName(url);

#if UNITY_EDITOR
            string savePath = Path.Combine(Application.dataPath, "Resources");
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string finalPath = Path.Combine(savePath, fileName);
            File.WriteAllBytes(finalPath, data);

            Debug.Log("✅ Saved to Resources: " + finalPath);
            UnityEditor.AssetDatabase.Refresh();
#else
            string savePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(savePath, data);
            Debug.Log("✅ Saved to persistentDataPath: " + savePath);
#endif

            Sprite sprt = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            // ✅ Assign sprite directly to a UI Image (if you want immediate preview)
          
            assignedImage = sprt;
            return true;
        }
    }


    public void ClearSaveDirectory()
    {
#if UNITY_EDITOR
        string savePath = Path.Combine(Application.dataPath, "Resources");
#else
    string savePath = Application.persistentDataPath;
#endif

        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true); // delete folder and all files
        }
        Directory.CreateDirectory(savePath); // recreate empty folder

        Debug.Log("🧹 Cleared old files in: " + savePath);
    }
}
