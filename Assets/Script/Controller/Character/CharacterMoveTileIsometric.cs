using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class CharacterMoveTileIsometric : MonoBehaviour
{
    public CharacterController controller;

    public GameManager gameManager;

    public float movementSpeed = 1;
    public int tileMove = 1;

    public Vector3 offsetPosition;
    public Vector3 movePosition;
    Vector3Int mouse = Vector3Int.zero;

    public Vector3Int Mouse { get => this.mouse; set => this.mouse = value; }

    //Resets iniciais
    public void Start()
    {
        movePosition = gameManager.tilemap.GetCellCenterWorld(controller.currentTileIndex) + offsetPosition;
    }

    public void FixedUpdate()
    {
        Vector3Int moveCell = Vector3Int.zero;

        if (!gameManager.InPause && controller.animator.isWalking == false && controller.delay)//if (!gameManager.InPause && controller.animator.GetBool("Walk") == false && controller.delay)//Testa o delay para correção da movimentação por tile
        {
            Vector3Int keyboard = GetMoveCellKeyboard();

            moveCell = Mouse != Vector3Int.zero ? Mouse : keyboard;//Detecta se deve usar o valor do teclado ou do mouse

            if (moveCell != Vector3Int.zero)
            {
                gameManager.EndMyTurn(controller);
                Mouse = Vector3Int.zero;

                if (CanMoveToTile(moveCell))
                {
                    controller.direction = controller.GetDirection(moveCell);
                    if(controller.direction != "W" && controller.direction != "E")
                    {
                        movementSpeed = 1.5f;
                    }
                    else
                    {
                        movementSpeed = 1.75f;
                    }

                    if (!controller.animator.isWalking)
                    {
                        controller.animator.PlayAnimation("Walk", controller.direction, false);
                        controller.animator.isWalking = true;
                    }
                    controller.currentTileIndex += moveCell * tileMove;
                    movePosition = gameManager.tilemap.GetCellCenterWorld(controller.currentTileIndex) + offsetPosition;
                    StartCoroutine(TestHasEntityInTile(controller.currentTileIndex));
                }
            }
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);

        if(Vector3.Distance(this.transform.position, movePosition) < 0.02)
        {
            if (controller.animator.isWalking == true)
            {
                controller.animator.isWalking = false;
                if (!controller.MouseOn())
                {
                    if (controller.direction == "")
                    {
                        controller.animator.PlayAnimation("Idle", "S", false);
                    }
                    else
                    {
                        controller.animator.PlayAnimation("Idle", controller.direction, false);
                    }
                }
            }
        }
    }

    public IEnumerator TestHasEntityInTile(Vector3Int tileIndex)
    {
        yield return new WaitForSeconds(0.2f);

        Manager.Instance.gameManager.cenarioEntities.Find(n => n.currentTileIndex == tileIndex && n.tileBlock == false)?.EventInTile(controller);

    }

    /// <summary>
    /// Função que retorna a posição pelo teclado
    /// </summary>
    public Vector3Int GetMoveCellKeyboard()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");//Recebe o input do jogador no eixo Horizontal(A/D,Seta para esquerda/Seta para direita)
        float vertical = Input.GetAxisRaw("Vertical");//Recebe o input do jogador no eixo Vertical(W/S,Seta para cima/Seta para baixo)
        return new Vector3Int((int)vertical, -(int)horizontal, 0);
    }

    public bool CanMoveToTile(Vector3Int moveCell)
    {
        Vector3Int nextTile = controller.currentTileIndex + moveCell * tileMove;
        //Detecta se o proximo tile que iria se movimentar é um tile de colisão, se for nao realiza o 

        return (gameManager.HasAvailableTile(nextTile) &&
                (Manager.Instance.gameManager.GetCreatureInTile(nextTile) == null));
    }

    public void Blink(Vector3Int index)
    {
        Vector3 pos = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(index) + offsetPosition;
        this.transform.position = pos;
        movePosition = pos;
        controller.currentTileIndex = index;
    }

}

