using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public FlexibleColorPicker cp;
    public GameObject carGeometry;

    public void ChangeCarColor()
    {
        int childCount = carGeometry.transform.childCount;

        for(int i = 0; i < childCount; ++i)
        {
            GameObject go = carGeometry.transform.GetChild(i).gameObject;
            Renderer r = go.GetComponent<Renderer>();

            int range = r.materials.Length;

            for (int j = 0; j < range; ++j)
            {
                r.materials[j].color = cp.color;
            }
        }
    }
}
