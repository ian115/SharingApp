using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : MonoBehaviour
{

    public GameObject JoinPanel;

    public GameObject CreatePanel;

    public Image GrayScale;

    private void OnEnable()
    {
        GrayScale.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        GrayScale.gameObject.SetActive(false);
    }

    public void OnJoinButtonPress()
    {
        JoinPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnCreateButtonPress()
    {

        CreatePanel.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnCancelButtonPress()
    {
        this.gameObject.SetActive(false);
    }
}
