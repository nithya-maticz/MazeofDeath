using UnityEngine;

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

}
