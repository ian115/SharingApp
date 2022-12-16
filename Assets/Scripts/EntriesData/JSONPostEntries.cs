using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class JSONPostEntries : MonoBehaviour
{
    private string _EntryAmountValue;
    private string _EntryCommentValue;

    private JSONReaderEntries _WorldDataEntries;
    private JSONReader _WorldDataPool;

    private string URL;

    private bool _Processing;

    public void JsonPostEntry(string entryAmountValue, string entryCommentValue)
    {
        _EntryAmountValue = entryAmountValue;
        _EntryCommentValue = entryCommentValue;

        Debug.Log("[JSON-POST-ENTRY] RUNNING..");

        int fetchIndexForList = 0;

        // Loop through the Pool IDs, check in which position of the List<> we are in and use that index to gather the Association ID at that index.
        for(int i = 0; i < _WorldDataPool.GetPoolID().Count; i++)
        {
            if (_WorldDataPool.GetPoolID()[i] == _WorldDataEntries.FetchPoolIdForEntries)
            {
                fetchIndexForList = i;
            }
        }

        string data = "{\"addEntries\": [{\"aID\": " + "\"" + _WorldDataPool.GetAssociationId()[fetchIndexForList] + "\"" +  ", \"amount\": "+"\""+_EntryAmountValue + "\"" + ", \"comment\": " + "\"" + _EntryCommentValue + "\"" + "}]}";

        Debug.Log(data);

        // To avoid repeating this if they spam the Create button.
        if (!_Processing)
        {
            Debug.Log("[JSON-POST-ENTRY] Process Post.");
            StartCoroutine(PostJSON(data));
            _Processing = true;
        }

    }

    IEnumerator PostJSON(string data)
    {
        string poolId = _WorldDataEntries.FetchPoolIdForEntries;
        
        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/"+poolId+"/entries/add";

        UnityWebRequest webrequest = UnityWebRequest.Put(URL, data);
        webrequest.SetRequestHeader("Content-Type", "application/json");
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webrequest.error);
            _Processing = false;
            yield break;
        }
        else
        {
            Debug.Log("PostJsonEntries: SUCCESS!");
            _Processing = false;
            _WorldDataEntries.ForceEntryListRefresh(); // To restart the process.
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _WorldDataEntries = GetComponent<JSONReaderEntries>();
        _WorldDataPool = GetComponent<JSONReader>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
