using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
         
    [SerializeField]
    Menu[] menus;

    void Awake()
    {
        instance = this;
    }

    public void OpenMenu(string name)
    {
        foreach (Menu m in menus)
        {
            if (m.mName == name)
            {
                m.Open();
            }
            else if (m.opened == true)
            {
                CloseMenu(m);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (Menu m in menus)
        {
            if (m.opened == true)
            {
                CloseMenu(m);
            }
        }

        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
