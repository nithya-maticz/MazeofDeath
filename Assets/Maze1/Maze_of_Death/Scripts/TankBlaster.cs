using UnityEngine;

public class TankBlaster : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colloide : " + collision.gameObject.name);
        if(collision.GetComponent<IGetBlastTank>() != null)
        {
            collision.GetComponent<IGetBlastTank>().TankBlastUpdate();
        }
    }
}
