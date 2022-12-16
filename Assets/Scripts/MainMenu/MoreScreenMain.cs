using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreScreenMain : MonoBehaviour
{
    [Header("The Sharing Screen")]
    public GameObject SharingScreen;

    [Header("The Entries Screen")]
    public GameObject EntriesScreen;

    JSONReaderEntries _WorldData;

    [Header("Sharing Pool Options")]
    public GameObject SharingPoolsOptions;

    [Header("Remove Pool Option")]
    public GameObject RemoveSharingPoolOption;

    [Header("Modify Pool Option")]
    public GameObject ModifySharingPoolOption;

    [Header("Go Back Option")]
    public GameObject GoBackToExtraMenu;

    [Header("About Option")]
    public GameObject AboutOption;

    [Header("Settings Option")]
    public GameObject SettingsOption;

    [Header("The About Screen")]
    public GameObject AboutScreen;

    [Header("The Settings Screen")]
    public GameObject SettingsScreen;

    public enum APP_USER_STATE
    {
        USER_STATE_NONE,
        USER_STATE_DELETE_SHARES,
        USER_STATE_MODIFY_SHARES
    }

    public static APP_USER_STATE LocalUserState;


    // We will use the variable below to save which was the last opened screen.
    // This way we can bring it back when the Extras Menu is closed.
    enum ScreenType
    {
        SCREEN_TYPE_INVALID = -1,
        SCREEN_TYPE_SHARING_SCREEN = 0,
        SCREEN_TYPE_ENTRIES_SCREEN,
        SCREEN_TYPE_ABOUT_SCREEN,
        SCREEN_TYPE_SETTINGS_SCREEN
    }

    [SerializeField] ScreenType LastOpenedScreen = ScreenType.SCREEN_TYPE_INVALID;


    // Start is called before the first frame update
    void Start()
    {
        LocalUserState = APP_USER_STATE.USER_STATE_NONE;
        _WorldData = GameObject.FindGameObjectWithTag("WorldScript").GetComponent<JSONReaderEntries>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetSharingPoolsOptions(bool enable)
    {
        RemoveSharingPoolOption.SetActive(enable);
        ModifySharingPoolOption.SetActive(enable);
        GoBackToExtraMenu.SetActive(enable);
        SharingPoolsOptions.SetActive(!enable);
        AboutOption.SetActive(!enable);
        SettingsOption.SetActive(!enable);
    }

    public void OnSharingPoolButtonPress()
    {
        SetSharingPoolsOptions(true);
    }

    public void OnGoBackExtraButtonPress()
    {
        SetSharingPoolsOptions(false);
    }

    public void OnAboutButtonPress()
    {
        this.gameObject.SetActive(false);
        AboutScreen.SetActive(true);
        LastOpenedScreen = ScreenType.SCREEN_TYPE_INVALID;
    }

    public void OnExtrasDeleteButtonPress()
    {
        LocalUserState = APP_USER_STATE.USER_STATE_DELETE_SHARES;
        _WorldData.OnExitButtonPress();
        this.gameObject.SetActive(false);

    }

    public void OnExtrasModifyButtonPress()
    {
        LocalUserState = APP_USER_STATE.USER_STATE_MODIFY_SHARES;
        _WorldData.OnExitButtonPress();
        this.gameObject.SetActive(false);

    }

    public void OnMainSharingsButtonPress()
    {
        MoreScreenMain.LocalUserState = MoreScreenMain.APP_USER_STATE.USER_STATE_NONE;
        LastOpenedScreen = ScreenType.SCREEN_TYPE_INVALID;
        _WorldData.OnExitButtonPress();
        this.gameObject.SetActive(false);
        EntriesScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        AboutScreen.SetActive(false);

    }

    public void OnSettingsButtonPress()
    {
        this.gameObject.SetActive(false);
        SettingsScreen.SetActive(true);
        LastOpenedScreen = ScreenType.SCREEN_TYPE_INVALID;
    }

    void DetectOpenedScreen()
    {
        if (SharingScreen.activeSelf)
        {
            LastOpenedScreen = ScreenType.SCREEN_TYPE_SHARING_SCREEN;
            SharingScreen.SetActive(false);
        }
        else if (EntriesScreen.activeSelf)
        {
            LastOpenedScreen = ScreenType.SCREEN_TYPE_ENTRIES_SCREEN;
            EntriesScreen.SetActive(false);

        }
        else if (SettingsScreen.activeSelf)
        {
            LastOpenedScreen = ScreenType.SCREEN_TYPE_SETTINGS_SCREEN;
            SettingsScreen.SetActive(false);

        }
        else if (AboutScreen.activeSelf)
        {
            LastOpenedScreen = ScreenType.SCREEN_TYPE_ABOUT_SCREEN;
            AboutScreen.SetActive(false);
        }
    }

    public void OnExtrasButtonPress()
    {
        DetectOpenedScreen();

        this.gameObject.SetActive(!this.gameObject.activeSelf);

        if(!this.gameObject.activeSelf)
        {
            if (LastOpenedScreen == ScreenType.SCREEN_TYPE_SHARING_SCREEN)
            {
                SharingScreen.SetActive(true);
            }
            else if (LastOpenedScreen == ScreenType.SCREEN_TYPE_ENTRIES_SCREEN)
            {
                EntriesScreen.SetActive(true);
            }
            else if (LastOpenedScreen == ScreenType.SCREEN_TYPE_SETTINGS_SCREEN)
            {
                SettingsScreen.SetActive(true);
            }
            else if (LastOpenedScreen == ScreenType.SCREEN_TYPE_ABOUT_SCREEN)
            {
                AboutScreen.SetActive(true);

            }
        }
    }

}
