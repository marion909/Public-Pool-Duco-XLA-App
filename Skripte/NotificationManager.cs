using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using Unity.Notifications;
using System;

public class NotificationManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var channel = new AndroidNotificationChannel(){

            Id = "my_channel_id",
            Name = "My Channel Name",
            Importance = Importance.High,
            Description = "My Channel Description"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Notification Title";
        notification.Text = "Notification Text";
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowNotification(string message, DateTime FT){

        var notification = new AndroidNotification();
        notification.Title = "Public-Pool.io";
        notification.Text = message;
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";
        notification.FireTime = FT;
        AndroidNotificationCenter.SendNotification(notification, "my_channel_id");
    }
}
