using System.Collections.Generic;
using UnityEngine;
using System;
public class AStarTester{

    public static void RunAStarTest()
    {

        int[,] gridPattern = new int[,] {
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 1, 1, 1, 0, 1, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 1, 0, 0, 1, 1, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 0, 0, 0, 1, 1, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 0, 0, 0, 1, 1, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };


        // Create a grid
        int width = 10;
        int height = 10;
        AStarNode[,] grid = new AStarNode[width, height];

        // Initialize the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new AStarNode();
                grid[x, y].position = new Vector2Int(x, y);
                grid[x, y].isWalkable = true;
                if (gridPattern[x, y] == 1)
                {
                    grid[x, y].isWalkable = false;
                }
            }
        }

        // Create starting and ending nodes
        Vector2Int startNode = new Vector2Int(0, 0);
        Vector2Int endNode = new Vector2Int(9, 9);

        // Perform the A* algorithm
        List<Vector2Int> path = AStarPathfinding.FindPath(grid, startNode, endNode);

        // Print the result
        if (path != null)
        {
            Console.WriteLine("Path found:");
            Console.WriteLine(path.Count + " steps");
            foreach (Vector2Int Coord in path)
            {
                gridPattern[Coord.x, Coord.y] = 2;
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (gridPattern[x, y] == 0)
                    {
                        Console.Write(" . ");
                    }
                    else if (gridPattern[x, y] == 1)
                    {
                        Console.Write("███");
                    }
                    else
                    {
                        Console.Write(" * ");
                    }
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No path found.");
        }
    }

    
    public static void Main(string[] args)
    {

        // Run the A* test
        RunAStarTest();
    }



    

    


}