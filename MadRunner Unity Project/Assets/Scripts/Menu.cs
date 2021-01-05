using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string mName;
    public bool opened;

    public void Open()
    {
        opened = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        opened = false;
        gameObject.SetActive(false);
    }
}
