using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public static int currentLevel;
    public static int loadedLevel;
    public static bool OnTutorial;
    public static bool OnRetry;
    public bool IsNewUser;

    [Header("Pages")]
    public GameObject LoadingPage;
    public GameObject StoryPage;
    public GameObject LoginPage;
    public GameObject LobbyPage;
    public GameObject charPage;
    public TMP_Text levelText;

    [Header("Character Selection")]
    public static int currentCharacter;
   

    [Header("Lobby")]
    public GameObject maleAniChar;
    public GameObject femaleAniChar;
    public Image lobbyCharIcon;
    public Image lobbyCharName;
    public Sprite lobbyMaleIcon;
    public Sprite lobbyMaleName;
    public Sprite lobbyFemaleIcon;
    public Sprite lobbyFemaleName;


    [Header("Fader")]
    public Animator Fader;

   

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
        if(!PlayerPrefs.HasKey("Localdata") && !OnTutorial)
        {
            //New User
            print("=======> New User <=======");
            LoadingPage.SetActive(true);
            StartCoroutine(StoryWithDelay());
        }
        else if (OnTutorial && !PlayerPrefs.HasKey("Localdata"))
        {
            LoginPage.SetActive(true);
            
        }
        else if(PlayerPrefs.HasKey("Localdata"))
        {
            GetLocalData();
            
            LobbyPage.SetActive(true);
            changeLobby();
            levelText.text = "Level " + currentLevel.ToString();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetLocalData()
    {
        string savedJson = PlayerPrefs.GetString("Localdata", "{}");
        LocalData loaded = JsonUtility.FromJson<LocalData>(savedJson);
        print("Level : " +  loaded.level);
        print("Avatar : " +  loaded.avatar);
        currentLevel = loaded.level;
        currentCharacter = loaded.avatar;
    }

    public void NewData()
    {
        LocalData data = new LocalData();
        data.level = 1;
        data.avatar = currentCharacter;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("Localdata", json);
        PlayerPrefs.Save();
        currentLevel = data.level;
        print(PlayerPrefs.GetString("Localdata"));
        levelText.text = "Level " + currentLevel.ToString();
    }

    private IEnumerator StoryWithDelay()
    {
        yield return new WaitForSeconds(2f);
        OnVideoPlay();
    }
    
    void OnVideoPlay()
    {
        Fade();
        LoadingPage.SetActive(false);
        VideoController.Instance.PlayNextVideo();
    }

    public void Fade()
    {
        Fader.gameObject.SetActive(true);
        Fader.SetTrigger("Fade");
    }
    public void PlayGame()
    {
        Fade();
        SceneManager.LoadScene("Game Loader");
    }
   public void OpenCharacterPage()
   {
        Fade();
       
        charPage.SetActive(true);
        
   }

   

    public void Character(int index)
    {
        currentCharacter = index;
    }

    public void selectCharacter()
    {
        Fade();
        Debug.Log(currentCharacter); 
        LobbyPage.SetActive(true);
        changeLobby();
        NewData();

    }
    public void changeLobby()
    {
        
        if (currentCharacter == 1)
        {
            maleAniChar.SetActive(true);
            femaleAniChar.SetActive(false);
            lobbyCharIcon.sprite = lobbyMaleIcon;
            lobbyCharName.sprite = lobbyMaleName;

        }
        else if (currentCharacter == 2)
        {
            maleAniChar.SetActive(false);
            femaleAniChar.SetActive(true);
            lobbyCharIcon.sprite = lobbyFemaleIcon;
            lobbyCharName.sprite = lobbyFemaleName;
        }

    }

}


[Serializable]
public class LocalData
{
    public int level;
    public int avatar;
}
