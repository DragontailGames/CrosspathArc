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
                _instance = FindObjectOfType<TeleportManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<TeleportManager>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null || ReferenceEquals(this, _instance))
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public Vector3Int destinationTile;

    public void TestTeleport()
    {
        if (destinationTile != Vector3Int.zero)
        {
            CharacterController controller = Manager.Instance.characterController;
            controller.Awake();

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
