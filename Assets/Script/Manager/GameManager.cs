using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Game manager geral
/// </summary>
public class GameManager : MonoBehaviour
{
    public Tilemap tilemap;

    public Tilemap collisionTM;

    private void Awake()
    {
        Manager.Instance.gameManager = this;
    }

    public Transform particleClick;

    private bool inPause;

    public List<GameObject> creatures = new List<GameObject>();

    public int currenteCreature = 0;

    public bool InPause { get => this.inPause; set => this.inPause = value; }//pausa o jogo quando abrir o menu

    /// <summary>
    /// Carrega a particula na posição do mouse
    /// </summary>
    /// <param name="position">posição do mouse</param>
    public void MoveParticle(Vector2 position)
    {
        particleClick.transform.position = position;
        particleClick.GetComponent<ParticleSystem>().Play();
    }

    string lastDirection = "S";

    public string GetDirection(Vector3Int startIndex, Vector3Int destinationIndex)
    {
        Vector3Int index = MathfCustom.Sign(destinationIndex - startIndex);

        if (index == new Vector3Int(1, 1, 0)) lastDirection = "N";
        if (index == new Vector3Int(1, 0, 0)) lastDirection = "NE";
        if (index == new Vector3Int(1, -1, 0)) lastDirection = "E";
        if (index == new Vector3Int(0, -1, 0)) lastDirection = "SE";
        if (index == new Vector3Int(-1, -1, 0)) lastDirection = "S";
        if (index == new Vector3Int(-1, 0, 0)) lastDirection = "SW";
        if (index == new Vector3Int(-1, 1, 0)) lastDirection = "W";
        if (index == new Vector3Int(0, 1, 0)) lastDirection = "NW";

        return lastDirection;
    }

    public void EndMyTurn(CharacterController cController = null)
    {
        currenteCreature++;
        if (currenteCreature >= creatures.Count)
        {
            currenteCreature = 0;
        }

        if(cController != null)
            cController.myTurn = false;

        if (creatures[currenteCreature] == null)
            EndMyTurn();

        if (creatures[currenteCreature].GetComponent<CharacterController>())
        {
            CharacterController playerController = creatures[currenteCreature].GetComponent<CharacterController>();

            if (playerController.CharacterStatus.Hp > 0)
                StartCoroutine(playerController.StartMyTurn());
        }
        else
        {
            EnemyController enemyController = creatures[currenteCreature].GetComponent<EnemyController>();

            if(enemyController.enemy.hp > 0)
                StartCoroutine(enemyController.StartMyTurn());
        }
    }
}
