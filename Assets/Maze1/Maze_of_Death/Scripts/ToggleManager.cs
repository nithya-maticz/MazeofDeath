using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ToggleManager : MonoBehaviour
{
    public List<Toggle> toggles;

    void Start()
    {
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    DeselectOthers(toggle);
                }
            });
        }
    }

    void DeselectOthers(Toggle selectedToggle)
    {
        foreach (Toggle toggle in toggles)
        {
            if (toggle != selectedToggle)
            {
                toggle.isOn = false;
            }
        }
    }
}
