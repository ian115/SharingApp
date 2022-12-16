using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PoolSingleData : MonoBehaviour
{
    [SerializeField] private string MyPoolId;
    [SerializeField] private string MyAssociationIdToThisPool;
    [SerializeField] private string MyPoolTitle;
    [SerializeField] private string MyPoolDescription;

    SharingsMain _SharingsMain;

    JSONReaderEntries _WorldDataEntries;
    JSONPost _WorldDataPost;
    JSONReader _WorldData;

    public TMP_Text TotalAmount;

    private string URL;

    [SerializeField] private float _CurrentTime;
    private int _WaitTime;

    [Header("Button used to remove a share")]
    public Button DeleteButton;

    [Header("Button used to modify a share")]
    public Button ModifyButton;

    [Header("The Modify Form")]
    public GameObject ModifyForm;

    private void Start()
    {
        _WorldDataEntries = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReaderEntries>();
        _WorldData = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPost>();
        _SharingsMain = GameObject.FindGameObjectWithTag("SharingScreen").GetComponent<SharingsMain>();
        _WaitTime = 5;
    }

    void GatherTotalAmount()
    {
        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/" + GetMyPoolId() +"/associations/" + GetMyAssociationId() +"/amount";

        if(GetMyPoolId() != null && GetMyAssociationId() != null) // Safe check
        {
            if(_CurrentTime < 0) // To avoid spamming every frame, we send a repeated request with a 1 second interval.
            {
                _CurrentTime = _WaitTime;
                StartCoroutine(FetchAmount());
            }
            else
            {
                _CurrentTime -= Time.deltaTime;
            }
        }

    }

    IEnumerator FetchAmount()
    {

        UnityWebRequest webrequest = UnityWebRequest.Get(URL);
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webrequest.error);
            yield break;
        }
        else
        {
            string failMsg = "No entries for poolID: " + GetMyPoolId();
            // Show results as text
            if (webrequest.downloadHandler.text != failMsg)
            {
                Debug.Log(webrequest.downloadHandler.text);
                TotalAmount.text = webrequest.downloadHandler.text;
            }
            yield break;
        }

    }

    void UpdateShareHUD()
    {
        if(MoreScreenMain.LocalUserState == MoreScreenMain.APP_USER_STATE.USER_STATE_DELETE_SHARES)
        {
            ModifyButton.gameObject.SetActive(false);
            DeleteButton.gameObject.SetActive(true);
        }
        else if(MoreScreenMain.LocalUserState == MoreScreenMain.APP_USER_STATE.USER_STATE_MODIFY_SHARES)
        {
            DeleteButton.gameObject.SetActive(false);
            ModifyButton.gameObject.SetActive(true);
        }
        else
        {
            DeleteButton.gameObject.SetActive(false);
            ModifyButton.gameObject.SetActive(false);
        }
    }

    public void OnDeleteThisPoolButtonPress()
    {
        StartCoroutine(DeleteLocalPool());
    }

    IEnumerator DeleteLocalPool()
    {
        string poolId = GetMyPoolId();

        URL = "https://davoratski-web.ian114.repl.co/api/pools/get/" + poolId + "/association/remove";

        string data = "{\"associationsToRemove\": [" + GetMyAssociationId() + "]}";

        UnityWebRequest webrequest = UnityWebRequest.Put(URL, data);
        webrequest.SetRequestHeader("Content-Type", "application/json");
        webrequest.timeout = 10;
        yield return webrequest.SendWebRequest();

        if (webrequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("[DeleteLocalPool] >> Failed!");

            Debug.Log(webrequest.error);
            yield break;
        }
        else
        {
            Debug.Log("[DeleteLocalPool] >> SUCCESS!");
            

            int GatherIndexForDeletion = 0;

            for(int i = 0; i < _WorldData.GetPoolID().Count; i++)
            {
                if(GetMyPoolId() == _WorldData.GetPoolID()[i])
                {
                    GatherIndexForDeletion = i;
                }
            }

            _WorldData.GetPoolID().RemoveAt(GatherIndexForDeletion);
            _WorldData.GetPoolTitle().RemoveAt(GatherIndexForDeletion);
            _WorldData.GetPoolType().RemoveAt(GatherIndexForDeletion);
            _WorldData.GetPoolDescription().RemoveAt(GatherIndexForDeletion);
            _WorldData.GetAssociationId().RemoveAt(GatherIndexForDeletion);

            _WorldDataPost.GetPoolIDsFromPUT().Remove(GetMyPoolId());
            _SharingsMain._PoolCount = _WorldData.GetPoolID().Count;
            _WorldData.PerformManualSave();
            MoreScreenMain.LocalUserState = MoreScreenMain.APP_USER_STATE.USER_STATE_NONE;
            Destroy(this.gameObject);
            yield break;
        }

    }

    public void OnModifyButtonPress()
    {
        // First enable the new modify form - once that is accepted we send a PUT.

        GameObject MyModifyForm = Instantiate(ModifyForm, _SharingsMain.transform);

        MyModifyForm.gameObject.SetActive(true);
        MyModifyForm.GetComponent<ModifyPool>().InitialPoolTitleValue = GetMyPoolTitle();
        MyModifyForm.GetComponent<ModifyPool>().InitialPoolDescriptionValue = GetMyPoolDescription();
        MyModifyForm.GetComponent<ModifyPool>().POOL_ID_TO_MODIFY = GetMyPoolId();
    }

    private void Update()
    {
        GatherTotalAmount();
        UpdateShareHUD();
    }

    public void SetMyPoolId(string poolId)
    {
        MyPoolId = poolId;
    }

    public void SetMyAssociationId(string poolAssociationId)
    {
        MyAssociationIdToThisPool = poolAssociationId;
    }

    public void SetMyPoolTitle(string poolTitle)
    {
        MyPoolTitle = poolTitle;
    }

    public void SetMyPoolDescription(string poolDescription)
    {
        MyPoolDescription = poolDescription;
    }

    public string GetMyPoolTitle() { return MyPoolTitle; }
    public string GetMyPoolDescription() { return MyPoolDescription; }

    public string GetMyPoolId() { return MyPoolId; }

    public string GetMyAssociationId() { return MyAssociationIdToThisPool; }

    public void OnThisPoolButtonPressed()
    {
        _WorldDataEntries.isPoolSelected = true;
        _WorldDataEntries.PoolButtonPressed = true;
        _WorldDataEntries.FetchPoolIdForEntries = MyPoolId;
        _WorldDataEntries.ForceEntryListRefresh(); // To restart the process.
    }

}
