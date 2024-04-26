using UnityEngine;
using UnityEngine.UI;

public class TabBar : MonoBehaviour
{
    public Button BTCButton, DucoButton, XLAButton;

    public GameObject BTC, Duco, XLA;

    void Update()
    {
        UpdateButtons();
    }

    void UpdateButtons()
    {
        DestroyButtonIfToggleOff("BTCActive", BTCButton);
        DestroyButtonIfToggleOff("DucoActive", DucoButton);
        DestroyButtonIfToggleOff("XLAActive", XLAButton);

        DestroyGameobjectIfToggleOff("BTCActive", BTC);
        DestroyGameobjectIfToggleOff("DucoActive", Duco);
        DestroyGameobjectIfToggleOff("XLAActive", XLA);


    }

    void DestroyButtonIfToggleOff(string toggleKey, Button button)
    {
        bool toggleActive = PlayerPrefs.GetInt(toggleKey, 0) == 1;

        if (!toggleActive && button != null)
        {
            Destroy(button.gameObject);
        }
    }

    void DestroyGameobjectIfToggleOff(string toggleKey, GameObject gameObject)
    {
        bool toggleActive = PlayerPrefs.GetInt(toggleKey, 0) == 1;

        if (!toggleActive && gameObject != null)
        {
           gameObject.SetActive(false);
        }
    }
}
