using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenarioEntity : MonoBehaviour
{
    public bool entity = true;

    public Vector3Int currentTileIndex;

    public bool tileBlock = true;

    public GameManager gameManager;

    public Color mouseOverColor = Color.white;

    protected Color baseColor;

    protected SpriteRenderer spriteRenderer;

    public virtual void Start()
    {
        gameManager = Manager.Instance.gameManager;
        if (entity)
        {
            currentTileIndex = Manager.Instance.gameManager.tilemap.WorldToCell(this.transform.position);
            gameManager.cenarioEntities.Add(this);
        }
    }

    public virtual void EventInTile(CreatureController controller)
    {

    }

    public virtual void OnMouseEnter() { gameManager.cenarioEntitiesMouseOn.Add(this); }

    public virtual void OnMouseOver(){
        if (entity)
        {
            spriteRenderer.color = mouseOverColor;
        }
    }

    public virtual void OnMouseExit(){
        gameManager.cenarioEntitiesMouseOn.Remove(this);
        if (entity)
        {
            spriteRenderer.color = baseColor;
        }
    }

    public virtual void OnMouseDown(){}
}
