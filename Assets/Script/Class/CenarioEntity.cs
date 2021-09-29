using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenarioEntity : MonoBehaviour
{
    public bool entity = true;

    public Vector3Int currentTileIndex;

    public virtual void Start()
    {
        Debug.Log("dadwa");
        if (entity)
        {
            Debug.Log(this.transform.position);
            currentTileIndex = Manager.Instance.gameManager.tilemap.WorldToCell(this.transform.position);
            Manager.Instance.gameManager.cenarioEntities.Add(this);
        }
    }
}
