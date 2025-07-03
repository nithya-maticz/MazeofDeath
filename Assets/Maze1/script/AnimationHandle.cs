using UnityEngine;
using UnityEngine.UI;

public class AnimationHandle : MonoBehaviour
{
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
        Player.Instance.IncreaseHealth();
    }

    public void EnemyAttackCount()
    {
        ManagerMaze.instance.bloodImage.GetComponent<Image>().enabled = true;
        Player.Instance.PlayerHealthCount--;
        
       // Debug.Log(Player.Instance.PlayerHealthCount);
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
                break;


        }


    }

}
