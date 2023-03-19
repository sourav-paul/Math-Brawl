using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class Connection : MonoBehaviour
{
    public string Url = "ws://localhost:5000";
    
    private WebSocket WebSocket;
    
    async void Start()
    {
        ConnectWs();
    }

    public async void ConnectWs()
    {
        WebSocket = new WebSocket(Url);

        WebSocket.OnOpen += WebSocketOnOpen();
        WebSocket.OnError += WebSocketOnError();
        WebSocket.OnClose += WebSocketOnClose();
        WebSocket.OnMessage += WebSocketOnMessage();

        await WebSocket.Connect();
    }

    private static WebSocketMessageEventHandler WebSocketOnMessage()
    {
        return (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        };
    }

    private static WebSocketCloseEventHandler WebSocketOnClose()
    {
        return (e) =>
        {
            Debug.Log("Connection closed!");
        };
    }

    private static WebSocketErrorEventHandler WebSocketOnError()
    {
        return (e) =>
        {
            Debug.Log("Error! " + e);
        };
    }

    private static WebSocketOpenEventHandler WebSocketOnOpen()
    {
        return () =>
        {
            Debug.Log("Connection open!");
        };
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            WebSocket.DispatchMessageQueue();
        #endif
    }

    async void SendWebSocketMessage()
    {
        if (WebSocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await WebSocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await WebSocket.SendText("plain text message");
        }
    }

    private async void OnApplicationQuit()
    {
        await WebSocket.Close();
    }
}
