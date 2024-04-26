using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

public class DucoLoader : MonoBehaviour
{
    public Text userInfoText;
    public Text minersInfoText;
    public Text highestDifficultyText;
    public Text maxHashrateText;
    public Text individualMinerText;
    public Text lastReloadTimeText; // New public Text field for last reload time
    public Text poolNameText; // New public Text field for pool name

    private string apiUrl = "https://server.duinocoin.com";
    private string username;
    private string poolName;

    private List<DucoMiner> minersList = new List<DucoMiner>();
    private double totalHashrate = 0.0;
    private int totalMiners = 0;
    private double highestDifficulty = 0.0;
    private double maxHashrate = 0.0;
    private DateTime lastReloadTime;

     private int ReloadTime;

    void Start()
    {



            ReloadTime = 10;
    

        if(PlayerPrefs.GetInt("Init") == 1){
                 username = PlayerPrefs.GetString("DucoAddress");

                 if(PlayerPrefs.GetInt("DucoActive") == 1){
            StartCoroutine(DisplayData());
                 }
        }
        
        // Set pool name (replace "YourPoolName" with the actual pool name)
        poolName = "Duinocoin";

        
    }

    IEnumerator DisplayData()
    {  
        if(PlayerPrefs.GetInt("DucoActive") == 1){
        while (true) // Run indefinitely
        {
                totalHashrate = 0.0;
                totalMiners = 0;
            // Retrieve user data
            string userDataUrl = $"{apiUrl}/users/{username}";
            
            UnityWebRequest userDataRequest = UnityWebRequest.Get(userDataUrl);

            yield return userDataRequest.SendWebRequest();

            if (userDataRequest.result == UnityWebRequest.Result.Success)
            {
                string userDataJson = userDataRequest.downloadHandler.text;
                DisplayUserInfo(userDataJson);
            }
            else
            {
                Debug.LogError($"Error fetching user data: {userDataRequest.error}");
            }

            // Retrieve miner data
            string minersDataUrl = $"{apiUrl}/miners/{username}";
            UnityWebRequest minersDataRequest = UnityWebRequest.Get(minersDataUrl);

            yield return minersDataRequest.SendWebRequest();

            if (minersDataRequest.result == UnityWebRequest.Result.Success)
            {
                string minersDataJson = minersDataRequest.downloadHandler.text;
                DisplayMinerInfo(minersDataJson);
            }
            else
            {
                Debug.LogError($"Error fetching miner data: {minersDataRequest.error}");
            }

            // Display total hashrate
            double totalHashrateFormatted = ConvertHashrate(totalHashrate);
            userInfoText.text = $"{username}";

            // Display miners info
            minersInfoText.text = $"Total Miners: {totalMiners}";

            // Display highest difficulty
            highestDifficultyText.text = $"Highest Difficulty: {highestDifficulty}";

            // Display max hashrate
            maxHashrateText.text = $"Total Hashrate: {totalHashrateFormatted} KH/s";

            // Display individual miner info
            DisplayIndividualMinerInfo();

            // Update last reload time
            lastReloadTime = DateTime.Now;
            lastReloadTimeText.text = $"Last Update: {lastReloadTime.ToString("yyyy-MM-dd HH:mm:ss")}";

            // Update pool name
            poolNameText.text = $"{poolName}";

            yield return new WaitForSeconds(ReloadTime * 1.0f); // Wait for 10 seconds before the next reload
        }
        }
    }

    void DisplayUserInfo(string userDataJson)
    {
        DucoUserInfo userInfo = JsonUtility.FromJson<DucoUserInfo>(userDataJson);

        if (userInfo != null && userInfo.success)
        {
            // No need to display user info here, it's displayed in the userInfoText later
        }
        else
        {
            Debug.LogError("Error parsing user information from JSON.");
        }
    }

    void DisplayMinerInfo(string minersDataJson)
    {
        DucoMinerInfo minerInfo = JsonUtility.FromJson<DucoMinerInfo>(minersDataJson);

        if (minerInfo != null && minerInfo.success && minerInfo.result.Length > 0)
        {
            minersList.Clear(); // Clear the list before populating it again

            foreach (DucoMiner miner in minerInfo.result)
            {
                totalMiners++;
                double hashrateInKH = ConvertHashrate(miner.hashrate);

                // Update highest difficulty
                if (miner.diff > highestDifficulty)
                {
                    highestDifficulty = miner.diff;
                }

                // Update max hashrate
                if (miner.hashrate > maxHashrate)
                {
                    maxHashrate = miner.hashrate;
                }

                minersList.Add(miner);
                totalHashrate += miner.hashrate;
            }
        }
        else
        {
            Debug.LogError("Error parsing miner information from JSON.");
        }
    }

    double ConvertHashrate(double hashrate)
    {
        string[] suffixes = { "H/s", "KH/s", "MH/s", "GH/s", "TH/s" };
        int suffixIndex = 0;

        while (hashrate >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            hashrate /= 1000.0;
            suffixIndex++;
        }

        return hashrate;
    }

    void DisplayIndividualMinerInfo()
    {
        individualMinerText.text = "Wallet Balance: " + PlayerPrefs.GetString("DuinoCoinBalance") + "\n";
        int id = 0;
        foreach (DucoMiner miner in minersList)
        {
            id++;
            double hashrateInKH = ConvertHashrate(miner.hashrate);
            individualMinerText.text += $"ID: {id}\nName: {miner.identifier}\nHashrate: {hashrateInKH} KH/s\nDifficulty: {miner.diff}\n\n";
        }
    }

    // Define data structures for JSON parsing
    [System.Serializable]
    public class DucoUserInfo
    {
        public DucoUserResult result;
        public bool success;
    }

    [System.Serializable]
    public class DucoUserResult
    {
        public DucoUserBalance balance;
    }

    [System.Serializable]
    public class DucoUserBalance
    {
        public double balance;
        public string username;
    }

    [System.Serializable]
    public class DucoMinerInfo
    {
        public DucoMiner[] result;
        public bool success;
    }

    [System.Serializable]
    public class DucoMiner
    {
        public int accepted;
        public string algorithm;
        public int diff;
        public double hashrate;
        public string identifier;
        public int rejected;
        public double sharetime;
        public string software;
        public string threadid;
        public string username;
    }
}
