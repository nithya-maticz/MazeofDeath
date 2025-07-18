using UnityEngine;
using System.Collections;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;
    public static bool OnTutorial;
    public bool IsNewUser;

    [Header("Pages")]
    public GameObject LoadingPage;
    public GameObject StoryPage;

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

    private IEnumerator LoadLobbyWithDelay()
    {
        yield return new WaitForSeconds(2f);
        OnLoadLobby();
    }

    void OnLoadLobby()
    {
        //LoadingPage.SetActive(false);

        if (IsNewUser)
        {
            StoryPage.SetActive(true);
            VideoController.Instance.PlayNextVideo();
        }
    }
}
