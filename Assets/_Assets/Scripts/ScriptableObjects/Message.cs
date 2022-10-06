using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MessageType
{
    ALERT
}

[CreateAssetMenu(menuName = "Message", fileName = "Message")]
public class Message : ScriptableObject
{
    public MessageType messageType;
    public float BroadCastRadius;
    public LayerMask MessageReceivers;
}
