using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public FlexibleColorPicker cp;

    public void ChangeCarColor()
    {
        gameObject.GetComponent<Renderer>().materials[0].color = cp.color;
    }

}
