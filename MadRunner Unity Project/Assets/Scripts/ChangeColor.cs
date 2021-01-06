using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public FlexibleColorPicker cp;

    public void ChangeCarColor()
    {

        Renderer r = gameObject.GetComponent<Renderer>();

        int range = r.materials.Length;

        for (int j = 0; j < range; ++j)
        {
            r.materials[j].color = cp.color;
        }
    }

}
