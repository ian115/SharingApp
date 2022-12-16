using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppGlobalData : MonoBehaviour
{

    private bool _DarkThemeApplied;

    public static bool isDarkThemeOn;

    [Header("All Objects to apply the Dark Theme to")]
    public List<Image> Elements;

    Color oldColor;

    public static bool GetIsPlayerOnDarkTheme()
    {
        return isDarkThemeOn;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_DarkThemeApplied && GetIsPlayerOnDarkTheme())
        {
            foreach(Image image in Elements)
            {
                if (oldColor == null)
                {
                    oldColor = image.color;
                }

                image.color = new Color(0.2352941f, 0.2313726f, 0.2313726f, 0.9843137f);
            }

            _DarkThemeApplied = true;
        }

        if (_DarkThemeApplied && !GetIsPlayerOnDarkTheme())
        {
            foreach (Image image in Elements)
            {
               
                image.color = new Color(1f, 1f, 1f, 0.3921569f);
            }

            _DarkThemeApplied = false;
        }

    }
}
