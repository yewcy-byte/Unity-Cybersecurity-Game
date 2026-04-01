using UnityEngine;
using System.Collections.Generic;

public class SQLpagecontroller : MonoBehaviour
{
    public GameObject[] panels;

    private int currentIndex = 0 ;


public void next()
    {
        currentIndex++;
        if (currentIndex >= panels.Length)
        {
            currentIndex = panels.Length -1;
        }
        showPanel(currentIndex);
    }


public void previous()
    {

        currentIndex--;

                if (currentIndex < 0)
        {
            currentIndex = 0 ;
        }
        showPanel(currentIndex);
    }


     void showPanel(int i)
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
            

        panels[i].SetActive(true);

    }
}
