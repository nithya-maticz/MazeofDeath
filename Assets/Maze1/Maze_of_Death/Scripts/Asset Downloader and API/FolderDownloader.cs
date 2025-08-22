/*

using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Threading.Tasks;

public class FolderDownloader : MonoBehaviour
{

    public static FolderDownloader Instance;
    [Tooltip("Direct GitHub RAW link to your ZIP file")]


    private string extractPath;


    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    *//*  public async void ExtractTile()
      {
  #if UNITY_EDITOR
          extractPath = Path.Combine(Application.dataPath, "Resources/MySprites");
  #else
          extractPath = Path.Combine(Application.persistentDataPath, "MySprites");
  #endif
          StartCoroutine(DownloadAndExtract());
      }*//*
    public Task ExtractTile()
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(ExtractTileRoutine(tcs));
        return tcs.Task;
    }

    private IEnumerator ExtractTileRoutine(TaskCompletionSource<bool> tcs)
    {
#if UNITY_EDITOR
        extractPath = Path.Combine(Application.dataPath, "Resources/MySprites");
#else
    extractPath = Path.Combine(Application.persistentDataPath, "MySprites");
#endif

        yield return DownloadAndExtract(); // Wait for the download + extraction

        tcs.SetResult(true); // Mark task as done
    }

    IEnumerator DownloadAndExtract()
    {
        string zipUrl = "https://raw.githubusercontent.com/kavinmaticz/unity-assets/main/Level" + LevelController.Instance.selectedLevel;
        print("Zip : " + zipUrl);
        UnityWebRequest www = UnityWebRequest.Get(zipUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Download failed: {www.error}");
            yield break;
        }

        byte[] data = www.downloadHandler.data;
        string zipPath = Path.Combine(Application.temporaryCachePath, "Level.zip");
        File.WriteAllBytes(zipPath, data);

        ClearFolder(extractPath);
        WebGLCacheClear.Instance.ClearCache();

        try
        {
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            Debug.Log($"Assets extracted to: {extractPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Extraction failed: {ex.Message}");
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        if (SpriteAssigner.Instance != null)
            SpriteAssigner.Instance.AssignData();
    }

    void ClearFolder(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        foreach (var file in Directory.GetFiles(path))
            File.Delete(file);

        foreach (var dir in Directory.GetDirectories(path))
            Directory.Delete(dir, true);
    }
}*/



using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FolderDownloader : MonoBehaviour
{
    public static FolderDownloader Instance;

    public string[] assetName;

    public static string S3Url = "https://mazeofdeath.s3.ap-south-1.amazonaws.com/ASSETS";

    public Sprite assignedImage;
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
        // LobbyManager.currentLevel = 1;
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
            string[] extensions = { ".png", ".jpg", ".jpeg" };

            foreach (string ext in extensions)
            {
                string url = S3Url + "/Level" + LobbyManager.currentLevel + "/" + assetName[i] + ext;

                if (await TryDownload(url))
                {
                    success = true;
                    if(assetName[i] == "101")
                    {
                        GridLoader.Instance.doorBgSprite = assignedImage;
                    }
                    else if(assetName[i] == "bg")
                    {
                        GridLoader.Instance.bgSprite = assignedImage;
                    }
                    break; // ✅ stop trying other extensions
                }
            }

            if (!success)
            {
                Debug.LogWarning("❌ No valid image found for " + assetName[i]);
            }

           
        }

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
