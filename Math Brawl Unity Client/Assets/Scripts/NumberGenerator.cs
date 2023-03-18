using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NumberGenerator
{
    public class Level
    {
        public List<int> Numbers { get; set; }
        public List<string> Operations { get; set; }
        public double Solution { get; set; }
    }
    
    public enum Operation
    {
        NotSet         = -1,
        Addition       = 10,
        Subtraction    = 20,
        Multiplication = 30,
        Division       = 40
    }
    
    public static readonly Dictionary<Operation, string> AvailableOperationsMap = new Dictionary<Operation, string>()
    {
        { Operation.Addition,       "+"      },
        { Operation.Subtraction,    "-"      },
        { Operation.Multiplication, "x"      },
        { Operation.Division,       "\u00F7" }
    };

    public static Level GenerateIntegers(int totalNumbers = 2)
    {
        List<int> numbers = new List<int>()                {1,2,4,5,2,8,2};
        List<Operation> operations = new List<Operation>() { Operation.Addition , Operation.Multiplication, Operation.Subtraction, Operation.Addition, Operation.Multiplication, Operation.Subtraction};
        int additions = 2;
        int subtractions = 2;
        int multiplications = 2;
        int divisions = 0;
        double solution = 0;
        
        // GetRandomNumbers(totalNumbers, numbers);
        // GetRandomOperations(totalNumbers, operations);
        // GetOperationCounters(operations, ref additions, ref subtractions, ref multiplications, ref divisions);

        numbers    = AdjustNumberIndices(numbers);
        operations = AdjustOperationIndices(operations);

        
        
        while (numbers.Count != 0 && operations.Count != 0)
        {
            var no = new List<int>();
            no.AddRange(numbers);
            var op = new List<Operation>();
            op.AddRange(operations);

            int tempResult = 0;

            if (multiplications != 0)
            {
                for (int i = 0; i < op.Count; i++)
                {
                    if (op[i] == Operation.Multiplication)
                    {
                        tempResult += no[i - 1] * no[i + 1];

                        numbers[i - 1] = tempResult;
                        operations[i - 1] = Operation.NotSet;

                        numbers.RemoveRange(i, 2);
                        operations.RemoveRange(i, 2);

                        multiplications--;
                    }
                }
                
                continue;
            }

            if (divisions != 0)
            {
                for (int i = 0; i < op.Count; i++)
                {
                    if (op[i] == Operation.Division)
                    {
                        tempResult += no[i - 1] / no[i + 1];

                        numbers[i - 1] = tempResult;
                        operations[i - 1] = Operation.NotSet;

                        numbers.RemoveRange(i, 2);
                        operations.RemoveRange(i, 2);

                        divisions--;
                    }
                }
                continue;
            } 
            
            if (additions != 0)
            {
                for (int i = 0; i < op.Count; i++)
                {
                    if (op[i] == Operation.Addition)
                    {
                        tempResult += no[i - 1] + no[i + 1];

                        numbers[i - 1] = tempResult;
                        operations[i - 1] = Operation.NotSet;

                        numbers.RemoveRange(i, 2);
                        operations.RemoveRange(i, 2);

                        additions--;
                    }
                }
                continue;
            }
            
            if (subtractions != 0)
            {
                for (int i = 0; i < op.Count; i++)
                {
                    if (op[i] == Operation.Subtraction)
                    {
                        tempResult += no[i - 1] - no[i + 1];

                        numbers[i - 1] = tempResult;
                        operations[i - 1] = Operation.NotSet;

                        numbers.RemoveRange(i, 2);
                        operations.RemoveRange(i, 2);

                        subtractions--;
                    }
                }
                continue;
            }
        }

        return new Level();
    }

    private static List<Operation> AdjustOperationIndices(List<Operation> operations)
    {
        List<Operation> adjusted = new List<Operation>();
        
        foreach (var operation in operations)
        {
            adjusted.Add(Operation.NotSet);
            adjusted.Add(operation);
        }

        adjusted.Add(Operation.NotSet);
        
        return adjusted;
    }

    private static List<int> AdjustNumberIndices(List<int> numbers)
    {
        List<int> adjusted = new List<int>();

        foreach (var number in numbers)
        {
            adjusted.Add(number);
            adjusted.Add(0);
        }

        adjusted.RemoveAt(adjusted.Count-1);
        
        return adjusted;
    }

    private static void GetOperationCounters(List<Operation> operations, ref int additions, ref int subtractions, ref int multiplications,
        ref int divisions)
    {
        foreach (var item in operations)
        {
            switch (item)
            {
                case Operation.Addition:
                    additions++;
                    break;
                case Operation.Subtraction:
                    subtractions++;
                    break;
                case Operation.Multiplication:
                    multiplications++;
                    break;
                case Operation.Division:
                    divisions++;
                    break;
                case Operation.NotSet:
                default:
                    Debug.Log("Unknown Operation!");
                    break;
            }
        }
    }

    private static void GetRandomOperations(int totalNumbers, List<Operation> operations)
    {
        for (int i = 0; i < totalNumbers - 1; i++)
        {
            operations.Add(AvailableOperationsMap.ElementAt(Random.Range(0, AvailableOperationsMap.Count - 1)).Key);
        }
    }

    private static void GetRandomNumbers(int totalNumbers, List<int> numbers)
    {
        for (int i = 0; i < totalNumbers; i++)
        {
            numbers.Add(Random.Range(1, 5));
        }
    }
}
