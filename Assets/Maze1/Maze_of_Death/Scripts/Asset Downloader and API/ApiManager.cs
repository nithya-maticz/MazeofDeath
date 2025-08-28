using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;
    public static string baseURL = "https://mazetest.onrender.com";

    [Header("Level Data")]
    public LevelRootWrapper allLevels;
    public LevelEntry levelData;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //LobbyManager.currentLevel = 1;
        GetLevels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetLevels()
    {
        StartCoroutine(GetLevelsData());
    }
    IEnumerator GetLevelsData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(baseURL + "/api/levels/?level=" + LobbyManager.currentLevel))
        {

            // webRequest.SetRequestHeader("Authorization", "Bearer " + localData.token);


            yield return webRequest.SendWebRequest();


            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");

            }
            else
            {
                string rawJson = webRequest.downloadHandler.text;

                // Wrap the raw array in an object so JsonUtility can parse it
                string wrappedJson = "{\"items\":" + rawJson + "}";

                LevelRootWrapper wrapper = JsonUtility.FromJson<LevelRootWrapper>(wrappedJson);
                if (wrapper.items != null && wrapper.items.Count > 0 && wrapper.items[0].level.Count > 0)
                {
                    levelData = wrapper.items[0].level[0];
                    PathManager.Instance.ClearCache();
                    TilemapPathfinding.instance.LoadBlockedPrefabs();
                    GridLoader.Instance.patrolPoints = levelData.data.patrolPoints;
                    GridLoader.Instance.playerSpawn = levelData.data.playerSpawn;
                    FolderDownloader.Instance.StartDownLoad();
                    Debug.Log("Player Spawn: " + levelData.data.playerSpawn.x + "," + levelData.data.playerSpawn.y);
                }
                else
                {
                    Debug.LogError("No levels found in JSON!");
                }

                /*allLevels = wrapper;
                List<LevelRoot> levels = wrapper.items;
                GetMyLevel();*/
            }
        }
    }

    public void GetMyLevel()
    {
        for (int i = 0; i < allLevels.items.Count; i++)
        {
            if (allLevels.items[i].level[0].level == LobbyManager.currentLevel)
            {
                levelData = allLevels.items[i].level[0];
                TilemapPathfinding.instance.LoadBlockedPrefabs();
                PathManager.Instance.ClearCache();
                GridLoader.Instance.patrolPoints = levelData.data.patrolPoints;
                GridLoader.Instance.playerSpawn = levelData.data.playerSpawn;
                FolderDownloader.Instance.StartDownLoad();
                break;
            }
        }
    }

}



[Serializable]
public class LevelRootWrapper
{
    public List<LevelRoot> items;
}

[Serializable]
public class LevelRoot
{
    public string _id;
    public List<LevelEntry> level = new List<LevelEntry>();
    public string createdAt;
    public string updatedAt;
    public int __v;
}

[Serializable]
public class LevelEntry
{
    public int level;
    public LevelData data = new LevelData();
    public string _id;
}

[Serializable] public class TileData { public Vector2Int position; public int tileIndex; public int tilemapType; public int rotation; public bool flipX; public bool flipY; }
[Serializable] public class PlacedData { public Vector2Int position; public int prefabIndex; public Vector2Int size; public int rotation; }

[Serializable]
public class LevelData
{
    public int width, height;
    public List<TileData> tiles = new List<TileData>();
    public List<PlacedData> prefabs = new List<PlacedData>();
    public List<Vector2Int> patrolPoints = new List<Vector2Int>();
    public int selectedBackgroundIndex;
    public Vector2Int playerSpawn = new Vector2Int();
}

[Serializable]
public class CreateLevelPayload
{
    public List<LevelEntryPayload> level = new List<LevelEntryPayload>();
}

[Serializable]
public class CreatedResponse
{
    public string message;
}

[Serializable]
public class LevelEntryPayload
{
    public int level;
    public LevelData data = new LevelData();
}

