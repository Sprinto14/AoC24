
namespace Day01
{
    using System;
    using System.IO;

    class Solution 
    {
        static List<List<int>> ParseLists(string filePath)
        {
            List<List<int>> numsLists = [];

            try 
            {
                StreamReader reader = new(filePath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] - '0' >= 0 && line[0] - '0' <= 9)
                    {
                        string[] lineItems = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                        int[] newNums = lineItems.Select(int.Parse).ToArray();
                        
                        while (newNums.Length > numsLists.Count) 
                        {
                            numsLists.Add([]);
                        }  

                        for (int i = 0; i < newNums.Length; i++) 
                        {
                            numsLists[i].Add(newNums[i]);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Unable to read file {0}", filePath);
                Console.WriteLine(e.Message);
                return [];
            }

            return numsLists;
        }

        static int Part1(string filePath)
        {
            List<List<int>> numsLists = ParseLists(filePath);

            // Now we have our input, we can actually solve the puzzle

            // Sort each list
            foreach (List<int> l in numsLists) 
            {
                l.Sort();
            }

            // Take the difference between adjacent items in the lists
            int output = 0;
            foreach (var item in numsLists[0].Zip(numsLists[1], Tuple.Create))
            {
                output += Math.Abs(item.Item1 - item.Item2);
            }

            return output;
        }

        static int Part2(string filePath)
        {
            List<List<int>> numsLists = ParseLists(filePath);

            // Count how many of each number appears in each list
            List<Dictionary<int, int>> numCounts = [];

            foreach (List<int> l in numsLists)
            {
                Dictionary<int, int> curNumCounts = [];
                foreach (int n in l) 
                {
                    if (!curNumCounts.ContainsKey(n)) 
                    {
                        curNumCounts[n] = 1;
                    }
                    else
                    {
                        curNumCounts[n]++;
                    }
                }
                numCounts.Add(curNumCounts);
            }

            // Multiply out the number of each value in the lists
            int output = 0;
            foreach (KeyValuePair<int, int> item in numCounts[0])
            {
                output += item.Key * item.Value * numCounts[1].GetValueOrDefault(item.Key, 0);
            }

            return output;
        }

        static void Main(string[] args) 
        {
            if (args.Length != 1) {
                Console.WriteLine("Usage dotnet run -- filePath");
                return;
            }

            Console.WriteLine("Part 1 Solution: {0}", Part1(args[0]));
            Console.WriteLine("Part 2 Solution: {0}", Part2(args[0]));
        }
    };
    
}