using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinPool : MonoBehaviour
{

    public TMP_InputField JoinPoolId;

    public TextMeshProUGUI _ErrorMsg;

    string _JoinPoolId;

    // JSONRead
    JSONReader _WorldDataRead;
    
    JSONPost _WorldDataPost;

    public Image GrayScale;

    private void OnEnable()
    {
        GrayScale.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        GrayScale.gameObject.SetActive(false);
    }

    public void OnCancelButtonPress()
    {
        // On Cancel, empty fields and close Panel.
        JoinPoolId.text = "";
        _ErrorMsg.text = "";

        this.gameObject.SetActive(false);
    }

    void SaveData()
    {
        _JoinPoolId = JoinPoolId.text;
    }

    public void OnJoinButtonPress()
    {

        if (_JoinPoolId == null)
        {
            _ErrorMsg.text = "Error: Entry Amount is empty!";
            _ErrorMsg.gameObject.SetActive(true);
        }

        if (_JoinPoolId != null)
        {
            // Send information by faking a "Post".
            // The normal post would return us a pool Id, but in this case we already have it so we 
            // skip the process by passing it directly to the list.
            // The reader will detect the increment of the Pool Ids from the POST list and add it in.

            bool alreadyExist = false;

            for(int i = 0; i < _WorldDataPost.GetPoolIDsFromPUT().Count; i++)
            {
                if(_JoinPoolId == _WorldDataPost.GetPoolIDsFromPUT()[i])
                {
                    alreadyExist = true;
                }
            }

            if(!alreadyExist)
            {
                _WorldDataPost.AddPoolIDFakePut(_JoinPoolId);
                _WorldDataRead.isJoiningPool = true;
                OnCancelButtonPress();
            }
            else
            {
                _ErrorMsg.text = "Error: Already joined this Pool ID!";
                _WorldDataRead.isJoiningPool = false;
                _ErrorMsg.gameObject.SetActive(true);
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPost>();
        _WorldDataRead = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();
    }

    // Update is called once per frame
    void Update()
    {
        SaveData();
    }
}
