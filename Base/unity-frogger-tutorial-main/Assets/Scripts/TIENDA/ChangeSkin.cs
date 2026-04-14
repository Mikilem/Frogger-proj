using UnityEngine;

using UnityEngine;
using System.Collections;


public class GUITest : MonoBehaviour
{

    public GUISkin mySkin;

    void OnGUI()
    {
        // Assign the skin to be the one currently used.
        GUI.skin = mySkin;

        // Make a button. This will get the default "button" style from the skin assigned to mySkin.
        GUI.Button(new Rect(10, 10, 150, 20), "Skinned Button");
    }

    void Skin()
    {
        Sprite rendered = Frogger;
    }
}