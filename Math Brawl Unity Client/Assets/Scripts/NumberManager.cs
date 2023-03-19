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
        var level = LevelGenerator.GenerateIntegers(totalNumbers);

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < level.Numbers.Count; i++)
        {
            if (i <= level.Operations.Count-1)
            {
                stringBuilder.Append(level.Numbers[i]).Append(level.Operations[i]);
            }
            else
            {
                stringBuilder.Append(level.Numbers[i]);
            }
        }
        Debug.Log(stringBuilder.ToString() + "=" + level.Solution);
    }
}
