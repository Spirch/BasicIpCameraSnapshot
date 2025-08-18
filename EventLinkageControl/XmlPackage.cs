using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EventLinkageControl;

public static class EventTriggerFactory
{
    public static EventTrigger NewEventTrigger(string eventType, string action)
    {
        return new EventTrigger()
        {
            Id = eventType + "-1",
            EventType = eventType,
            EventTriggerNotificationList = new()
            {
                EventTriggerNotification = string.Equals(action, "off", StringComparison.OrdinalIgnoreCase) ?
                new()
                {
                    new() { Id = "center", NotificationMethod = "center" },
                    new() { Id = "FTP", NotificationMethod = "FTP" },
                } :
                new()
                {
                    new() { Id = "email", NotificationMethod = "email" },
                    new() { Id = "center", NotificationMethod = "center" },
                    new() { Id = "FTP", NotificationMethod = "FTP" },
                },
            }
        };
    }
}



[XmlRoot(ElementName = "EventTriggerNotification")]
public class EventTriggerNotification
{

    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "notificationMethod")]
    public string NotificationMethod { get; set; }
}

[XmlRoot(ElementName = "EventTriggerNotificationList")]
public class EventTriggerNotificationList
{

    [XmlElement(ElementName = "EventTriggerNotification")]
    public List<EventTriggerNotification> EventTriggerNotification { get; set; }
}

[XmlRoot(ElementName = "EventTrigger")]
public class EventTrigger
{

    [XmlElement(ElementName = "id")]
    public string Id { get; set; }

    [XmlElement(ElementName = "eventType")]
    public string EventType { get; set; }

    [XmlElement(ElementName = "videoInputChannelID")]
    public int VideoInputChannelID { get; set; } = 1;

    [XmlElement(ElementName = "EventTriggerNotificationList")]
    public EventTriggerNotificationList EventTriggerNotificationList { get; set; }
}

