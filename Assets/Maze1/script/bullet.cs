using UnityEngine;

public class bullet : MonoBehaviour
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
        Debug.Log("Tigger");
        if (collision.gameObject.tag == "box")
        {

            Debug.Log("bullet attack!");
          
            //  animatorRef.SetTrigger("damage");

        }

    }
}
