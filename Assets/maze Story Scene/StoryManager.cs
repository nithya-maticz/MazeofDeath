using Unity.Cinemachine;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
   /* public Player malePlayer;
    public Player femalePlayer;
    public Transform playerSpawnPoint;*/

    public CinemachineCamera cineCam;
    public Joystick joystick;
    public GameObject enemy;
    public static StoryManager Instance;
    public GameObject box_heart;
    public GameObject fillImage;
    public GameObject box_key;

    public GameObject CollideCircle;
    public GameObject FillCanvas;
    public GameObject CollideCircle1;
    public GameObject FillCanvas1;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        Instance = this;
    }
    public void FirstBox()
    {
        CollideCircle.SetActive(false);
        FillCanvas.SetActive(false);
        FindObjectOfType<KeyBox>().EndAnimation();
            
    }
    public void SecondBox()
    {
        CollideCircle1.SetActive(false);
       
        FindObjectOfType<KeyBox1>().EndAnimation();
    }
}
