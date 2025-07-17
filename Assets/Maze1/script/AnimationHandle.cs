using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AnimationHandle : MonoBehaviour
{
    public int VideoCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Player playerRef;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColliderVisibleFun()
    {
        playerRef.playerCollider.SetActive(true);
    }
    public void ColliderInvisibleFun()
    {
        playerRef.playerCollider.SetActive(false);
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
        Player.Instance.PlayerHealthCount--;
        
        Debug.Log(Player.Instance.PlayerHealthCount);
        switch (Player.Instance.PlayerHealthCount)
        {
            case 4:
               // ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.damage1;
                Debug.Log("Health is between  4");
                break;
            case 3:
                ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.damage1;
                // Handle case when health is 3 or 4
                Debug.Log("Health is between 3 and 4");
                break;

            case 2:
                ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.damage2;
                Debug.Log("Health is 2");
                break;

            case 1:
                ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.damage3;
                Debug.Log("Health is 1");
                break;

            case 0:
                ManagerMaze.instance.bloodImage.sprite = ManagerMaze.instance.damage4;
                Debug.Log("Player dead");
                ManagerMaze.instance.GameOver();
                break;


        }


    }

}
