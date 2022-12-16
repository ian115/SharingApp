using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateEntry : MonoBehaviour
{

    // Important!
    JSONPostEntries _WorldDataPost;

    // JSONRead
    JSONReaderEntries _WorldDataRead;

    public TMP_InputField EntryAmountInputField;

    public TMP_InputField CommentInputField;

    public Image GrayScale;

    public string _EntryAmountValue { get; private set; }
    public string _EntryCommentValue { get; private set; }

    public TextMeshProUGUI _ErrorMsg;

    public void OnCancelButtonPress()
    {
        // On Cancel, empty fields and close Panel.
        EntryAmountInputField.text = "";
        CommentInputField.text = "";
        _ErrorMsg.text = "";

        this.gameObject.SetActive(false);
    }


    void SaveData()
    {
        _EntryAmountValue = EntryAmountInputField.text;
        _EntryCommentValue = CommentInputField.text;
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

        if (_EntryAmountValue == null)
        {
            _ErrorMsg.text = "Error: Entry Amount is empty!";
        }

        if (_EntryCommentValue == null)
        {
            _ErrorMsg.text = "Error: Entry Comment is empty!";
        }


        if (_EntryAmountValue != null && _EntryCommentValue != null)
        {
            // Send information and create a POST.
            _WorldDataRead.IsWorking = true; // To stop anything from creating while this loads.
            _WorldDataPost.JsonPostEntry(_EntryAmountValue, _EntryCommentValue);
            OnCancelButtonPress();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPostEntries>();
        _WorldDataRead = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReaderEntries>();

    }

    // Update is called once per frame
    void Update()
    {
        SaveData();
    }
}
