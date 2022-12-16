using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScreen : MonoBehaviour
{

    public Slider ThemeSlider;
    public GameObject ExtraMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnGoBackButton()
    {
        this.gameObject.SetActive(false);
        ExtraMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Implement better dark theme.
        if(ThemeSlider.value == 1)
        {
            //AppGlobalData.isDarkThemeOn = true;
        }
        else
        {
            //AppGlobalData.isDarkThemeOn = false;
        }
    }
}
