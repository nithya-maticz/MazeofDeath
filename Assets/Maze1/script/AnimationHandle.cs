using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AnimationHandle : MonoBehaviour
{
    public int VideoCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player playerRef;
    public StoryPlayer storyPlayer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColliderVisibleFun()
    {

        if(playerRef != null)
            playerRef.playerCollider.SetActive(true);
        else if(storyPlayer != null)
            storyPlayer.playerCollider.SetActive(true);
    }
    public void ColliderInvisibleFun()
    {
        if (playerRef != null)
            playerRef.playerCollider.SetActive(false);
        else if (storyPlayer != null)
            storyPlayer.playerCollider.SetActive(false);
    }

    void Key()
    {
        ManagerMaze.instance.TreasureAnimation.gameObject.SetActive(false);
        ManagerMaze.instance.TreasureKey.SetActive(false);
        ManagerMaze.instance.keyImg.SetActive(true);
        ManagerMaze.instance.isPlayerGetKey = true;
    }

    void Medikit()
    {
        ManagerMaze.instance.TreasureAnimation.gameObject.SetActive(false);
        ManagerMaze.instance.TreasureMedikit.SetActive(false);
        ManagerMaze.instance.bloodImage.GetComponent<Image>().enabled = false;
        Player.Instance.PlayerHealthCount=4;

        
    }
    public void NextVideo()
    {
        Debug.Log("Next");
        if(ManagerMaze.instance.loadVideo)
        {
            ManagerMaze.instance.loadVideo = false;
            ManagerMaze.instance.textMeshPro.text = "";
            ManagerMaze.instance.rawImage.GetComponent<RawImage>().texture = ManagerMaze.instance.videoPlayer[ManagerMaze.instance.count].videoTexture;
            ManagerMaze.instance.videoPlayer[ManagerMaze.instance.count].videoPlayer.Play();
        }
        else
        {
            ManagerMaze.instance.textMeshPro.text = "";
            ManagerMaze.instance.count++;
            Debug.Log("COUNT  ---- " + ManagerMaze.instance.videoPlayer.Count);
            Debug.Log("COUNT  ---- " + ManagerMaze.instance.count);
            if (ManagerMaze.instance.videoPlayer.Count > ManagerMaze.instance.count && ManagerMaze.instance.gameStart == false)
            {

                ManagerMaze.instance.rawImage.GetComponent<RawImage>().texture = ManagerMaze.instance.videoPlayer[ManagerMaze.instance.count].videoTexture;
                ManagerMaze.instance.videoPlayer[ManagerMaze.instance.count].videoPlayer.Play();
            }
        }






    }
    public void nextTyping()
    {
        ManagerMaze.instance.FadeImg.SetActive(false);
        if (ManagerMaze.instance.videoPlayer.Count > ManagerMaze.instance.count &&  ManagerMaze.instance.gameStart == false)
        {
            ManagerMaze.instance.StartTyping();
        }

    }
    public void EnemyAttackCount()
    {
        ManagerMaze.instance.bloodImage.GetComponent<Image>().enabled = true;
       


        int index = Player.Instance.PlayerHealthCount-1 ;
        Player.Instance.PlayerHealthCount = index;
        if (index >= 0 && index < ManagerMaze.instance.bloodSprite.Count)
        {
            Debug.Log("Setting sprite based on health");
            ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.bloodSprite[index];
            if(index==0)
            {
                Debug.Log("gameOver");
               // ManagerMaze.instance.GameOver();
            }

        }

        Debug.Log(Player.Instance.PlayerHealthCount);

      


    }

}
