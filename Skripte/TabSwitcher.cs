using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    public Button duco, bitcoin, xla;
    public GameObject ducoTab, bitcoinTab, xlaTab;
    public Toggle BTCToggle, DucoToggle, XLAToggle;
    public SettingsMenu settingsMenu; // Referenz auf das SettingsMenu-Skript

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("Init") == 1){
                    duco.onClick.AddListener(delegate { SwitchTab(ducoTab, duco); });
        bitcoin.onClick.AddListener(delegate { SwitchTab(bitcoinTab, bitcoin); });
        xla.onClick.AddListener(delegate { SwitchTab(xlaTab, xla); });

        // Load toggle states initially
        LoadToggleStates();
        }

    }

    void LoadToggleStates()
    {
        BTCToggle.isOn = PlayerPrefs.GetInt("BTCActive", 0) == 1;
        DucoToggle.isOn = PlayerPrefs.GetInt("DucoActive", 0) == 1;
        XLAToggle.isOn = PlayerPrefs.GetInt("XLAActive", 0) == 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SwitchTab(GameObject tab, Button button)
    {
        string toggleKey = "";

        if (button == duco)
            toggleKey = "DucoActive";
        else if (button == bitcoin)
            toggleKey = "BTCActive";
        else if (button == xla)
            toggleKey = "XLAActive";

        bool toggleActive = PlayerPrefs.GetInt(toggleKey, 0) == 1;

        if (toggleActive)
        {
            ducoTab.SetActive(false);
            bitcoinTab.SetActive(false);
            xlaTab.SetActive(false);
            tab.SetActive(true);
            button.interactable = true; // Aktiviere den Button
        }
        else
        {
            // Zerstöre den Button
            Destroy(button.gameObject);
            tab.SetActive(false);
        }
    }
}
