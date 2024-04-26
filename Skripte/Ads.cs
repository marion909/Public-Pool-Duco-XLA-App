using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
private string _adUnitId = "ca-app-pub-1252758290436254/6292046147";


public Button LoadBanner, DestroyBanner;

BannerView _bannerView;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

    _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);

    LoadBanner.onClick.AddListener(LoadAd);
    DestroyBanner.onClick.AddListener(DestroyBannerView);
    
    LoadAd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void CreateBannerView()
  {
      Debug.Log("Creating banner view");

      // If we already have a banner, destroy the old one.
      if (_bannerView != null)
      {
          DestroyBannerView();
      }

      // Create a 320x50 banner at top of the screen
      _bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
  }

  public void LoadAd()
{
    // create an instance of a banner view first.
    if(_bannerView == null)
    {
        CreateBannerView();
    }

    // create our request used to load the ad.
    var adRequest = new AdRequest();

    // send the request to load the ad.
    Debug.Log("Loading banner ad.");
    _bannerView.LoadAd(adRequest);
}

public void DestroyBannerView()
{
    if (_bannerView != null)
    {
        Debug.Log("Destroying banner view.");
        _bannerView.Destroy();
        _bannerView = null;
    }
}
}
