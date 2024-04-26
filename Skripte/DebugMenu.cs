using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{

    public GameObject debugMenuButton;
    public GameObject debugMenu;

    public Button debugButton, SaveButton, CancelButton, XLAButton, DeactivateDebug;

    private int DebugMenuInt = 0;

    public Slider Slider;
    public Text SliderText;



    // Start is called before the first frame update
    void Start()
    {
       SearchForFile("debug.menu");
       ActivateDebugMenu(); 
       Debug.Log("Persistent Data Path: " + Application.persistentDataPath);

       debugButton.onClick.AddListener(delegate {OpenDebugMenu();});
       CancelButton.onClick.AddListener(delegate {CloseDebugMenu();});
       SaveButton.onClick.AddListener(delegate {Save();});

        Slider.value = PlayerPrefs.GetInt("ReloadTime");

        XLAButton.onClick.AddListener(delegate{DebugMenuInt += 1;});

        DeactivateDebug.onClick.AddListener(delegate { DeactivateDebugMenu(); });
        
    }

    // Update is called once per frame
    void Update()
    {
        SliderText.text = Slider.value.ToString() + " Sekunden";

        if(DebugMenuInt == 20){

            PlayerPrefs.SetInt("DebugMenuActive", 1);
            ReloadScene();
            
        }
    }

    void DeactivateDebugMenu(){

        PlayerPrefs.SetInt("DebugMenuActive", 0);
        ReloadScene();
    }

    void SearchForFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            Debug.Log("Datei gefunden: " + filePath);
            debugMenuButton.SetActive(true);
        }
        else
        {
            Debug.Log("Datei nicht gefunden: " + filePath);
            
        }
    }

    void ActivateDebugMenu(){

        if (PlayerPrefs.GetInt("DebugMenuActive") == 1)
        {

            debugMenuButton.SetActive(true);
        }

    }

    void OpenDebugMenu(){

        debugMenu.SetActive(true);
    }

    void CloseDebugMenu(){

        debugMenu.SetActive(false);
    }

        void Save(){

        PlayerPrefs.SetInt("ReloadTime", (int)Slider.value);
        debugMenu.SetActive(false);
    }

        void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
