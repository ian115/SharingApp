using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SharingsMain : MonoBehaviour
{

    // List with all the Instantiated Pools
    public List<GameObject> ListOfPools;

    JSONReader _WorldData;

    JSONPost _WorldDataPost;

    public GameObject CreatePanel;

    public GameObject PoolPrefab;

    [Header("Where to instantiate the Pool Objects")]
    public GameObject PoolTableParent;

    [SerializeField] public int _PoolCount;

    public bool HasInitialized;

    public void OnAddShareButtonPress()
    {
        CreatePanel.SetActive(true);
    }

    public void CreateNewPool(int iPoolIndex)
    {
        GameObject tmp = Instantiate(PoolPrefab, PoolTableParent.transform);

        PoolSingleData PoolData = tmp.GetComponent<PoolSingleData>();

        foreach (TextMeshProUGUI myText in tmp.GetComponentsInChildren<TextMeshProUGUI>())
        {
            Debug.Log("I IS >>>>>>>> " + iPoolIndex);

            switch (myText.name)
            {
                case "Title":
                    myText.text = _WorldData.GetPoolTitle()[iPoolIndex];
                    Debug.Log("POOL TITLE >> " + _WorldData.GetPoolTitle()[iPoolIndex]);
                    break;
                case "Type":
                    myText.text = _WorldData.GetPoolType()[iPoolIndex];
                    Debug.Log("POOL TYPE >>>>>>>> " + _WorldData.GetPoolType()[iPoolIndex]);
                    break;
                case "Amount":
                    myText.text = "£0";
                    break;
            }
        }

        // We cache all the info when we create the pool prefab (share).
        PoolData.SetMyPoolId(_WorldData.GetPoolID()[iPoolIndex]);
        PoolData.SetMyAssociationId(_WorldData.GetAssociationId()[iPoolIndex]);
        PoolData.SetMyPoolDescription(_WorldData.GetPoolDescription()[iPoolIndex]);
        PoolData.SetMyPoolTitle(_WorldData.GetPoolTitle()[iPoolIndex]);
        ListOfPools.Add(tmp);
    }

    /// <summary>
    /// The function below is for the users who previously used the app and need their data
    /// loaded and their game objects created as they previously were.
    /// </summary>
    void SetupOnInit()
    {
        if(!HasInitialized && _WorldData.GetPoolID().Count > 0 && _WorldData.GetAssociationId().Count > 0)
        {
            for(int i = 0; i < _WorldData.GetPoolID().Count; i++)
            {
                CreateNewPool(i);
            }

            // Init
            _PoolCount = _WorldData.GetPoolID().Count;
            HasInitialized = true;
        }
    }


    public void RefreshEntireListOfPools()
    {
        _WorldData.ResetAllData();
        _WorldDataPost.PoolIDFromPUT = new List<string>();

        for (int i = 0; i < ListOfPools.Count; i++)
        {
            // Below we fake a GET (this is how we force a refresh).
            _WorldDataPost.GetPoolIDsFromPUT().Add(ListOfPools[i].GetComponent<PoolSingleData>().GetMyPoolId());
            Destroy(ListOfPools[i]);
        }

        ListOfPools = new List<GameObject>();

        _PoolCount = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        _WorldData = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();
        _WorldDataPost = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONPost>();
    }

    // Update is called once per frame
    void Update()
    {
        SetupOnInit();

        if(_WorldData.GetPoolID().Count != _PoolCount)
        {
            if(!_WorldData.IsWorking && !_WorldData.ListIsRefreshing) // Only if it finished processing everything..
            {
                if (_WorldData.GetAssociationId().Count > 0)
                {
                    CreateNewPool(_WorldData.GetPoolID().Count - 1);
                    _PoolCount = _WorldData.GetPoolID().Count;
                }
                
            }
        }
    }
}
