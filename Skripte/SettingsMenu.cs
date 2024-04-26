using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public InputField BTCInputField, DucoInputField, XLAInputField;
    public Button saveButton, cancelButton, settingsButton; 
    public Toggle BTCToggle, DucoToggle, XLAToggle;
    public GameObject SettingsPanel, BTC, Duco, XLA;

    public Dropdown ReloadTime;

    public Dropdown StandardTab;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("Init") == 1){
                   LoadToggleStates();
        // Lade die gespeicherten Adressen aus den PlayerPrefs und setze sie in die Input-Fields
        BTCInputField.text = PlayerPrefs.GetString("BTCAddress", "");
        DucoInputField.text = PlayerPrefs.GetString("DucoAddress", "");
        XLAInputField.text = PlayerPrefs.GetString("XLAAddress", "");

        // Füge den Save-Button-Code zum Klicken des Buttons hinzu
        saveButton.onClick.AddListener(SaveSettings);

        // Füge den Cancel-Button-Code zum Klicken des Buttons hinzu
        cancelButton.onClick.AddListener(ClosePanel);
        settingsButton.onClick.AddListener(OpenPanel);

        // Füge die Toggle-Listener hinzu
        BTCToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(BTCToggle, BTC); });
        DucoToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(DucoToggle, Duco); });
        XLAToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(XLAToggle, XLA); });

        // Füge den Dropdown-Listener hinzu
        StandardTab.onValueChanged.AddListener(delegate { DropdownValueChanged(StandardTab); });


              // Aktiviere standardmäßig das entsprechende Tab für jeden Toggle
        ActivateDefaultTab(); 
        }



    }

    void LoadToggleStates()
    {
        BTCToggle.isOn = PlayerPrefs.GetInt("BTCActive", 0) == 1;
        DucoToggle.isOn = PlayerPrefs.GetInt("DucoActive", 0) == 1;
        XLAToggle.isOn = PlayerPrefs.GetInt("XLAActive", 0) == 1;
    }

    // Methode für den Save-Button im Settings-Skript
    public void SaveSettings()
    {
        // Speichere die Adressen in den PlayerPrefs
        PlayerPrefs.SetString("BTCAddress", BTCInputField.text);
        PlayerPrefs.SetString("DucoAddress", DucoInputField.text);
        PlayerPrefs.SetString("XLAAddress", XLAInputField.text);

        // Speichere die Toggle-Werte in den PlayerPrefs
        PlayerPrefs.SetInt("BTCActive", BTCToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("DucoActive", DucoToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("XLAActive", XLAToggle.isOn ? 1 : 0);

        PlayerPrefs.SetInt("ReloadTime", ReloadTime.value);

        // Speichere die PlayerPrefs
        PlayerPrefs.Save();

        // Neuladen der Szene
        ReloadScene();

        SettingsPanel.SetActive(false);

    }

    // Methode für den Cancel-Button im Settings-Skript
    public void ClosePanel()
    {
        // Füge hier die Logik hinzu, um das Panel zu schließen
        SettingsPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        SettingsPanel.SetActive(true);
    }

    // Methode für den Toggle-Event
    void ToggleValueChanged(Toggle toggle, GameObject gameObjectToToggle)
    {
         // Deaktiviere das zugehörige GameObject, wenn der Toggle deaktiviert ist
        gameObjectToToggle.SetActive(toggle.isOn);
        // Hier kannst du Logik basierend auf dem Toggle-Event hinzufügen
        Debug.Log($"{toggle.gameObject.name} is toggled to {toggle.isOn}");
    }

    // Methode zum Neuladen der Szene
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

     // Aktiviere standardmäßig das entsprechende Tab für jeden Toggle
    void ActivateDefaultTab()
    {
    if (!BTCToggle.isOn && DucoToggle.isOn && XLAToggle.isOn)
    {
        BTC.SetActive(false);
        Duco.SetActive(true);
        XLA.SetActive(false);
    }
    else if (!DucoToggle.isOn && BTCToggle.isOn && XLAToggle.isOn)
    {
        BTC.SetActive(true);
        Duco.SetActive(false);
        XLA.SetActive(false);
    }
    else if (!XLAToggle.isOn && BTCToggle.isOn && DucoToggle.isOn)
    {
        BTC.SetActive(true);
        Duco.SetActive(true);
        XLA.SetActive(false);
    }
        else if (XLAToggle.isOn && !BTCToggle.isOn && !DucoToggle.isOn)
    {
        BTC.SetActive(false);
        Duco.SetActive(false);
        XLA.SetActive(true);
    }
            else if (!XLAToggle.isOn && !BTCToggle.isOn && DucoToggle.isOn)
    {
        BTC.SetActive(false);
        Duco.SetActive(true);
        XLA.SetActive(false);
    }
                else if (!XLAToggle.isOn && BTCToggle.isOn && !DucoToggle.isOn)
    {
        BTC.SetActive(true);
        Duco.SetActive(false);
        XLA.SetActive(false);
    }

    else
    {
        // If none of the above conditions are met, activate the default tab
        BTC.SetActive(true);
        Duco.SetActive(false);
        XLA.SetActive(false);
    }
            
    }

    void DropdownValueChanged(Dropdown dropdown)
    {
        // Aktiviere standardmäßig das entsprechende Tab basierend auf der Dropdown-Option
        switch (dropdown.value)
        {
            case 0: // BTC
                BTCToggle.isOn = true;
                break;
            case 1: // Duco
                DucoToggle.isOn = true;
                break;
            case 2: // XLA
                XLAToggle.isOn = true;
                break;
            default:
                break;
        }
    }
}
