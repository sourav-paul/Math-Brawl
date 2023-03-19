using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceManager : MonoBehaviour
{
    public GameObject Leaderboard;
    
    public GameObject PlayerCreation;
    public TMP_InputField PlayerNameInput;
    public GameObject PlayerNameStatus;
    public Button StartButton;
    
    public GameObject GameWindow;

    public GameObject ConnectionManager;

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
