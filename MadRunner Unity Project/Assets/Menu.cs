using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string mName;
    public bool openned;

    public void Open()
    {
        openned = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        openned = false;
        gameObject.SetActive(false);
    }
}
