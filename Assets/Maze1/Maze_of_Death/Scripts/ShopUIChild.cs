using UnityEngine;
 using UnityEngine.Events;

public class ShopUIChild : MonoBehaviour
{
    public UnityEvent SelectedEvents;
    public UnityEvent DeselectEvents;
    public bool selected;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        SelectedEvents?.Invoke();
    }

    public void Deselect()
    {
        DeselectEvents?.Invoke();
    }
}
