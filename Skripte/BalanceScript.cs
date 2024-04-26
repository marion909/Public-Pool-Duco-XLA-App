using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BalanceScript : MonoBehaviour
{
    public string btcAddress;
    public string duinoCoinAddress;

    private void Start()
    {
        if(PlayerPrefs.GetInt("Init") == 1){
        
        btcAddress = PlayerPrefs.GetString("BTCAddress");
        duinoCoinAddress = PlayerPrefs.GetString("DucoAddress");

        StartCoroutine(FetchBalances());
        }

    }

    private IEnumerator FetchBalances()
    {
        yield return FetchBalance("BTC", btcAddress, "BTCBalance", "https://blockchain.info/q/addressbalance/");
        yield return FetchBalance("Duinocoin", duinoCoinAddress, "DuinoCoinBalance", "https://server.duinocoin.com/balances/");

        // Hier kannst du weitere Logik hinzufügen, abhängig von den abgerufenen Balances
    }

    private IEnumerator FetchBalance(string poolName, string address, string playerPrefKey, string apiUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{apiUrl}{Uri.EscapeUriString(address)}");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string balance = request.downloadHandler.text;
            if(playerPrefKey == "DuinoCoinBalance"){

                balance = ParseDuinoCoinBalance(balance);
                PlayerPrefs.SetString(playerPrefKey, balance);
            }else{
                    PlayerPrefs.SetString(playerPrefKey, balance);

            }
            
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError($"Error fetching {poolName} balance: {request.error}");
        }
    }

    private string ParseDuinoCoinBalance(string response)
    {
        try
        {
            // Parse JSON
            var json = JsonUtility.FromJson<DuinoCoinResponse>(response);

            // Extrahiere den Kontostand vor dem Punkt
            string[] balanceParts = json.result.balance.ToString().Split('.');
            string wholeBalance = balanceParts.Length > 0 ? balanceParts[0] : "0";

            return wholeBalance;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing DuinoCoin response: " + e.Message);
            return "0";
        }
    }


    [System.Serializable]
    private class DuinoCoinResponse
    {
        public Result result;
        public string server;
        public bool success;

        [System.Serializable]
        public class Result
        {
            public float balance;
            public string created;
            public long last_login;
            public float stake_amount;
            public long stake_date;
            public int trust_score;
            public string username;
            public string verified;
            public string verified_by;
            public long verified_date;
            public int warnings;
        }
    }
}