using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class IsometricCharacterMoveTile : MonoBehaviour
{

    [Tooltip("Velocidade do movimento para ambos os tipos")]
    public float movementSpeed = 1;

    [Tooltip("Tiles de movimento")]
    public int tileMove = 1;

    public Tilemap tilemap;

    public Tilemap collisionTM;

    public Vector3 offsetPosition;

    public GameManager gameManager;

    private bool canMove = true;

    private Vector3Int currentTileIndex;

    private Vector3 movePosition;

    Vector3Int mouse = Vector3Int.zero;

    //Resets iniciais
    public void Start()
    {
        currentTileIndex = tilemap.WorldToCell(this.transform.position);

        movePosition = tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameManager.InPause)//Detecta o click do jogador
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            mouse = MouseMove();
        }
    }

    public void FixedUpdate()
    {
        Vector3Int moveCell = Vector3Int.zero;
        if (canMove && !gameManager.InPause)//Testa o delay para correção da movimentação por tile
        {
            Vector3Int keyboard = GetMoveCellKeyboard();

            moveCell = mouse != Vector3Int.zero ? mouse: keyboard;//Detecta se deve usar o valor do teclado ou do mouse

            if (moveCell != Vector3Int.zero)
            {
                mouse = Vector3Int.zero;

                if (collisionTM.GetTile(currentTileIndex + moveCell * tileMove) == null && tilemap.GetTile(currentTileIndex + moveCell * tileMove) != null)//Detecta se o proximo tile que iria se movimentar é um tile de colisão, se for nao realiza o movimento
                {
                    currentTileIndex += moveCell * tileMove;
                    movePosition = tilemap.GetCellCenterWorld(currentTileIndex) + offsetPosition;
                    StartCoroutine(DelayMove());//Inicia o delay do movimento
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
    /// Função que retorna a posição do mouse corrigida
    /// </summary>
    public Vector3Int MouseMove()
    {
        Vector2 pos = Input.mousePosition;//Detecta a posição do mouse
        pos = Camera.main.ScreenToWorldPoint(pos);//Converte a posição com relação a camera
        Vector3Int gridPos = tilemap.WorldToCell(pos);//pega o index do tile que foi clicado
        gridPos -= currentTileIndex;

        gameManager.MoveParticle(pos);//Efeito para identificar o click
        return MathfCustom.Sign(gridPos);//Retorna o sinal do valor(-1,+1 ou 0)
    }

    /// <summary>
    /// Delay na movimentação do usuario, apenas para simular melhor o movimento por tiles
    /// </summary>
    public IEnumerator DelayMove()
    {
        this.GetComponent<CharacterStatus>().MoveOneTile();

        canMove = false;

        yield return new WaitForSeconds(0.2f);

        canMove = true;
    }
}
