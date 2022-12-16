using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONReaderEntries : MonoBehaviour
{

    // Panel Game Object
    [Header("Entites Screen Game Object")]
    public GameObject EntriesScreen;

    [Header("Sharing Screen Screen Game Object")]
    public GameObject SharingScreen;

    EntriesMain _EntriesMain;

    public bool isPoolSelected;

    [Header("URL Of the website to gather the data from")]
    [SerializeField] private string URL;

    private string JsonFile;

    // The attribute [SerializeField] helps us to see the variable on the Unity Editor without the need to make the variable public.
    [SerializeField] private List<string> _associationID;
    private List<string> _amount;
    private List<string> _comment;
    private List<string> _date;
    private List<string> _name;

    [Header("The Pool ID to fetch the info from")]
    public string FetchPoolIdForEntries = "-1";

    public bool IsWorking { set; get; }

    [SerializeField] bool _IsFetchingData;

    public bool _ForceStop { private set; get; }
    [SerializeField] bool _HasInit;
    string _CachedCheckedId;

    public bool PoolButtonPressed;

    // The getters are a better way to gather the data in the future through other scripts without risking any user altering its contents.
    public List<string> GetEntriesId() { return _associationID; }

    public List<string> GetEntriesAmount() { return _amount; }

    public List<string> GetEntriesComment() { return _comment; }

    public List<string> GetEntriesDate() { return _date; }

    public List<string> GetEntriesName() { return _name; }

    void ResetLists()
    {
        // Init empty lists.
        _associationID = new List<string>();
        _amount = new List<string>();
        _comment = new List<string>();
        _date = new List<string>();
        _name = new List<string>();
    }

    public void OnExitButtonPress()
    {
        FetchPoolIdForEntries = "-1";
        EntriesScreen.gameObject.SetActive(false);
        SharingScreen.gameObject.SetActive(true);
        isPoolSelected = false;

        foreach (GameObject entry in _EntriesMain.EntriesList)
        {
            Destroy(entry);
        }

        ResetLists();
    }

    public void OnPoolButtonPressed()
    {
        PoolButtonPressed = true;
    }

    public void ForceEntryListRefresh()
    {
        foreach (GameObject entry in _EntriesMain.EntriesList)
        {
            Destroy(entry);
        }

        ResetLists();
        _HasInit = false;
    }

    void DoHTMLRequest()
    {
        if (FetchPoolIdForEntries != "-1" && !_IsFetchingData && !_HasInit) // if we got no pool id chosen, exit.
        {
            StartCoroutine(FetchJSON());

            _IsFetchingData = true;
        }

        if (_CachedCheckedId != FetchPoolIdForEntries)
        {
            _HasInit = false;
            _CachedCheckedId = FetchPoolIdForEntries;
        }

        if (_ForceStop)
        {
            StopCoroutine(FetchJSON());
            _ForceStop = false;
        }

    }
    
    IEnumerator FetchJSON()
    {
 
        string poolID = FetchPoolIdForEntries; // Gather the Pool Id

        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/"+poolID+"/entries"; // Gather the URL specific to the pool Id.

        Debug.Log("URL FOR READER ENTRIES: " + URL);

        UnityWebRequest webrequest = UnityWebRequest.Get(URL);
        webrequest.timeout = 5;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webrequest.error);
            yield break;
        }
        else
        {
            // Show results as text
            Debug.Log(webrequest.downloadHandler.text);
            JsonFile = webrequest.downloadHandler.text;
            yield break;
        }

    }

    void SaveHtmlDataToVariables()
    {
        if (FetchPoolIdForEntries == "-1") // if we got no pool id chosen, exit.
        {
            return;
        }

        if (JsonFile == null) // If the GET failed to gather the data, exit.
        {
            return;
        }
        else
        {
            string failMsg = "No entries for poolID: " + FetchPoolIdForEntries;
            if (JsonFile == failMsg)
            {
                Debug.Log(" >> WE RECEIVED AN ID NOT FOUND!");
                _ForceStop = true;
                _HasInit = true;
                _IsFetchingData = false;
                JsonFile = null; // Clear up to avoid coming back here.
                ResetLists();
                return;
            }

            //Debug.Log(JsonFile);
        }
        
        // Saves the JSON format text into the classes member variables. 
        // This is done by unity, and it's the reason why the Class Member variables hold the same names
        // as the JSON keys so it can match them and fill them correctly.
        EntriesData entriesDataInJson = JsonUtility.FromJson<EntriesData>(JsonFile);

        ResetLists();

        foreach (Entries entry in entriesDataInJson.Entries)
        {
            // Gather its contents and store them in our lists.
            _associationID.Add(entry.associationID);
            _amount.Add(entry.amount);
            _comment.Add(entry.comment);
            _date.Add(entry.date);
            _name.Add(entry.name);
        }

        IsWorking = false;
        JsonFile = null;
        _IsFetchingData = false;
        _HasInit = true;

        _EntriesMain.HasInitialized = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        FetchPoolIdForEntries = "-1";

        _EntriesMain = EntriesScreen.GetComponent<EntriesMain>();
    }

    // Update is called once per frame
    void Update()
    {

        if (PoolButtonPressed)
        {
            SharingScreen.gameObject.SetActive(false);
            EntriesScreen.gameObject.SetActive(true);
            Debug.Log("Pool Button Pressed");
            PoolButtonPressed = false;
        }

        if (isPoolSelected)
        {
            // Do a Web GET to gather the json data.
            DoHTMLRequest();


            // This is called here because it takes more than 1 frame for the webiste to respond to the GET request.
            // We need the GET request to reply first before we attempt to fill in the class data.
            SaveHtmlDataToVariables();
        }
    }
}
