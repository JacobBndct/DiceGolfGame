using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    // Private fields for tiles
    private int movementCost;

    // Abstract functions
    public abstract void TileInteraction();

    // getters and setters
    public int GetMovementCost()
    {
        return movementCost;
    }
    public void SetMovementCost(int newMovementCost)
    {
        this.movementCost = newMovementCost;
    }
}