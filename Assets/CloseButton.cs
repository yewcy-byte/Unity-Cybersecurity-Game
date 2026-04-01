using UnityEngine;
using UnityEngine.UI;

public class ItemButtonController : MonoBehaviour
{
    public Item item; // reference to Item

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            item.Close();
        });
    }
}
