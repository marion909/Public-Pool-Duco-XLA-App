using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class WorkerData
{
    public string sessionId;
    public string name;
    public string bestDifficulty;
    public string hashRate;
    public string startTime;
    public string lastSeen;
}

[System.Serializable]
public class JsonData
{
    public string bestDifficulty;
    public int workersCount;
    public List<WorkerData> workers;
}

public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}

public class JsonLoader : MonoBehaviour
{
    private const string ApiUrl = "https://public-pool.io:40557/api/client/";

    private const string DefaultPoolName = "Public Pool";

    private int ReloadTime;

    // UI elements
    public Text poolNameText, maxWorkersText, maxDifficultyText, maxHashRateText, lastLoadTimeText, walletAdress, workerListText;

    // Notification manager
    public NotificationManager notificationManager;

    // Settings menu script
    public SettingsMenu settingsMenuScript;

    // Variables to store wallet and last load time
    private string wallet;
    private DateTime lastLoadTime;

    // Variable to store max hash rate
    private float maxHashrate;

    private void Start()
    {
            if(PlayerPrefs.GetInt("Init") == 1){

            ReloadTime = 10;
        


        if(PlayerPrefs.GetInt("Init") == 0 && PlayerPrefs.GetInt("BTCActive") == 1){
               Debug.Log("Error on Init");

        }else{

        InitUI();
        HandleWallet();
        UpdateUIText();
        }
        }




    }

    private void InitUI()
    {
        // Initialize UI elements and button listeners
        poolNameText.text = PlayerPrefs.GetString("PoolName", DefaultPoolName);
    }

    private void HandleWallet()
    {
        // Check if wallet address is stored in PlayerPrefs
        if (PlayerPrefs.HasKey("BTCAddress"))
        {
            LoadWalletData();
            PeriodicJsonLoad();

            lastLoadTime = DateTime.Now;
        }

    }

    private void LoadWalletData()
    {
        // Load wallet data from PlayerPrefs
        wallet = PlayerPrefs.GetString("BTCAddress");
        walletAdress.text = wallet.Substring(Math.Max(0, wallet.Length - 20));;
    }

    private void PeriodicJsonLoad()
    {
        // Start a coroutine for periodic JSON loading
        StartCoroutine(PeriodicJsonLoadCoroutine());
    }

    private IEnumerator PeriodicJsonLoadCoroutine()
    {
        while (true)
        {
            // Load JSON data from the API
            LoadJsonFromURL(ApiUrl + wallet);
            yield return new WaitForSeconds(ReloadTime * 1.0f);
        }
    }

    private void LoadJsonFromURL(string url)
    {
        // Load JSON data from the specified URL
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new BypassCertificateHandler();
        StartCoroutine(HandleWebRequest(www));
    }

    private IEnumerator HandleWebRequest(UnityWebRequest www)
    {
        // Send the web request
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            // Log and handle errors
            HandleWebRequestError(www);
            yield break;
        }

        // Parse and log JSON data
        string jsonString = www.downloadHandler.text;

        try
        {
            JsonData jsonData = JsonUtility.FromJson<JsonData>(jsonString);
            LogJsonData(jsonData);
            lastLoadTime = DateTime.Now;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON data: {e.Message}");
        }
    }

    private void HandleWebRequestError(UnityWebRequest www)
    {
        // Handle and log web request errors
        Debug.LogError($"Error loading JSON data: {www.error}");
        // You can add additional error handling logic or user feedback here
    }

    private void LogJsonData(JsonData jsonData)
    {
        // Log and process JSON data
        //Debug.LogFormat("Best Difficulty: {0}", jsonData.bestDifficulty);
        //Debug.LogFormat("Number of Workers: {0}", jsonData.workersCount);
        PlayerPrefs.SetString("MaxWorkers", jsonData.workersCount.ToString());

        // Check worker availability and show notifications if necessary
        CheckWorkerAvailability(jsonData.workers);
        // Update the worker list UI
        UpdateWorkerList(jsonData.workers);

        foreach (WorkerData worker in jsonData.workers)
        {
            //Debug.LogFormat("Worker Name: {0}", worker.name);
            //Debug.LogFormat("Worker Best Difficulty: {0}", worker.bestDifficulty);
            //Debug.LogFormat("Worker Hashrate: {0}", worker.hashRate);
        }

        // Update PlayerPrefs and UI text based on JSON data
        UpdateJsonDataText(jsonData);
    }

   private void UpdateWorkerList(List<WorkerData> workers)
    {
        // Update the worker list UI
        workerListText.text = "Wallet Balance: " + PlayerPrefs.GetString("BTCBalance") + "\n";
        int ID = 0;
        maxHashrate = 0;

        foreach (WorkerData worker in workers)
        {
            ID++;

            // Find the position of the decimal point
            int decimalPointIndex = worker.hashRate.IndexOf('.');

            // Remove everything after the decimal point
            string cleanedHashRate = decimalPointIndex != -1 ? worker.hashRate.Substring(0, decimalPointIndex) : worker.hashRate;

            // Update maxHashrate
            if (float.TryParse(cleanedHashRate, out float hashRateValue))
            {
                maxHashrate += hashRateValue;

                // Format the hash rate using the FormatHashRate function
                string formattedHashRate = FormatHashRate(hashRateValue);

                // Update worker list text
                string workerInfo = $"ID: {ID}\nName: {worker.name}\nHashrate: {formattedHashRate}\nDifficulty: {worker.bestDifficulty}\n\n";
                workerListText.text += workerInfo;
               // Debug.Log(workerInfo);
            }
            else
            {
                Debug.LogError($"Fehler beim Konvertieren von Hashrate für Worker ID {ID}");
            }
        }
    }

 private string FormatHashRate(float hashRate)
    {
        string[] units = { "H/s", "KH/s", "MH/s", "GH/s", "TH/s" };
        int unitIndex = 0;

        while (hashRate >= 1000 && unitIndex < units.Length - 1)
        {
            hashRate /= 1000;
            unitIndex++;
        }

        return $"{hashRate:N3} {units[unitIndex]}";
    }
   

    private void CheckWorkerAvailability(List<WorkerData> workers)
    {
        // Check worker availability and show notifications if necessary
        foreach (WorkerData worker in workers)
        {
            DateTime lastSeenTime;
            if (DateTime.TryParse(worker.lastSeen, out lastSeenTime))
            {
                TimeSpan timeSinceLastSeen = DateTime.Now - lastSeenTime;
                if (timeSinceLastSeen.TotalMinutes > 15)
                {
                    ShowNotification($"Worker {worker.name} is no longer reachable!");
                }
            }
        }
    }

    private void UpdateJsonDataText(JsonData jsonData)
    {
        // Update PlayerPrefs and UI text based on JSON data
        PlayerPrefs.SetString("MaxDifficulty", jsonData.bestDifficulty.Substring(0, Math.Min(7, jsonData.bestDifficulty.Length)));
        // Format and update maxHashRateText
        string maxHashRateText = FormatHashRate(maxHashrate);
        PlayerPrefs.SetString("MaxHashRate", maxHashRateText);

        // Update UI text
        UpdateUIText();
    }

    private void UpdateUIText()
    {
        // Update UI text based on PlayerPrefs values
        maxWorkersText.text = $"Workers: {PlayerPrefs.GetString("MaxWorkers", "N/A")}";
        maxHashRateText.text = $"HashRate: {PlayerPrefs.GetString("MaxHashRate", "N/A")}";
        maxDifficultyText.text = $"Max Diff: {PlayerPrefs.GetString("MaxDifficulty", "N/A")}";
        lastLoadTimeText.text = $"Last Update: {lastLoadTime:yyyy-MM-dd HH:mm:ss}";

        // Show notification if enabled
        if (PlayerPrefs.GetInt("NotificationEnabled") == 1)
        {
            notificationManager.ShowNotification($"{maxWorkersText.text}\n{maxHashRateText.text}\n{maxDifficultyText.text}", DateTime.Now.AddSeconds(1));
        }
    }

    private void RefreshData()
    {
        // Refresh JSON data
        LoadJsonFromURL(ApiUrl + wallet);
    }

    private void ShowNotification(string message)
    {
        // Show a notification with the specified message
        notificationManager.ShowNotification(message, DateTime.Now.AddSeconds(5));
       // Debug.Log($"Notification: {message}");
    }

    private void ShowTestNotification()
    {
        // Show a test notification
        ShowNotification("This is a test notification!");
    }
}
