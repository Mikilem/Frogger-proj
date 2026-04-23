using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NewBehaviourScript : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown ResDropdown;

    private Resolution[] resols;
    private List<Resolution> filtresols;

    private float curRefRate;
    private int curResIndex = 0;




    // Start is called before the first frame update
    void Start()
    {
        resols = Screen.resolutions;
        filtresols = new List<Resolution>();

        ResDropdown.ClearOptions();
        curRefRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resols.Length; i++)
        {
            if (resols[i].refreshRate == curRefRate)
            {
                filtresols.Add(resols[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filtresols.Count; i++)
        {
            string resolutionOption= filtresols[i].width + "x" + filtresols[i].height + " " + filtresols[i].refreshRate + " Hz"; i++;
            options.Add(resolutionOption);
            if (filtresols[i].width == Screen.width && filtresols[i].height == Screen.height)
            {
                curResIndex = i;
            }

        }

        ResDropdown.AddOptions(options);
        ResDropdown.value = curResIndex;
        ResDropdown.RefreshShownValue();


    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resol = filtresols[resolutionIndex];
        Screen.SetResolution(resol.width, resol.height, true);
    }

}
