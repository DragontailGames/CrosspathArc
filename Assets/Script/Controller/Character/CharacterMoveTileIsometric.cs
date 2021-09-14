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
    private Vector3 movePosition;
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

        if (!gameManager.InPause && controller.animator.GetBool("Walk") == false)//Testa o delay para correção da movimentação por tile
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
                        controller.animator.speed = 0.8f;
                    }
                    else
                    {
                        movementSpeed = 2.0f;
                        controller.animator.speed = 0.7f;
                    }

                    if (!controller.animator.GetBool("Walk"))
                    {
                        PlayAnimation(controller.animationName + "_Walk_" + controller.direction);
                        controller.animator.SetBool("Walk", true);
                    }
                    controller.currentTileIndex += moveCell * tileMove;
                    movePosition = gameManager.tilemap.GetCellCenterWorld(controller.currentTileIndex) + offsetPosition;
                    this.GetComponent<CharacterStatus>().MoveOneTile();
                }
            }
            else
            {
                controller.myTurn = true;
            }
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);

        controller.animator.SetBool("Walk", Vector3.Distance(this.transform.position, movePosition) > 0.05);
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

    public void PlayAnimation(string animation)
    {
        controller.animator.Play(animation);
    }

    public void Blink(Vector3Int index)
    {
        Vector3 pos = Manager.Instance.gameManager.tilemap.GetCellCenterWorld(index) + offsetPosition;
        this.transform.position = pos;
        movePosition = pos;
        controller.currentTileIndex = index;
    }

}

