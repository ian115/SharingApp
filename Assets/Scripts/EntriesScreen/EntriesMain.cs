using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntriesMain : MonoBehaviour
{
    public TextMeshProUGUI PanelTitle;

    JSONReaderEntries _WorldDataEntries;
    JSONReader _WorldDataPools;

    public GameObject CreatePanel;

    [Header("The entry prefab to instantiate")]
    public GameObject EntryPrefab;

    [Header("Where to instantiate the Pool Objects")]
    public GameObject PoolTableParent;

    [SerializeField] private int _EntriesCount;

    public bool HasInitialized;

    public List<GameObject> EntriesList;

    string _CachedFetchedPoolId;

    bool _TitleUpdated;

    [Header("The Description of the Pool")]
    public TextMeshProUGUI PoolDescription;

    public void OnAddShareButtonPress()
    {
        CreatePanel.SetActive(true);
    }

    public void CreateNewEntry(int iPoolIndex)
    {

        GameObject tmp = Instantiate(EntryPrefab, PoolTableParent.transform);

        foreach (TextMeshProUGUI myText in tmp.GetComponentsInChildren<TextMeshProUGUI>())
        {
            Debug.Log("I IS >>>>>>>> " + iPoolIndex);

            switch (myText.name)
            {
                case "Name":
                    myText.text = _WorldDataEntries.GetEntriesName()[iPoolIndex];
                    Debug.Log("POOL TITLE >> " + _WorldDataEntries.GetEntriesName()[iPoolIndex]);
                    break;
                case "Comment":
                    myText.text = _WorldDataEntries.GetEntriesComment()[iPoolIndex];
                    Debug.Log("POOL TYPE >>>>>>>> " + _WorldDataEntries.GetEntriesComment()[iPoolIndex]);
                    break;
                case "Amount":
                    myText.text = _WorldDataEntries.GetEntriesAmount()[iPoolIndex];
                    break;
                case "Date":
                    myText.text = _WorldDataEntries.GetEntriesDate()[iPoolIndex];
                    break;
                case "Completed":
                    myText.text = "...";
                    break;
            }
        }

        EntriesList.Add(tmp);
    }

    /// <summary>
    /// The function below is for the users who previously used the app and need their data
    /// loaded and their game objects created as they previously were.
    /// </summary>
    void SetupOnInit()
    {
        if (!HasInitialized && _WorldDataEntries.GetEntriesId().Count > 0)
        {
            for (int i = 0; i < _WorldDataEntries.GetEntriesId().Count; i++)
            {
                CreateNewEntry(i);
            }

            // Init
            _EntriesCount = _WorldDataEntries.GetEntriesId().Count;
            HasInitialized = true;
            _CachedFetchedPoolId = _WorldDataEntries.FetchPoolIdForEntries;
        }
    }

    void UpdatePanelInfo()
    {
        if(_WorldDataEntries.isPoolSelected)
        {
            string title = "Title Here";
            string description = "Description Here..";

            for (int i = 0; i < _WorldDataPools.GetPoolID().Count; i++)
            {
                if (_WorldDataPools.GetPoolID()[i] == _WorldDataEntries.FetchPoolIdForEntries)
                {
                    title = _WorldDataPools.GetPoolTitle()[i];
                    description = _WorldDataPools.GetPoolDescription()[i];
                }
            }

            PanelTitle.text = title;
            PoolDescription.text = description;
            _TitleUpdated = true;
        }
        else
        {
            if(_TitleUpdated)
            {
                _TitleUpdated = false;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _WorldDataPools = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReader>();
        _WorldDataEntries = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReaderEntries>();

        EntriesList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        SetupOnInit();

        if (_WorldDataEntries.GetEntriesId().Count > 0 && _WorldDataEntries.GetEntriesId().Count != _EntriesCount && HasInitialized)
        {
            if (!_WorldDataEntries.IsWorking) // Only if it finished processing everything..
            {
                CreateNewEntry(_WorldDataEntries.GetEntriesId().Count - 1);
                _EntriesCount = _WorldDataEntries.GetEntriesId().Count;
            }
        }

        UpdatePanelInfo();
    }
}
