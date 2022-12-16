using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatePool : MonoBehaviour
{

    [SerializeField] SharingsMain PoolPanel;

    // Important!
    [SerializeField] JSONPost _WorldDataPost;

    // JSONRead
    JSONReader _WorldDataPostRead;

    public TMP_InputField PoolTitleInputField;

    public TMP_InputField DescriptionInputField;

    public Image DivideButton;

    public Image ContributeButton;

    public Image GrayScale;

    public string _PoolTitleValue { get; private set; }
    public string _DescriptionValue { get; private set; }

    [SerializeField] bool _isDivide;

    public TextMeshProUGUI _ErrorMsg;

    public void OnCancelButtonPress()
    {
        // On Cancel, empty fields and close Panel.
        PoolTitleInputField.text = "";
        DescriptionInputField.text = "";

        this.gameObject.SetActive(false);
    }

    void SaveData()
    {
        _PoolTitleValue = PoolTitleInputField.text;
        _DescriptionValue = DescriptionInputField.text;

        if(_isDivide)
        {
            DivideButton.color = new Color32(51, 88, 171, 255);
            ContributeButton.color = Color.white;
        }
        else
        {
            DivideButton.color = Color.white;
            ContributeButton.color = new Color32(51, 88, 171, 255);
        }
    }

    public void OnDivideButtonPress()
    {
        _isDivide = true;
    }

    public void OnContributeButtonPress()
    {
        _isDivide = false;
    }

    private void OnEnable()
    {
        GrayScale.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        GrayScale.gameObject.SetActive(false);
    }

    public void OnCreateButtonPress()
    {

        if(_PoolTitleValue == null)
        {
            _ErrorMsg.text = "Error: Pool Title is empty!";
        }

        if(_DescriptionValue == null)
        {
            _ErrorMsg.text = "Error: Description is empty!";
        }


        if(_PoolTitleValue != null && _DescriptionValue != null)
        {
            // Send information and create a POST.
            _WorldDataPostRead.IsWorking = true; // To stop anything from creating while this loads.
            _WorldDataPost.JsonPost(_PoolTitleValue, _DescriptionValue, _isDivide);
            OnCancelButtonPress();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPost>();
        _WorldDataPostRead = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();

        PoolPanel = GameObject.FindGameObjectWithTag("SharingScreen").GetComponent<SharingsMain>();
    }

    // Update is called once per frame
    void Update()
    {
        SaveData();
    }
}
