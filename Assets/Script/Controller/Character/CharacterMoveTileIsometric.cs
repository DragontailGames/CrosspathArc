using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class CharacterMoveTileIsometric : MonoBehaviour
{
    private CharacterController characterController;
    public GameManager gameManager;

    public float movementSpeed = 1;
    public int tileMove = 1;

    public Vector3 offsetPosition;
    private Vector3Int currentTileIndex;
    private Vector3 movePosition;
    Vector3Int mouse = Vector3Int.zero;

    public Vector3Int Mouse { get => this.mouse; set => this.mouse = value; }
    public Vector3Int CurrentTileIndex { get => this.currentTileIndex; set => this.currentTileIndex = value; }

    //Resets iniciais
    public void Start()
    {
        characterController = this.GetComponent<CharacterController>();

        CurrentTileIndex = gameManager.tilemap.WorldToCell(this.transform.position);

        movePosition = gameManager.tilemap.GetCellCenterWorld(CurrentTileIndex) + offsetPosition;
    }

    public void FixedUpdate()
    {
        Vector3Int moveCell = Vector3Int.zero;

        if (!gameManager.InPause && characterController.myTurn && characterController.Animator.GetBool("Walk") == false)//Testa o delay para correção da movimentação por tile
        {
            Vector3Int keyboard = GetMoveCellKeyboard();

            moveCell = Mouse != Vector3Int.zero ? Mouse : keyboard;//Detecta se deve usar o valor do teclado ou do mouse

            if (moveCell != Vector3Int.zero)
            {
                gameManager.EndMyTurn(characterController);
            }

            if (moveCell != Vector3Int.zero)
            {
                Mouse = Vector3Int.zero;

                if (CanMoveToTile(moveCell))
                {
                    characterController.direction = characterController.GetDirection(moveCell);
                    if(characterController.direction != "W" && characterController.direction != "E")
                    {
                        movementSpeed = 1.5f;
                        characterController.Animator.speed = 0.8f;
                    }
                    else
                    {
                        movementSpeed = 2.0f;
                        characterController.Animator.speed = 0.7f;
                    }

                    if (!characterController.Animator.GetBool("Walk"))
                    {
                        PlayAnimation(characterController.animationName + "_Walk_" + characterController.direction);
                        characterController.Animator.SetBool("Walk", true);
                    }
                    CurrentTileIndex += moveCell * tileMove;
                    movePosition = gameManager.tilemap.GetCellCenterWorld(CurrentTileIndex) + offsetPosition;
                    this.GetComponent<CharacterStatus>().MoveOneTile();
                }
            }
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);

        characterController.Animator.SetBool("Walk", Vector3.Distance(this.transform.position, movePosition) > 0.05);
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
        Vector3Int nextTile = CurrentTileIndex + moveCell * tileMove;
        //Detecta se o proximo tile que iria se movimentar é um tile de colisão, se for nao realiza o 

        return (!gameManager.elevationTM.HasTile(nextTile + new Vector3Int(1,1,0))) &&
                (gameManager.tilemap.HasTile(nextTile)) &&
                (Manager.Instance.enemyManager.CheckEnemyInTile(nextTile) == null);
    }

    public void PlayAnimation(string animation)
    {
        Debug.Log("walk");
        characterController.Animator.Play(animation);
    }
}

