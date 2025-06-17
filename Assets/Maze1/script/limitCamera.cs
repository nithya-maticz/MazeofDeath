using UnityEngine;

public class limitCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerRef;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LateUpdate()
    {
        transform.position = new Vector3(playerRef.transform.position.x,40f, playerRef.transform.position.z);
    }
}
