using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public ZombieDoor door;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUp()
    {
        if (Game_Manager.Instance.IsGetKey && Game_Manager.Instance.IsShowKey)
            PlayerMovements.Instance.CloseDoor(door);
    }
}
