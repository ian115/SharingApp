using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static JSONReader;

public class JSONPost : MonoBehaviour
{

    JSONReaderEntries _WorldDataEntries;
    JSONReader _WorldData;

    private string _PoolTitleValue;
    private string _DescriptionValue;
    private bool _IsDivide;

    public List<string> PoolIDFromPUT;
    

    public List<string> GetPoolIDsFromPUT()
    {
        return PoolIDFromPUT;
    }

    public void AddPoolIDFakePut(string poolId)
    {
        PoolIDFromPUT.Add(poolId);
    }

    [Header("URL Of the website to POST the data to")]
    public string URL;

    [SerializeField] private bool _Processing;

    public void JsonPost(string poolTitleValue, string descriptionValue, bool isDivide)
    {
        _PoolTitleValue = poolTitleValue;
        _DescriptionValue = descriptionValue;
        _IsDivide = isDivide;

        string optionChosen;

        if(_IsDivide)
        {
            optionChosen = "Divide";
        }
        else
        {
            optionChosen = "Contribute";
        }

        Debug.Log("Is this running?");

        //string data = "{title': "+"'"+_PoolTitleValue+"'"+",'type': "+"'"+optionChosen+"'"+",'description': "+"'"+_DescriptionValue+"'" +",'associationsAddCount': 1,'associations': [{'aID': 1,'aName': 'Sarah','aNumber': '07732211212','aBirthday': '19/12/2001'}, {'aID': 2,'aName': 'Belmont'}]}"+"'";
        string data = "{\"title\": " + "\"" + _PoolTitleValue + "\"" + ",\"type\": " + "\"" + optionChosen + "\"" + ",\"description\": " + "\"" + _DescriptionValue + "\"" + ",\"associationsAddCount\": 1,\"associations\": [{\"aID\": 1,\"aName\": \"Sarah\",\"aNumber\": \"07732211212\",\"aBirthday\": \"19/12/2001\"}]}";

        Debug.Log(data);

        // To avoid repeating this if they spam the Create button.
        if(!_Processing)
        {
            Debug.Log("JSONPOST");
            StartCoroutine(PostJSON(data));
            _WorldData.IsWorking = true;
            _Processing = true;
        }
        
    }

    IEnumerator PostJSON(string data)
    {

        if(URL == null)
        {
            URL = "http://127.0.0.1:8080/api/pools/add";
            Debug.Log("PostJson: URL NOT SET - Using localhost..");
        }
        else
        {
            Debug.Log("PostJson: URL SET");
        }
        
        UnityWebRequest webrequest = UnityWebRequest.Put(URL, data);
        webrequest.SetRequestHeader("Content-Type", "application/json");
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webrequest.error);
            _Processing = true;
        }
        else
        {
            Debug.Log("PostJson: SUCCESS!");

            string id = webrequest.downloadHandler.text;

            id = id.Replace('"', ' ').Trim(); // Only solution that worked. Replace " for spaces and run Trim to remove spaces.

            // Get the return from the PUT
            PoolIDFromPUT.Add(id);
            _Processing = false;
            _WorldData.IsWorking = false;
            _WorldData._IsFetchingData = false;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        PoolIDFromPUT = new List<string>();
        _WorldDataEntries = GetComponent<JSONReaderEntries>();
        _WorldData = GetComponent<JSONReader>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
