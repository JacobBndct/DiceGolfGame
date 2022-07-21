using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Tile
{
    // Initialize movementCost of tile right before first frame of the game
    void Start()
    {
        SetMovementCost(1);
        gameObject.tag = "Grass";
    }

    // Implementation of abstract function "tileInteraction" of class tile which is called by DieMovement script when die ends on a tile
    public override void TileInteraction()
    {
        Debug.Log("landed on grass Tile");
        return;
    }
}
