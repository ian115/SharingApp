using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ModifyPool : MonoBehaviour
{

    public string POOL_ID_TO_MODIFY;

    // Important!
    JSONPost _WorldDataPost;

    // JSONRead
    JSONReader _WorldData;

    SharingsMain _SharingsScreen;

    public TMP_InputField PoolTitleInputField;

    public TMP_InputField PoolDescriptionInputField;

    public Image GrayScale;

    public string _PoolTitleValue { get; private set; }
    public string _PoolDescriptionValue { get; private set; }

    public string InitialPoolTitleValue;
    public string InitialPoolDescriptionValue;

    public TextMeshProUGUI _ErrorMsg;

    bool _InitValues;

    string URL;

    public void OnCancelButtonPress()
    {
        // On Cancel, empty fields and close Panel.
        PoolTitleInputField.text = "";
        PoolDescriptionInputField.text = "";
        _ErrorMsg.text = "";

        this.gameObject.SetActive(false);
    }


    void SaveData()
    {

        if(!_InitValues)
        {
            if(InitialPoolTitleValue != "")
            {
                PoolTitleInputField.text = InitialPoolTitleValue;
                PoolDescriptionInputField.text = InitialPoolDescriptionValue;
                _InitValues = true;
            }
        }


        _PoolTitleValue = PoolTitleInputField.text;
        _PoolDescriptionValue = PoolDescriptionInputField.text;
    }

    private void OnEnable()
    {
        GrayScale.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        GrayScale.gameObject.SetActive(false);
    }

    public void OnModifyButtonPress()
    {

        if (_PoolTitleValue == null)
        {
            _ErrorMsg.text = "Error: Pool Title is empty!";
        }

        if (_PoolDescriptionValue == null)
        {
            _ErrorMsg.text = "Error: Description is empty!";
        }


        if (_PoolTitleValue != null && _PoolDescriptionValue != null)
        {
            // Send information and create a POST.
            _WorldData.IsWorking = true; // To stop anything from creating while this loads.
            _WorldData.ListIsRefreshing = true;
            // Do Post Here
            StartCoroutine(ModifyAssignedPool());
        }

    }

    IEnumerator ModifyAssignedPool()
    {
        string poolId = POOL_ID_TO_MODIFY;

        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/" + poolId + "/modify";

        string data = "{\"title\": \"" + _PoolTitleValue + "\", \"description\" : \"" + _PoolDescriptionValue + "\"}";

        UnityWebRequest webrequest = UnityWebRequest.Put(URL, data);
        webrequest.SetRequestHeader("Content-Type", "application/json");
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("[ModifyPool] >> Failed!");

            Debug.Log(webrequest.error);
            yield break;
        }
        else
        {
            Debug.Log("[ModifyPool] >> SUCCESS!");
            // RefreshEntireListOfPools
            _WorldData.WaitBeforeFetch = true;
            _SharingsScreen.RefreshEntireListOfPools();
            _WorldData.RestartProcess();
            _WorldData.IsWorking = false;
            _WorldData.ListIsRefreshing = false;
            MoreScreenMain.LocalUserState = MoreScreenMain.APP_USER_STATE.USER_STATE_NONE;
            Destroy(this.gameObject);
            yield break;
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPost>();
        _WorldData = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();
        _SharingsScreen = GameObject.FindGameObjectWithTag("SharingScreen").GetComponent<SharingsMain>();
    }

    // Update is called once per frame
    void Update()
    {
        SaveData();
    }
}
