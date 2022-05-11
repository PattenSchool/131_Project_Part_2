using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    private SettingsManager settingsManager;
    private TextMeshProUGUI myTMPComponent;
    public Material outlineSimpleFontMaterial;
    public Material outlineFancyFontMaterial;


    // Start is called before the first frame update
    void Start()
    {
        //1
        settingsManager = GameObject.FindGameObjectWithTag("SettingsManager").GetComponent<SettingsManager>();
        myTMPComponent = gameObject.GetComponent<TextMeshProUGUI>();
        //2
        myTMPComponent.font = settingsManager.fontStyleSelected;
        myTMPComponent.fontSize = settingsManager.fontSizeSelected;

        switch (settingsManager.fontStyleSelected.name)
        {
            case "LiberationSans SDF":
                myTMPComponent.fontMaterial = outlineSimpleFontMaterial;
                break;
            case "UnifrakturMaguntia - Book SDF":
                myTMPComponent.fontMaterial = outlineFancyFontMaterial;
                break;
            default:
                myTMPComponent.fontMaterial = outlineFancyFontMaterial;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
