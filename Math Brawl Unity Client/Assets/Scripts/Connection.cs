using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using Newtonsoft.Json;
using TMPro;

public class Connection : MonoBehaviour
{
    public string Url = "ws://localhost:5000";
    
    private WebSocket WebSocket;

    public static Payload ClientPayload = new Payload();

    public ReferenceManager Refs;

    async void Start()
    {
        ConnectWs();
    }

    public async void ConnectWs()
    {
        WebSocket = new WebSocket(Url);

        WebSocket.OnOpen += WebSocketOnOpen;
        WebSocket.OnError += WebSocketOnError;
        WebSocket.OnClose += WebSocketOnClose;
        WebSocket.OnMessage += WebSocketOnMessage;

        await WebSocket.Connect();
    }

    private void WebSocketOnMessage(byte[] bytes)
    {
        var message = System.Text.Encoding.UTF8.GetString(bytes);
        Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);

        if (message.Contains("ConnID"))
        {
            ClientPayload.PlayerId = Guid.Parse(message.TrimStart("ConnID: ".ToCharArray()));
            ClientPayload.Client = "player";
            Refs.PlayerCreation.SetActive(true);
        }
        else
        {
            ProcessGameMessages(message);
        }
    }

    private void ProcessGameMessages(string message)
    {
        var payload = JsonConvert.DeserializeObject<Payload>(message);
        
        switch (payload.Status)
        {
            case "room":
                Refs.PlayerCreation.SetActive(false);
                // activate game window
                Refs.GameWindow.SetActive(true);
                
                // activate waiting for opponent
                break;
            case "playing":
                Refs.PlayerCreation.SetActive(false);
                // activate game window
                Refs.GameWindow.SetActive(true);
                
                Refs.GameStatus.SetActive(false);
                
                Refs.TimerText.text = payload.Level.Time.ToString();
                
                Refs.NumbersContainer.SetActive(true);
                // instantiate all numbers
                foreach (var number in payload.Level.Numbers)
                {
                    var item = Instantiate(Refs.NumberItem, Refs.NumbersContainer.transform);
                    item.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
                }
                
                Refs.OperationsContainer.SetActive(true);
                // instantiate all operations
                
                Refs.DropablesContainer.SetActive(true);
                // instantiate all droppables
                
                break;;
            default:
                Debug.LogError("unknown player status!");
                break;
        }
    }

    private void WebSocketOnClose(WebSocketCloseCode e)
    {
        Debug.Log("Connection closed! " + e);
    }

    private void WebSocketOnError(string e)
    {
        Debug.Log("Error! " + e);
    }

    private void WebSocketOnOpen()
    {
        Debug.Log("Connection open!");
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            WebSocket.DispatchMessageQueue();
        #endif
    }
    
    async void SendMessage(string message)
    {
        if (WebSocket.State == WebSocketState.Open)
        {
            await WebSocket.SendText(message);
        }
    }
    
    async void SendMessage(byte[] bytes)
    {
        if (WebSocket.State == WebSocketState.Open)
        {
            await WebSocket.Send(bytes);
        }
    }

    private async void OnApplicationQuit()
    {
        await WebSocket.Close();
    }

    public void CreateUser()
    {
        if (!string.IsNullOrEmpty(Refs.PlayerNameInput.text))
        {
            Refs.PlayerNameStatus.SetActive(false);
            
            ClientPayload.Type = "user-creation";
            ClientPayload.PlayerName = Refs.PlayerNameInput.text;
            SendMessage(JsonConvert.SerializeObject(ClientPayload));
        }
        else
        {
            Refs.PlayerNameStatus.SetActive(true);
        }
    }
}
