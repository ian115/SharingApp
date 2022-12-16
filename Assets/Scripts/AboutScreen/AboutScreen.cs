using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutScreen : MonoBehaviour
{

    [Header("The extra menu screen")]
    public GameObject MoreScreen;

    public void OnGoBackButtonPress()
    {
        this.gameObject.SetActive(false);
        MoreScreen.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
