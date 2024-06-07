using UnityEngine;
using System.Collections.Generic;
public class AStarPathfinding {

    public static List<Vector2Int> FindPath(AStarNode[,] grid, Vector2Int relativeStart, Vector2Int relativeEnd) {
        // Create a list to store the path
        List<Vector2Int> path = new List<Vector2Int>();

        // Set the start and end nodes
        AStarNode startNode = grid[relativeStart.x, relativeStart.y];
        AStarNode endNode = grid[relativeEnd.x, relativeEnd.y];

        // Create lists to store the open and closed nodes
        // Open nodes are nodes that have been visited but not yet checked
        List<AStarNode> openSet = new List<AStarNode>();
        // Closed nodes are nodes that have been checked
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        // Add the start node to the open set
        openSet.Add(startNode);

        // Loop until the open set is empty
        while (openSet.Count > 0 ){
            // Set the current node to the first node in the open set
            AStarNode currentNode = openSet[0];

            // Loop through the open set to find the node with the lowest fCost
            for (int i=1; i<openSet.Count; i++){
                // If the fCost of the current node is lower than the fCost of the node at index i
                // or if the fCosts are equal but the hCost of the current node is lower than the hCost of the node at index i
                // then set the current node to the node at index i
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost){
                    currentNode = openSet[i];
                }
            }

            // Remove the current node from the open set
            openSet.Remove(currentNode);
            // Add the current node to the closed set
            closedSet.Add(currentNode); 

            // If the current node is the end node
            if (currentNode == endNode){
                // Reconstruct the path
                path = RetracePath(startNode, endNode);
                return path;
            }

            // Loop through the current node's neighbors
            foreach (AStarNode neighbor in GetNeighbors(grid, currentNode)){
                // If the neighbor is not walkable or if the neighbor is in the closed set
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)){
                    // Skip to the next neighbor
                    continue;
                }

                // Calculate the new gCost of the neighbor
                int newGCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                // If the new gCost is lower than the neighbor's gCost
                // or if the neighbor is not in the open set
                // then update the neighbor's gCost, hCost, and parent
                if (newGCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)){
                    neighbor.gCost = newGCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    // If the neighbor is not in the open set
                    if (!openSet.Contains(neighbor)){
                        // Add the neighbor to the open set
                        openSet.Add(neighbor);
                    }
                }

            }

        }
        return path;

    }

    // Retrace the path from the end node to the start node
    private static List<Vector2Int> RetracePath(AStarNode startNode, AStarNode endNode){
        // Create a list to store the path
        List<Vector2Int> path = new List<Vector2Int>();
        // Set the current node to the end node
        AStarNode currentNode = endNode;
        // Loop until the current node is the start node
        while (currentNode != startNode){
            // Add the current node's position to the path
            path.Add(currentNode.position);
            // Set the current node to its parent
            currentNode = currentNode.parent;
        }
        // Reverse the path
        path.Reverse();
        return path;
    }

    // Get the distance between two nodes
    private static int GetDistance(AStarNode nodeA, AStarNode nodeB){
        // Calculate the distance in the x and y directions
        int dstX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int dstY = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        // If the distance in the x direction is greater than the distance in the y direction
        if (dstX > dstY){
            // Return the distance in the y direction multiplied by 14 plus the distance in the x direction minus the distance in the y direction multiplied by 10
            return 14 * dstY + 10 * (dstX - dstY);
        }
        // Otherwise, return the distance in the x direction multiplied by 14 plus the distance in the y direction minus the distance in the x direction multiplied by 10
        return 14 * dstX + 10 * (dstY - dstX);

    }

    // Get the neighbors of a node
    private static List<AStarNode> GetNeighbors(AStarNode[,] aStarNodeGrid, AStarNode node){
        // Create a list to store the neighbors
        List<AStarNode> neighbors = new List<AStarNode>();

        // Loop through the x and y offsets of the neighbors
        for (int x=-1; x<=1; x++){
            for (int y=-1; y<=1; y++){
                // If the x and y offsets are both 0
                if (x == 0 && y == 0){
                    // Skip to the next iteration
                    continue;
                }
                // Calculate the position of the neighbor
                int checkX = node.position.x + x;
                int checkY = node.position.y + y;
                // If the position of the neighbor is within the grid
                int startX = aStarNodeGrid[0,0].position.x;
                int startY = aStarNodeGrid[0,0].position.y;
                int xLowerBound = startX;
                int xUpperBound = xLowerBound + aStarNodeGrid.GetLength(0);
                int yLowerBound = startY;
                int yUpperBound = yLowerBound + aStarNodeGrid.GetLength(1);

                if (checkX >= xLowerBound && checkX < xUpperBound && checkY >= yLowerBound && checkY < yUpperBound){
                    //if the neighbor is walkable add it to the list of neighbors
                    //a diagonal neighbor is only walkable if both the horizontal and vertical neighbors are walkable
                    if (x != 0 && y != 0){
                        if (aStarNodeGrid[checkX - startX, node.position.y - startY].isWalkable && aStarNodeGrid[node.position.x - startX, checkY - startY].isWalkable){
                            neighbors.Add(aStarNodeGrid[checkX - startX, checkY - startY]);
                        }
                    } else {
                        if (aStarNodeGrid[checkX - startX, checkY - startY].isWalkable){
                            neighbors.Add(aStarNodeGrid[checkX - startX, checkY - startY]);
                        }
                    }
                }
            }
        }
        return neighbors;
    }

}