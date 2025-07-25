using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleComponent : MonoBehaviour
{
    public UnityEvent OnEvent;
    public UnityEvent OffEvent;


    public void OnToggleChanged()
    {
        bool isOn = gameObject.GetComponent<Toggle>().isOn;
        if (isOn)
        {
            Debug.Log("Toggle is ON");
            OnEvent?.Invoke();
        }
        else
        {
            Debug.Log("Toggle is OFF");
            OffEvent?.Invoke();
        }
    }
}
