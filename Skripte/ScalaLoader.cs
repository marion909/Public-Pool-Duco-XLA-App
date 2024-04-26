using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System;

public class MiningStats : MonoBehaviour
{
    private string address;

    public string ServerURL = "https://pool.scalaproject.io/api/";
    private Coroutine fetchAddressCoroutine;

    // Unity UI Text fields for displaying mining statistics
    public Text addressText;
    public Text balanceText;
    public Text totalHashrateText;
    public Text minersInfoText;

    public Text PoolNameText;

    public Text totalpaid1;

    public string Adresse1 = "SvmcGVfx4QU5JeYZSfCa672NChUCfy2TXDtFbjbcayKDjpX7Ey4uCkd8Drfgtqzb5AXHqxEYu38Rb7STZdjEjncU1UMb3wDHX";

    public string PoolNameScala = "ScalaProject.io";

    public Text lastUpdateText; // Neues Textfeld für das letzte Aktualisierungsdatum

    private DateTime lastUpdateDateTime;

    private int ReloadTime;

    private void Start()
    {

            ReloadTime = 10;
        

        if(PlayerPrefs.GetInt("Init") == 1){

            address = PlayerPrefs.GetString("XLAAddress");

                    // Initialize variables and start fetching data
        if (!string.IsNullOrEmpty(address))
        {
            fetchAddressCoroutine = StartCoroutine(FetchAddressStatsCoroutine(false));
        }
        }
        PoolNameText.text = PoolNameScala;
    }

    private IEnumerator FetchAddressStatsCoroutine(bool longpoll)
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(ServerURL + "/stats_address?address=" + address + "&longpoll=" + longpoll);
            //Debug.Log(ServerURL + "/stats_address?address=" + address + "&longpoll=" + longpoll);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                // Parse and update UI based on the response
                UpdateUI(responseText);
            }
            else
            {
                Debug.LogError("Error fetching data: " + request.error);
            }

            // Set the interval for fetching data
            yield return new WaitForSeconds(ReloadTime * 1.0f);
        }
    }

    private void UpdateUI(string responseData)
    {
// Parse the response data
    MiningStatsData miningStatsData = JsonUtility.FromJson<MiningStatsData>(responseData);

    // Update UI Text fields
    addressText.text = address.Substring(Math.Max(0, Adresse1.Length - 20));
// Extrahiere den Kontostandwert und teile ihn durch 100
float balanceValue = float.Parse(miningStatsData.stats.balance) / 100.0f;

// Formatieren und Anzeigen des Kontostands
string formattedBalance = string.Format("{0:F2}", balanceValue);
balanceText.text = "Balance: " + formattedBalance + " XLA";


    totalHashrateText.text = "Total Hashrate: " + ConvertHashrate(miningStatsData.stats.hashrate);

    // Format "paid" with two decimal places
    float paidValue = float.Parse(miningStatsData.stats.paid) / 100.0f;
    string formattedPaid = string.Format("{0:F2}", paidValue);
    totalpaid1.text = "Paid: " + formattedPaid + " XLA";

        // Update miners info
        StringBuilder minersInfoBuilder = new StringBuilder();

        int id = 0;
        foreach (var worker in miningStatsData.workers)
        {
            id++;
            if(worker.hashrate > 0){

            minersInfoBuilder.AppendLine($"ID: {id}\nName: {worker.name}\nHashrate: {ConvertHashrate(worker.hashrate)}\nHashes: {worker.hashes}");
            minersInfoBuilder.AppendLine();
            }

        }
        minersInfoText.text = minersInfoBuilder.ToString();

        // Aktualisiere das letzte Aktualisierungsdatum
        lastUpdateDateTime = DateTime.Now;
        lastUpdateText.text = "Last Update: " + lastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

   private string ConvertHashrate(int hashrate)
{
    if (hashrate >= 1000000000)
    {
        return $"{hashrate / 1000000000} GH/s";
    }
    else if (hashrate >= 1000000)
    {
        return $"{hashrate / 1000000} MH/s";
    }
    else if (hashrate >= 1000)
    {
        return $"{hashrate / 1000} KH/s";
    }
    else
    {
        return $"{hashrate} H/s";
    }
}

    // Funktion zum Aktualisieren der Adresse
    public void UpdateAddress(string updatedAddress)
    {
        Adresse1 = updatedAddress;
        // Optional: Hier kannst du weitere Logik hinzufügen, um die UI oder andere Funktionen entsprechend zu aktualisieren.
    }

    // Funktion zum Stoppen des Datenabrufs
    public void StopFetchingData()
    {
        if (fetchAddressCoroutine != null)
        {
            StopCoroutine(fetchAddressCoroutine);
        }
    }

    // Funktion zum Starten des Datenabrufs
    public void StartFetchingData()
    {
        fetchAddressCoroutine = StartCoroutine(FetchAddressStatsCoroutine(false));
    }

    [System.Serializable]
    public class MiningStatsData
    {
        public StatsData stats;
        public string[] payments;
        public ChartsData charts;
        public WorkerData[] workers;

        [System.Serializable]
        public class StatsData
        {
            public string address;
            public string balance;
            public int hashrate;

            public string paid;
        }

        [System.Serializable]
        public class ChartsData
        {
            public PaymentData[] payments;
            public int[] hashrate;
        }

        [System.Serializable]
        public class PaymentData
        {
            public long timestamp;
            public int value;
        }

        [System.Serializable]
        public class WorkerData
        {
            public string name;
            public int hashrate;
            public long lastShare;
            public int hashes;
            public int error_count;
            public int block_count;
            public int donations;
            public string pool_type;
        }
    }

    private void OnDestroy()
    {
        // Stop coroutine and clean up resources
        StopFetchingData();
    }
}
