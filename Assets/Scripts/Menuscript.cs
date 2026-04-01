using UnityEngine;
using UnityEngine.InputSystem;

public class Menuscript : MonoBehaviour
{
public GameObject menuCanvas;



    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
         if ((Keyboard.current.tabKey.wasPressedThisFrame))
        {
       OpenMenu();     
        }
       
    }

    public void OpenMenu()
    {
        
        

            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
        
    }
}
