using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

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
                // remove all numbers for next level if any
                if (Refs.NumbersContainer.transform.childCount>0)
                {
                    for (int i = 0; i < Refs.NumbersContainer.transform.childCount; i++)
                    {
                        Destroy(Refs.NumbersContainer.transform.GetChild(i));
                    }
                }
                // instantiate all numbers
                foreach (var number in payload.Level.Numbers)
                {
                    var item = Instantiate(Refs.NumberItem, Refs.NumbersContainer.transform);
                    item.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
                }
                // disable horizontal layout group
                Refs.NumbersContainer.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
                // initialize items
                for (int i = 0; i < Refs.NumbersContainer.transform.childCount; i++)
                {
                    Refs.DropablesContainer.transform.GetChild(i).GetComponent<Drag>().Init();
                }
                    
                Refs.OperationsContainer.SetActive(true);
                // disable all ops for next level
                Refs.AdditionDraggable.SetActive(false);
                Refs.SubtractionDraggable.SetActive(false);
                Refs.MultiplicationDraggable.SetActive(false);
                Refs.DivisionDraggable.SetActive(false);
                // instantiate all operations
                foreach (var operation in payload.Level.Operations)
                {
                    switch (operation)
                    {
                        case LevelGenerator.Operation.Addition:
                            Refs.AdditionDraggable.SetActive(true);
                            break;
                        case LevelGenerator.Operation.Subtraction:
                            Refs.SubtractionDraggable.SetActive(true);
                            break;
                        case LevelGenerator.Operation.Multiplication:
                            Refs.MultiplicationDraggable.SetActive(true);
                            break;
                        case LevelGenerator.Operation.Division:
                            Refs.DivisionDraggable.SetActive(true);
                            break;
                        case LevelGenerator.Operation.NotSet:
                        default:
                            Console.WriteLine("Unknown operation!");
                            break;
                    }
                }
                // disable horizontal layout group
                Refs.OperationsContainer.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
                
                Refs.DropablesContainer.SetActive(true);
                // remove all existing droppables for next level if any
                if (Refs.DropablesContainer.transform.childCount>0)
                {
                    for (int i = 0; i < Refs.DropablesContainer.transform.childCount; i++)
                    {
                        Destroy(Refs.DropablesContainer.transform.GetChild(i));
                    }
                }
                // instantiate all droppables
                foreach (var number in payload.Level.Numbers)
                {
                    var item = Instantiate(Refs.DroppableItem, Refs.DropablesContainer.transform);
                    item.transform.GetChild(0).GetComponent<TMP_Text>().text = "?";
                }
                var equal = Instantiate(Refs.DroppableItem, Refs.DropablesContainer.transform);
                equal.transform.GetChild(0).GetComponent<TMP_Text>().text = "=";
                var equalImage = equal.transform.GetComponent<Image>();
                equalImage.raycastTarget = false;
                equalImage.maskable = false;
                equalImage.fillCenter = false;

                var solution = Instantiate(Refs.DroppableItem, Refs.DropablesContainer.transform);
                solution.transform.GetChild(0).GetComponent<TMP_Text>().text = payload.Level.Solution.ToString();
                var solutionImage = solution.transform.GetComponent<Image>();
                solutionImage.raycastTarget = false;
                solutionImage.maskable = false;
                solutionImage.fillCenter = false;
                
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
