using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public static bool OnTutorial;
    public bool IsNewUser;

    [Header("Pages")]
    public GameObject LoadingPage;
    public GameObject StoryPage;
    public GameObject LobbyPage;

    [Header("Fader")]
    public Animator Fader;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (!OnTutorial)
        {
            LoadingPage.SetActive(true);
            StartCoroutine(LoadLobbyWithDelay());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadLobbyWithDelay()
    {
        yield return new WaitForSeconds(2f);
        OnLoadLobby();
    }

    void OnLoadLobby()
    {
        if (IsNewUser)
        {
            LoadingPage.SetActive(false);
            VideoController.Instance.PlayNextVideo();
        }
        else
        {
            Fade();
            LoadingPage.SetActive(false);
            LobbyPage.SetActive(true);
        }
    }

    public void Fade()
    {
        Fader.gameObject.SetActive(true);
        Fader.SetTrigger("Fade");
    }
    public void PlayGame()
    {
        Fade();
        SceneManager.LoadScene("Game");
    }
   
}
