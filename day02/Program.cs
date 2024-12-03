
namespace Day02
{
    using System;
    using System.IO;

    class Solution 
    {
        static bool JumpIsSafe(int l, int r, int mul, int minJump = 1, int maxJump = 3)
        {
            int diff = (r - l) * mul;
            // Console.WriteLine("diff = {0} - {1} {2} {3}", diff, minJump, maxJump, diff >= minJump && diff <= maxJump);
            return diff >= minJump && diff <= maxJump;
        }

        static bool IsSafe(ref List<int> arr, int minJump = 1, int maxJump = 3)
        {
            // If the list is only 1 or no items, then it's safe
            if (arr.Count < 2) return true;

            // Determine if it's ascending (+1) or descending (-1)
            // This multiplier is then used to account for positive/negative steps later
            int mul = (arr[1] > arr[0]) ? 1 : -1;

            // Go through the array in order and check for any steps less than the minJump or more than the maxJump
            // We still check between the first two in case they are the same value
            for (int i = 1; i < arr.Count; i++)
            {
                if (!JumpIsSafe(arr[i - 1], arr[i], mul, minJump, maxJump)) return false;
            }

            // If no bad jumps were found, it's safe
            return true;
        }

        /*
        static bool IsSafeWithinToleranceInDir(ref int[] arr, int dir, int minJump = 1, int maxJump = 3)
        {
            /// Checks if the report is safe within a tolerance of 1
            /// Tries to achieve this in O(n) time using a two pointers method
            /// This does not work and I don't know why
            
            // If the list is only 1 or no items (after removing a level for each tolerance), then it's safe
            if (arr.Length < 3) return true;

            // Go through the array in order and check for any steps less than the minJump or more than the maxJump
            // We still check between the first two in case they are the same value
            bool badJumpSeen = false;
            int l = 0, r = 1;
            while (r < arr.Length)
            {
                Console.WriteLine("l, r: {0}, {1}", l, r);
                // If the level jumps are safe, then just continue as normal
                if (JumpIsSafe(arr[l], arr[r], dir, minJump, maxJump))
                {
                    l = r;
                    r++;
                    continue;
                }

                // Otherwise, there is an unsafe level jump
                // If this is the second bad jump, then the report is unsafe
                if (badJumpSeen) return false;

                badJumpSeen = true;

                // Otherwise, we need to discard one of the two items we are currently considering

                // Revert from the current previous item to the one before and check if this resolves the problem
                // If the left item is the first item (l == 0), then this passes by default, but we need to recalculate 'mul'
                // First try the left item
                if (l == 0 || JumpIsSafe(arr[r], arr[l - 1], dir, minJump, maxJump))
                {
                    l = r;
                    r++;
                }
                else // Otherwise, try the right item
                {
                    r++;
                }
            }

            // If no bad jumps were found, it's safe
            return true;
        }
        */

        static bool IsSafeWithinTolerance(ref List<int> arr, int tolerance = 0, int minJump = 1, int maxJump = 3, int dir = 0, int start = 0)
        {
            // If the array is only 1 or no items (after we remove tolerances), then it is safe
            if (arr.Count < 2 + tolerance) return true;

            // If we have no more tolerance for errors, we can use the original solution (so long as the direction is the same)
            if (tolerance == 0) {
                return IsSafe(ref arr, minJump, maxJump); //(((arr[1] > arr[0]) ? 1 : -1) == dir) && 
            }

            // If we are at the start, we need to handle the special case for getting the wrong direction at the start (if the first number needs to be removed)
            // To fix this, try to solve in the direction of the starting pair (if it is a valid step)
            // If this fails, then we try only branches without the first item
            if (start == 0)
            {
                dir = (arr[1] > arr[0]) ? 1 : -1;
                if (IsSafeWithinTolerance(ref arr, tolerance, minJump, maxJump, dir, 1)) return true;

                // If this does not work, remove the start and try again
                arr.RemoveAt(0);
                return IsSafeWithinTolerance(ref arr, tolerance - 1, minJump, maxJump, dir, 0);
            }

            // Otherwise, scan through from the start index until we hit the end or find a problem
            for (int i = start + 1; i < arr.Count; i++)
            {
                // Iterate through so long as all jumps are safe
                if (JumpIsSafe(arr[i - 1], arr[i], dir, minJump, maxJump)) continue;
                

                // If there is an unsafe jump, then we branch, removing the left or right item in the pair (i-1, i)

                // Left
                int temp = arr[i-1];
                arr.RemoveAt(i-1);
                if (IsSafeWithinTolerance(ref arr, tolerance - 1, minJump, maxJump, dir, i - 2)) return true;
                arr.Insert(i - 1, temp);

                // Right
                temp = arr[i];
                arr.RemoveAt(i);
                if (IsSafeWithinTolerance(ref arr, tolerance - 1, minJump, maxJump, dir, i - 1)) return true;
                arr.Insert(i, temp);

                // If neither left nor right branch works, then this path will not solve
                // We needed to reform the arr in case we have branched at the start
                return false;
            }

            return true;
        }

        static int Part1(string filePath)
        {
            int output = 0;

            try 
            {
                StreamReader reader = new(filePath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] - '0' >= 0 && line[0] - '0' <= 9)
                    {
                        string[] lineItems = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                        List<int> nums = lineItems.Select(int.Parse).ToList();

                        output += IsSafe(ref nums) ? 1 : 0;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Unable to read file {0}", filePath);
                Console.WriteLine(e.Message);
                return -1;
            }

            return output;
        }

        static int Part2(string filePath, int tolerance = 1)
        {
            // Similar to Part 1, but now when we find an unsafe region in a report, we need to try removing each of the two possible problem numbers
            // We need to account for the fact that now any 2-number report is safe
            // The really annoying ones to fix will be if the first or last number is wrong (as we use these to find the ascending/descending multiplier)
            int output = 0;

            try 
            {
                StreamReader reader = new(filePath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line[0] - '0' >= 0 && line[0] - '0' <= 9)
                    {
                        string[] lineItems = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                        List<int> nums = lineItems.Select(int.Parse).ToList();

                        output += IsSafeWithinTolerance(ref nums, tolerance) ? 1 : 0;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Unable to read file {0}", filePath);
                Console.WriteLine(e.Message);
                return -1;
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