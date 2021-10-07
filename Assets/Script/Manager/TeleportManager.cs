using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    #region singleton
    private static TeleportManager _instance;

    public static TeleportManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TeleportManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public Vector3Int destinationTile;

    public void TestTeleport()
    {
        if (destinationTile != Vector3Int.zero)
        {
            CreatureController controller = Manager.Instance.characterController;

            Vector3 destinyPosition = Manager.Instance.gameManager.tilemap.CellToWorld(destinationTile);

            if (controller.GetComponent<CharacterMoveTileIsometric>())
            {
                CharacterMoveTileIsometric characterMoveTileIsometric = controller.GetComponent<CharacterMoveTileIsometric>();
                destinyPosition += characterMoveTileIsometric.offsetPosition;
                characterMoveTileIsometric.movePosition = destinyPosition;
            }

            controller.direction = "S";
            controller.transform.position = destinyPosition;
            controller.currentTileIndex = destinationTile;

            destinationTile = Vector3Int.zero;
        }
    }
}
