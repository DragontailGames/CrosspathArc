using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : CenarioEntity
{
    public Vector3Int destinyTile;

    public Teleporter destilyTeleporter;

    public string sceneToTeleport = "";

    public void Teleport(CreatureController controller)
    {
        if(sceneToTeleport != "" && destinyTile != Vector3Int.zero)
        {
            TeleportManager.Instance.destinationTile = destinyTile;
            Manager.Instance.sceneLoadManager.GotoScene(sceneToTeleport);
            return;
        }

        Vector3Int destinyIndex = new Vector3Int();

        if (destilyTeleporter != null)
        {
            destinyIndex = destilyTeleporter.currentTileIndex;
        }
        else
        {
            destinyIndex = destinyTile;
        }

        Vector3 destinyPosition = Manager.Instance.gameManager.tilemap.CellToWorld(destinyIndex);

        if(controller.GetComponent<CharacterMoveTileIsometric>())
        {
            CharacterMoveTileIsometric characterMoveTileIsometric = controller.GetComponent<CharacterMoveTileIsometric>();
            destinyPosition += characterMoveTileIsometric.offsetPosition;
            characterMoveTileIsometric.movePosition = destinyPosition;
        }

        controller.direction = "S";
        controller.transform.position = destinyPosition;
        controller.currentTileIndex = destinyIndex;

    }

    public override void EventInTile(CreatureController controller)
    {
        base.EventInTile(controller);
        Teleport(controller);
    }
}
