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

    public List<string> droppedItems = new List<string>();

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

    private Payload CurrentPayload = new Payload();
    private void ProcessGameMessages(string message)
    {
        var payload = JsonConvert.DeserializeObject<Payload>(message);
        CurrentPayload = payload;
        
        switch (payload.Status)
        {
            case "room":
                HandleInRoomWaitingState();

                break;
            case "playing":
                EnablePlayingStateUi();

                StartLevelTimer(payload);
                
                EnableNumbers(payload);

                EnableOperations(payload);

                EnableDroppables(payload);

                break;;
            default:
                Debug.LogError("unknown player status!");
                break;
        }
    }

    private void EnableDroppables(Payload payload)
    {
        Refs.DropablesContainer.SetActive(true);
        // remove all existing droppables for next level if any
        if (Refs.DropablesContainer.transform.childCount != 0)
        {
            for (int i = 0; i < Refs.DropablesContainer.transform.childCount; i++)
            {
                Destroy(Refs.DropablesContainer.transform.GetChild(i).gameObject);
            }
        }

        // instantiate all droppables
        for (int i = 0; i < payload.Level.Numbers.Count + payload.Level.Operations.Count; i++)
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
    }

    private void EnableOperations(Payload payload)
    {
        Refs.OperationsContainer.SetActive(true);
        // remove all existing droppables for next level if any
        if (Refs.OperationsContainer.transform.childCount != 0)
        {
            for (int i = 0; i < Refs.OperationsContainer.transform.childCount; i++)
            {
                Destroy(Refs.OperationsContainer.transform.GetChild(i).gameObject);
            }
        }
        
        // instantiate all operations
        foreach (var operation in payload.Level.Operations)
        {
            switch (operation)
            {
                case LevelGenerator.Operation.Addition:
                    var add = Instantiate(Refs.AdditionDraggable, Refs.OperationsContainer.transform);
                    add.GetComponent<Drag>().operation = operation;
                    break;
                case LevelGenerator.Operation.Subtraction:
                    var sub = Instantiate(Refs.SubtractionDraggable, Refs.OperationsContainer.transform);
                    sub.GetComponent<Drag>().operation = operation;
                    break;
                case LevelGenerator.Operation.Multiplication:
                    var multi = Instantiate(Refs.MultiplicationDraggable, Refs.OperationsContainer.transform);
                    multi.GetComponent<Drag>().operation = operation;
                    break;
                case LevelGenerator.Operation.Division:
                    var div = Instantiate(Refs.DivisionDraggable, Refs.OperationsContainer.transform);
                    div.GetComponent<Drag>().operation = operation;
                    break;
                case LevelGenerator.Operation.NotSet:
                default:
                    Console.WriteLine("Unknown operation!");
                    break;
            }
        }

        // disable horizontal layout group
        Refs.OperationsContainer.transform.GetComponent<HorizontalLayoutGroup>().enabled = true;
        // initialize items
        for (int i = 0; i < Refs.OperationsContainer.transform.childCount; i++)
        {
            Refs.OperationsContainer.transform.GetChild(i).GetComponent<Drag>().Init();
        }
    }

    private void EnableNumbers(Payload payload)
    {
        Refs.NumbersContainer.SetActive(true);
        // remove all numbers for next level if any
        if (Refs.NumbersContainer.transform.childCount != 0)
        {
            for (int i = 0; i < Refs.NumbersContainer.transform.childCount; i++)
            {
                Destroy(Refs.NumbersContainer.transform.GetChild(i).gameObject);
            }
        }

        // instantiate all numbers
        foreach (var number in payload.Level.Numbers)
        {
            var item = Instantiate(Refs.NumberItem, Refs.NumbersContainer.transform);
            item.transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
            item.GetComponent<Drag>().number = number;
        }

        // disable horizontal layout group
        // Refs.NumbersContainer.transform.GetComponent<HorizontalLayoutGroup>().enabled = true;
        // initialize items
        for (int i = 0; i < Refs.NumbersContainer.transform.childCount; i++)
        {
            Refs.NumbersContainer.transform.GetChild(i).GetComponent<Drag>().Init();
        }
    }

    private void StartLevelTimer(Payload payload)
    {
        // Refs.TimerText.text = payload.Level.Time.ToString();
        var timer = Refs.TimerText.GetComponent<Timer>();
        timer.currentLevelTimeLeft = payload.Level.Time.Seconds;
        timer.timerIsRunning = true;
    }

    private void EnablePlayingStateUi()
    {
        Refs.PlayerCreation.SetActive(false);
        // activate game window
        Refs.GameWindow.SetActive(true);

        Refs.GameStatus.SetActive(false);
        
        Refs.LevelResetButton.SetActive(true);
    }

    private void HandleInRoomWaitingState()
    {
        Refs.PlayerCreation.SetActive(false);
        // activate game window
        Refs.GameWindow.SetActive(true);
        // activate waiting for opponent
    }

    public void ResetGameUi()
    {
        EnableNumbers(CurrentPayload);
        EnableOperations(CurrentPayload);
        EnableDroppables(CurrentPayload);
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
