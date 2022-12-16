using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoolDetailsDisplay : MonoBehaviour
{
    public Pool pool;

    public TMP_Text titleText;
    public TMP_Text typeText;
    public TMP_Text amountText;

    public Image myImage;
    Color myColorRed;
    Color myColorYellow;
    Color myColorGreen;

    public void updatePoolDisplay()
    {
        //titleText.text = pool.title;
        //typeText.text = pool.type;
        //amountText.text = "£ " + pool.amount.ToString();

        if (pool.amount < 0)
        {
            myImage.color = myColorRed;
        }
        else if (pool.amount > 0)
        {
            myImage.color = myColorYellow;
        }
        else
        {
            myImage.color = myColorGreen;
        }
    }
    // Start is called before the first frame update   
    void Start()
    {
        myImage = gameObject.GetComponent<Image>(); //gameObject grabs THIS object.
        myColorRed = new Color32(226, 159, 159, 255); //Red
        myColorYellow = new Color32(226, 190, 159, 255); //Yellow amber
        myColorGreen = new Color32(172, 226, 159, 255); //Green
        updatePoolDisplay();
    }

}
