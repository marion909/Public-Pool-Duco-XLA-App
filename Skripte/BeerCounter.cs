using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeerCounter : MonoBehaviour
{
    public Text beerText;
    public int beerCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.HasKey("beerCount")){

            beerCount = PlayerPrefs.GetInt("beerCount");  
        }else{

            beerCount = 0;
        }

        beerText.text = "Beer Count: " + beerCount.ToString();
      
    }

    public void BeerPuchased(){

        int actualbeerCount = PlayerPrefs.GetInt("beerCount");
        actualbeerCount += 1;
        PlayerPrefs.SetInt("beerCount", actualbeerCount);
        PlayerPrefs.Save();
    }
}
