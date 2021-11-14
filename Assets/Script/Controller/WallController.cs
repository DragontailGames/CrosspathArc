using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float offsetY;

    public Vector3Int currentTile;

    public void CreateWall(Vector3Int currentTile)
    {
        this.currentTile = currentTile;
        Vector3 pos = Manager.Instance.gameManager.tilemap.CellToLocal(currentTile);
        pos.y += offsetY;
        this.transform.position = pos;
    }
}
