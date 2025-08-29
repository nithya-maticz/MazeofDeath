using System.Collections;
using UnityEngine;

public class TankBlaster : MonoBehaviour
{
    [SerializeField] Tanker tanker;
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
        Debug.Log("Collided : " + collision.gameObject.name);

        IGetBlastTank blastTank = collision.GetComponent<IGetBlastTank>();
        if (blastTank != null)
        {
            Debug.Log("Collided 1: " + collision.gameObject.name);
            // Delay calling TankBlastUpdate by 0.5s
            StartCoroutine(DelayBlast(blastTank, 0.5f));
        }
    }

    private IEnumerator DelayBlast(IGetBlastTank blastTank, float delay)
    {
        print("Delay1");
        yield return new WaitForSeconds(delay);
        print("Delay2");
        blastTank.TankBlastUpdate();
        tanker.shadow.SetActive(true);
        Destroy(gameObject);
        //this.gameObject.SetActive(false);
    }

}
