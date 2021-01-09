using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveNetworkedPropsPositons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/Networked Data.txt";
        string content = "";

        if (File.Exists(name) == false)
        {
            File.WriteAllText(path, content);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject child = transform.GetChild(i).gameObject;
            content += child.name;
            content += " ";
            content += child.transform.position.x.ToString();
            content += ":";
            content += child.transform.position.y.ToString();
            content += ":";
            content += child.transform.position.z.ToString();
            content += " ";
            content += child.transform.rotation.x.ToString();
            content += ":";
            content += child.transform.rotation.y.ToString();
            content += ":";
            content += child.transform.rotation.z.ToString();
            content += ":";
            content += child.transform.rotation.w.ToString();
            content += "\n";
        }

        File.AppendAllText(path, content);
    }

}
