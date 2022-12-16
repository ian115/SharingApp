using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSONReader : MonoBehaviour
{

    SharingsMain _SharesMainScreen;

    private JSONPost _JsonPost;

    [Header("URL Of the website to gather the data from")]
    [SerializeField] private string URL;

    private string JsonFile;

    // The attribute [SerializeField] helps us to see the variable on the Unity Editor without the need to make the variable public.
    [SerializeField] private List<string> _poolID;
    private List<string> _poolTitle;
    private List<string> _poolType;
    private List<string> _poolDescription;
    [SerializeField] private List<string> _AssociationId;

    public bool _DoPoolIdsExist { private set; get; } // To know when to fetch.
    public bool _IsFetchingData; // To track and avoid spam.
    private int _FetchID = 0;
    public bool isJoiningPool;

    private int _CachedPoolIdSize = 0;

    public bool IsWorking { set; get; }

    public bool ListIsRefreshing;

    public bool WaitBeforeFetch;
    float WaitTime = 1;
    float currentTimer = 1;


    // The getters are a better way to gather the data in the future through other scripts without risking any user altering its contents.
    public List<string> GetPoolTitle() { return _poolTitle; }

    public List<string> GetPoolID() { return _poolID; }

    public List<string> GetPoolType() { return _poolType; }

    public List<string> GetPoolDescription() { return _poolDescription; }

    public List<string> GetAssociationId() { return _AssociationId; }

    public void AddAssociationId(string id) { _AssociationId.Add(id); }

    public void RestartProcess()
    {
        _IsFetchingData = false;
        _FetchID = 0;
    }

    public void ResetAllData()
    {
        _poolID = new List<string>();
        _poolTitle = new List<string>();
        _poolType = new List<string>();
        _poolDescription = new List<string>();
        _AssociationId = new List<string>();
    }

    void DoHTMLRequest()
    {
        
        if(_JsonPost.GetPoolIDsFromPUT().Count <= 0)
        {
            // The return here is enough, but we make a boolean just in case we need to track this in the future.
            _DoPoolIdsExist = false;
            RestartProcess();
            return;
        }
        else
        {
            _DoPoolIdsExist = true;
        }

        if (_FetchID > 0)
        {
            if (_JsonPost.GetPoolIDsFromPUT().Count == 0)
            {
                RestartProcess(); // Reset in case the User delets all shares.
                return;
            }
        }

        if (!_IsFetchingData && _DoPoolIdsExist && _FetchID < _JsonPost.GetPoolIDsFromPUT().Count && !ListIsRefreshing)
        {
            // To avoid grabbing old data.
            if(WaitBeforeFetch)
            {
                if(currentTimer > 0)
                {
                    currentTimer -= Time.deltaTime;
                    return;
                }
                else
                {
                    currentTimer = WaitTime;
                    WaitBeforeFetch = false;
                }
            }

            StartCoroutine(FetchJSON());
            _IsFetchingData = true;
        }
        
    }

    IEnumerator FetchJSON()
    {

        // To avoid an array overrun.
        if(_FetchID > _JsonPost.GetPoolIDsFromPUT().Count)
        {
            yield break;
        }

        string poolID = _JsonPost.GetPoolIDsFromPUT()[_FetchID]; // Gather the Pool Id

        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/"+poolID; // Gather the URL specific to the pool Id.
        
        UnityWebRequest webrequest = UnityWebRequest.Get(URL);
        yield return webrequest.SendWebRequest();

        if(webrequest.result != UnityWebRequest.Result.Success)
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

            // Or retrieve results as binary data
            //byte[] results = webrequest.downloadHandler.data;
        }

    }

    void SaveHtmlDataToVariables()
    {

        if(!_DoPoolIdsExist) // If we got no Pool IDs (first time user), exit.
        {
            return;
        }
        
        if (JsonFile == string.Empty || JsonFile == null) // If the GET failed to gather the data, exit.
        {
            Debug.Log("[JSONReader.CS] [SaveHtmlDataToVariables] - JSON DATA FROM WEBSITE IS EMPTY OR INVALID.");
            
            return;
        }
        else
        {
            if (_FetchID < _JsonPost.GetPoolIDsFromPUT().Count)
            {
                string errorMsg = "Pool id: " + _JsonPost.GetPoolIDsFromPUT()[_FetchID] + " not found.";

                if (JsonFile == errorMsg)
                {
                    _JsonPost.GetPoolIDsFromPUT().RemoveAt(_FetchID);
                    Debug.Log("We removed the ID due to it not found.");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        if (ListIsRefreshing)
            return;

        // Saves the JSON format text into the classes member variables. 
        // This is done by unity, and it's the reason why the Class Member variables hold the same names
        // as the JSON keys so it can match them and fill them correctly.
        Pools poolDataInJson = JsonUtility.FromJson<Pools>(JsonFile);

        for (int i = 0; i < _poolID.Count; i++)
        {
            if (_poolID[i] == poolDataInJson.poolID)
            {
               return; // If the ID was already added, don't add it again.
            }
        }

        // Gather its contents and store them in our lists.
        _poolID.Add(poolDataInJson.poolID);
        _poolTitle.Add(poolDataInJson.title);
        _poolType.Add(poolDataInJson.type);
        _poolDescription.Add(poolDataInJson.description);
        //_AssociationId.Add(poolDataInJson.associations[_FetchID].aID);
        JsonFile = null;

        // For joins.
        if (isJoiningPool)
        {
            StartCoroutine(GatherAssociationsIdsWhenJoining());
        }
        else
        {
            AddAssociationId("1"); // ID = 1 (we made this entry)
            SaveSystem.SaveData(this); // Save the data.
            _FetchID++; // Fetch the next ID
            IsWorking = false;
        }
    }
    public void PerformManualSave()
    {
        SaveSystem.SaveData(this);
    }

    public IEnumerator GatherAssociationsIdsWhenJoining()
    {
        IsWorking = true;
        string poolId = _JsonPost.GetPoolIDsFromPUT()[_FetchID];

        string URL = "https://davoratski-web.ian114.repl.co/api/pools/get/" + poolId + "/association/add";

        Debug.Log("[GatherAssociationsIdsWhenJoining]: FOR ID: " + poolId);

        string emptyJson = "{\"associationsToAdd\": [{\"aName\": \"\",\"aNumber\": \"\",\"aBirthday\": \"\"}]}";

        UnityWebRequest webrequest = UnityWebRequest.Put(URL, emptyJson);
        webrequest.SetRequestHeader("Content-Type", "application/json");
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webrequest.error);
            Debug.Log("[GatherAssociationsIdsWhenJoining]: FAILED!");

            yield break;
        }
        else
        {
            Debug.Log("GatherAssociationsIdsWhenJoining: SUCCESS!");

            string id = webrequest.downloadHandler.text;
            Debug.Log("GatherAssociationsIdsWhenJoining: RESULT: " + id);


            id = id.Replace('"', ' ').Trim(); // Only solution that worked. Replace " for spaces and run Trim to remove spaces.

            // Get the return from the PUT

            Debug.Log("[GatherAssociationsIdsWhenJoining] - Association ID: " + id);

            _AssociationId.Add(id);
            SaveSystem.SaveData(this); // Save the data.
            _FetchID++;
            IsWorking = false;
            yield break;
        }

    }

    void UpdateStats()
    {
        if(_CachedPoolIdSize != _JsonPost.GetPoolIDsFromPUT().Count)
        {
            RestartProcess();
            _CachedPoolIdSize = _JsonPost.GetPoolIDsFromPUT().Count;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _JsonPost = GetComponent<JSONPost>();
        _SharesMainScreen = GameObject.FindGameObjectWithTag("SharingScreen").GetComponent<SharingsMain>();

        WaitTime = 1;
        currentTimer = WaitTime;

        // Init empty lists.
        _poolID = new List<string>();
        _poolTitle = new List<string>();
        _poolType = new List<string>();
        _poolDescription = new List<string>();
        _AssociationId = new List<string>();

        UserData data = SaveSystem.LoadData();

        if(data != null)
        {
            _poolID = data.poolId;
            _poolTitle = data.poolTitle;
            _poolType = data.poolType;
            _poolDescription = data.poolDescription;
            _AssociationId = data.entryAssociationId;

            Debug.Log("Data successfully loaded from file");
        }

    }

    // Update is called once per frame
    void Update()
    {

        // Do a Web GET to gather the json data.
        DoHTMLRequest();


        // This is called here because it takes more than 1 frame for the webiste to respond to the GET request.
        // We need the GET request to reply first before we attempt to fill in the class data.
        SaveHtmlDataToVariables();
        
        UpdateStats();
    }
}
