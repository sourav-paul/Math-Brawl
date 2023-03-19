using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
    
namespace MathBrawlServer
{
    public class LevelGenerator
    {
        public class Level
        {
            public List<int> Numbers { get; set; }
            public List<Operation> Operations { get; set; }
            public double Solution { get; set; }
            public TimeSpan Time { get; set; }
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
            List<int> numbers = new List<int>()                /*{ 1,2,4,5,2,8,2 }*/;
            List<Operation> operations = new List<Operation>() /*{ Operation.Addition , Operation.Multiplication, Operation.Subtraction, Operation.Addition, Operation.Multiplication, Operation.Subtraction}*/;
            int additions = 0;
            int subtractions = 0;
            int multiplications = 0;
            int divisions = 0;
            
            GetRandomNumbers(totalNumbers, numbers);
            GetRandomOperations(totalNumbers, operations);
            GetOperationCounters(operations, ref additions, ref subtractions, ref multiplications, ref divisions);

            var nums    = AdjustNumberIndices(numbers);
            var ops = AdjustOperationIndices(operations);

            CalculateSolution(nums, ops, multiplications, divisions, additions, subtractions);

            return CreateLevel(numbers, operations, nums);
        }

        private static Level CreateLevel(List<int> numbers, List<Operation> operations, List<int> nums)
        {
            Level level = new Level();
            level.Numbers = new List<int>();
            level.Numbers.AddRange(numbers);
            level.Operations = new List<Operation>();
            level.Operations.AddRange(operations);
            level.Solution = nums[0];
            level.Time = TimeSpan.FromSeconds(90);
            return level;
        }

        private static void CalculateSolution(List<int> nums, List<Operation> ops, int multiplications, int divisions, int additions,
            int subtractions)
        {
            while (nums.Count != 1 && ops.Count != 1)
            {
                var no = new List<int>();
                no.AddRange(nums);
                var op = new List<Operation>();
                op.AddRange(ops);

                int tempResult = 0;

                if (multiplications != 0 || divisions != 0)
                {
                    for (int i = 0; i < op.Count; i++)
                    {
                        if (op[i] == Operation.Multiplication)
                        {
                            tempResult += no[i - 1] * no[i + 1];

                            nums[i - 1] = tempResult;
                            ops[i - 1] = Operation.NotSet;

                            nums.RemoveRange(i, 2);
                            ops.RemoveRange(i, 2);

                            multiplications--;
                            break;
                        }

                        if (op[i] == Operation.Division)
                        {
                            tempResult += no[i - 1] / no[i + 1];

                            nums[i - 1] = tempResult;
                            ops[i - 1] = Operation.NotSet;

                            nums.RemoveRange(i, 2);
                            ops.RemoveRange(i, 2);

                            divisions--;
                            break;
                        }
                    }

                    continue;
                }

                if (additions != 0 || subtractions != 0)
                {
                    for (int i = 0; i < op.Count; i++)
                    {
                        if (op[i] == Operation.Addition)
                        {
                            tempResult += no[i - 1] + no[i + 1];

                            nums[i - 1] = tempResult;
                            ops[i - 1] = Operation.NotSet;

                            nums.RemoveRange(i, 2);
                            ops.RemoveRange(i, 2);

                            additions--;
                            break;
                        }

                        if (op[i] == Operation.Subtraction)
                        {
                            tempResult += no[i - 1] - no[i + 1];

                            nums[i - 1] = tempResult;
                            ops[i - 1] = Operation.NotSet;

                            nums.RemoveRange(i, 2);
                            ops.RemoveRange(i, 2);

                            subtractions--;
                            break;
                        }
                    }

                    continue;
                }
            }
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
                        Console.WriteLine("Unknown Operation!");
                        break;
                }
            }
        }

        private static void GetRandomOperations(int totalNumbers, List<Operation> operations)
        {
            Random random = new Random();
            
            for (int i = 0; i < totalNumbers - 1; i++)
            {
                operations.Add(AvailableOperationsMap.ElementAt(random.Next(0, AvailableOperationsMap.Count)).Key);
            }
        }

        private static void GetRandomNumbers(int totalNumbers, List<int> numbers)
        {
            Random random = new Random();
            
            for (int i = 0; i < totalNumbers; i++)
            {
                numbers.Add(random.Next(1, 5));
            }
        }
    }

}