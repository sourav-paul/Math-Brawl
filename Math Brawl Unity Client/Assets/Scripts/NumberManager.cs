using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NumberManager : MonoBehaviour
{
    public int totalNumbers = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNumbers()
    {
        var numbers = NumberGenerator.GenerateIntegers(totalNumbers);
    }
}
