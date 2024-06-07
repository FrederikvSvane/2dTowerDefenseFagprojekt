using UnityEngine;
using System.Collections.Generic;
public class AStarNode{
    //gCost = distance from the starting node
    public int gCost;

    //hCost = distance from the end node
    public int hCost;

    //fCost = gCost + hCost
    public int fCost { get { return gCost + hCost; } }
    
    //parent = the node that this node came from
    public AStarNode parent;

    //isWalkable = whether or not the node can be walked on
    public bool isWalkable;

    //position = the position of the node in the grid
    public Vector2Int position;

}
