using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class InitSkript : MonoBehaviour
{

    public string BTCAdress, DucoAdress, XLAAdress;
    public GameObject InitScreen, BTCButton, DucoButton, XLAButton;

    public bool BTCActive, DucoActive, XLAActive;

    public InputField BTCInputField, DucoInputField, XLAInputField;

    public Toggle BTCToggle, DucoToggle, XLAToggle;

    public Button saveButton;

    public Toggle DemoMode;

    // Start is called before the first frame update
    void Start()
    {


        if (PlayerPrefs.GetInt("Init") == 0 | PlayerPrefs.HasKey("Init") == false)
        {
            InitScreen.SetActive(true);
            BTCToggle.onValueChanged.AddListener(delegate { SetActive(BTCToggle, BTCButton, BTCInputField, "BTCAddress"); });
            DucoToggle.onValueChanged.AddListener(delegate { SetActive(DucoToggle, DucoButton, DucoInputField, "DucoAddress"); });
            XLAToggle.onValueChanged.AddListener(delegate { SetActive(XLAToggle, XLAButton, XLAInputField, "XLAAddress"); });


            // Füge den SaveAndCloseInitScreen-Code zum Klicken des Buttons hinzu
            saveButton.onClick.AddListener(SaveAndCloseInitScreen);
        }
    }

    // Update is called once per frame
    void Update()
    {
            if(DemoMode.isOn){

                    Demo();

            }
    }

    void Demo(){

        PlayerPrefs.SetString("BTCAddress", "bc1qa6tf2j4kp6yxmqvpaguceqlwm27ny6f95zfvr8");
        PlayerPrefs.SetString("DucoAddress", "marion808");
        PlayerPrefs.SetString("XLAAddress", "SvmcGVfx4QU5JeYZSfCa672NChUCfy2TXDtFbjbcayKDjpX7Ey4uCkd8Drfgtqzb5AXHqxEYu38Rb7STZdjEjncU1UMb3wDHX");

                // Speichere die Toggles in den PlayerPrefs
        PlayerPrefs.SetInt("BTCActive", 1);
        PlayerPrefs.SetInt("DucoActive", 1);
        PlayerPrefs.SetInt("XLAActive", 1);

        Debug.Log("Demo mode active!");

        PlayerPrefs.SetInt("Init", 1);

        InitScreen.SetActive(false);

        ReloadScene();
  
    }

    void SetActive(Toggle toggle, GameObject button, InputField inputField, string addressPlayerPrefKey)
    {
        bool isActive = toggle.isOn;

        // Aktiviere/deaktiviere das GameObject
        button.SetActive(isActive);

        // Mache das InputField interaktiv
        inputField.interactable = isActive;

        // Speichere die Adresse in den PlayerPrefs, wenn das Toggle aktiviert ist
        if (isActive)
        {
            PlayerPrefs.SetString(addressPlayerPrefKey, inputField.text);
        }

    }

    // Öffentliche Methode für den Button
    public void SaveAndCloseInitScreen()
    {
        // Speichere Init auf 1, um beim nächsten Start nicht mehr den Init-Screen zu zeigen
        PlayerPrefs.SetInt("Init", 1);

        // Speichere die Toggles in den PlayerPrefs
        PlayerPrefs.SetInt("BTCActive", BTCToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("DucoActive", DucoToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("XLAActive", XLAToggle.isOn ? 1 : 0);

        // Speichere die Daten in PlayerPrefs, falls die Toggles aktiviert sind
        if (BTCToggle.isOn)
        {
            PlayerPrefs.SetString("BTCAddress", BTCInputField.text);
        }
        if (DucoToggle.isOn)
        {
            PlayerPrefs.SetString("DucoAddress", DucoInputField.text);
        }
        if (XLAToggle.isOn)
        {
            PlayerPrefs.SetString("XLAAddress", XLAInputField.text);
        }

          // Neuladen der Szene
        ReloadScene();
        // Deaktiviere den Init-Screen
        InitScreen.SetActive(false);
    }

    
    // Methode zum Neuladen der Szene
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
