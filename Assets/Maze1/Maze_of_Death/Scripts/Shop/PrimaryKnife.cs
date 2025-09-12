using UnityEngine;

public class PrimaryKnife : MonoBehaviour
{
    public static PrimaryKnife Instance;
    public Weapon data;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
