using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;

public class IngameLogs : MonoBehaviour
{
    public Text logTextPrefab; // Prefab für einzelne Protokolleinträge
    public Transform content; // Das Inhaltsobjekt des ScrollView
    public uint qsize = 15; // Anzahl der zu behaltenden Nachrichten
    private Queue<string> myLogQueue = new Queue<string>(); // Warteschlange für Protokolle

    public InputField LogPassword;

    public GameObject LogPanel;

    void Start()
    {
        if(PlayerPrefs.GetInt("DebugMenuActive") == 1){
                        Debug.Log("Started up logging.");
        Application.logMessageReceived += HandleLog;
        }

    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Protokollzeile erstellen
        string log = "[" + type + "] : " + logString;
        if (type == LogType.Exception)
            log += "\n" + stackTrace;

        // Protokollzeile zur Warteschlange hinzufügen
        myLogQueue.Enqueue(log);

        // Sicherstellen, dass die Warteschlange nicht größer als qsize ist
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();

             UpdateScrollView();
    }

    void UpdateScrollView()
    {
        // Löschen Sie alle vorhandenen Protokolleinträge im ScrollView
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Fügen Sie neue Protokolleinträge zum ScrollView hinzu
        foreach (string log in myLogQueue)
        {
            Text newLogText = Instantiate(logTextPrefab, content);
            newLogText.text = log;
        }
    }

    public void SendLogs(){

        LogPanel.SetActive(true);
    }

    public void SendLogsWithPassword(){

            if(LogPassword.text == "SendLog0815!"){

                SendLogsByEmail();
                LogPassword.text = "Logs Sent!";

            }else{
                LogPassword.text = "Wrong Password!";
            }

    }

    void SendLogsByEmail()
    {
        // Erstellen Sie den Inhalt der E-Mail-Nachricht aus den aktuellen Protokollen
        string emailBody = string.Join("\n", myLogQueue.ToArray());

        // E-Mail-Versand konfigurieren
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("app@neuhauser.cloud"); // Ihre E-Mail-Adresse hier
        mail.To.Add("app@neuhauser.cloud"); // Empfänger-E-Mail-Adresse hier
        mail.Subject = "Debug Logs - " + SystemInfo.deviceUniqueIdentifier + " - " + SystemInfo.deviceModel + " - " + SystemInfo.operatingSystem + " - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // Betreff der E-Mail
        mail.Body = emailBody; // Inhalt der E-Mail

        // SMTP-Konfiguration
        SmtpClient smtpServer = new SmtpClient("smtp.world4you.com"); // SMTP-Server-Adresse hier
        smtpServer.Port = 587; // SMTP-Port (normalerweise 587 für TLS oder 465 für SSL)
        smtpServer.Credentials = new NetworkCredential("app@neuhauser.cloud", "lr0oFt6f+n"); // Ihre Anmeldeinformationen hier
        smtpServer.EnableSsl = true; // SSL aktivieren

        // Sicherheitszertifikat überprüfen
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

        // E-Mail senden
        try
        {
            smtpServer.Send(mail);
            Debug.Log("Logs sent successfully!");
             
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to send logs: " + e.ToString());
        }

        LogPanel.SetActive(false);
    }
}
