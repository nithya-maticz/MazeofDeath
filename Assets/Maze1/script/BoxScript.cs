using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    int randomNumber;
   public List<GameObject> boxList = new List<GameObject>();
    

    void Start()
    {
        randomNumber = Random.Range(0, 5);
        KeyFun();
    }
    public void KeyFun()
    {

        Debug.Log( "Random Number" + randomNumber);
        for(int i=0;i< boxList.Count;i++)
        {
            if(randomNumber==i)
            {
               
                boxList[i].tag = "keys";
                Debug.Log(boxList[i].tag);
            }
            else
            {
              
                boxList[i].tag = "box";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
