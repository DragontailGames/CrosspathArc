using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float offsetY;

    public Vector3Int currentTile;

    public int duration;

    public void StartRound()
    {
        duration--;
        if(duration<= 0)
        {
            DestroyImmediate(this.gameObject);
        }
    }

    public void CreateWall(Vector3Int currentTile, int duration)
    {
        this.currentTile = currentTile;
        Vector3 pos = Manager.Instance.gameManager.tilemap.CellToLocal(currentTile);
        pos.y += offsetY;
        this.transform.position = pos;
        this.duration = duration;
    }
}
