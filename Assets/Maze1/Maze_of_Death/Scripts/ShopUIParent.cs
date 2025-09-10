using UnityEngine;
using UnityEngine.UI;

public class ShopUIParent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {

            Button button = child.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnElementClicked(child));
            }

            if (child.GetComponent<ShopUIChild>().selected)
            {
                OnElementClicked(child);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnElementClicked(Transform clickedElement)
    {

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i);
            if (child != clickedElement)
            {
                child.GetComponent<ShopUIChild>().Deselect();
            }
            else
            {
                child.GetComponent<ShopUIChild>().Select();
            }

        }
        

    }
}
