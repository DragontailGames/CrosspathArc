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
    private bool canMove = true;
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
        if (canMove && !gameManager.InPause)//Testa o delay para correção da movimentação por tile
        {
            Vector3Int keyboard = GetMoveCellKeyboard();

            moveCell = Mouse != Vector3Int.zero ? Mouse : keyboard;//Detecta se deve usar o valor do teclado ou do mouse

            if (moveCell != Vector3Int.zero)
            {
                Mouse = Vector3Int.zero;

                if (CanMoveToTile(moveCell))
                {
                    characterController.direction = characterController.GetDirection(moveCell);
                    PlayAnimation(characterController.animationName + "_Walk_" + characterController.direction);
                    CurrentTileIndex += moveCell * tileMove;
                    movePosition = gameManager.tilemap.GetCellCenterWorld(CurrentTileIndex) + offsetPosition;
                    StartCoroutine(DelayMove(characterController.direction == "W" || characterController.direction == "E"));//Inicia o delay do movimento
                }
            }
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, movePosition, movementSpeed * Time.deltaTime);

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

    /// <summary>
    /// Delay na movimentação do usuario, apenas para simular melhor o movimento por tiles
    /// </summary>
    public IEnumerator DelayMove(bool sides)
    {
        this.GetComponent<CharacterStatus>().MoveOneTile();

        canMove = false;

        yield return new WaitForSeconds(sides ? 0.4f:0.2f);
        characterController.Animator.SetBool("Walk", false);

        canMove = true;
    }

    public bool CanMoveToTile(Vector3Int moveCell)
    {
        Vector3Int nextTile = CurrentTileIndex + moveCell * tileMove;
        //Detecta se o proximo tile que iria se movimentar é um tile de colisão, se for nao realiza o movimento

        return (gameManager.collisionTM.GetTile(nextTile) == null) &&
                (gameManager.tilemap.GetTile(nextTile) != null) &&
                (Manager.Instance.enemyManager.CheckEnemyInTile(nextTile) == null);
    }

    public void PlayAnimation(string animation)
    {
        characterController.Animator.Play(animation);
        characterController.Animator.SetBool("Walk", true);
    }
}

