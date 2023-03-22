using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ReferenceManager : MonoBehaviour
{
    public GameObject Leaderboard;
    
    public GameObject PlayerCreation;
    public TMP_InputField PlayerNameInput;
    public GameObject PlayerNameStatus;
    public Button StartButton;
    
    public GameObject GameWindow;
    public TMP_Text TimerText;
    public GameObject GameContainer;
    
    public GameObject NumbersContainer;
    public GameObject NumberItem;
    
    public GameObject OperationsContainer;
    public GameObject AdditionDraggable;
    public GameObject SubtractionDraggable;
    public GameObject MultiplicationDraggable;
    public GameObject DivisionDraggable;
    
    public GameObject DropablesContainer;
    public GameObject GameStatus;
    
    public GameObject DroppableItem;
    
    
    
    public GameObject ConnectionManager;

    public GameObject GameResetButton;
    public GameObject LevelResetButton;

    public GameObject LevelCompletionStatus;

    public TMP_Text Score;
    public TMP_Text PlayerName;
    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Connection());
    }

    private IEnumerator Connection()
    {
        yield return new WaitForSeconds(1f);
        
        ConnectionManager.SetActive(true);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
