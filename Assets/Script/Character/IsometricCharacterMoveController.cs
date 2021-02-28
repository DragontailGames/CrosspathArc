using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricCharacterMoveController : MonoBehaviour
{
    [Tooltip("Velocidade do movimento para ambos os tipos")]
    public float movementSpeed = 1;

    public Tilemap tilemap;

    public Vector3 offsetPosition;

    public GameManager gameManager;

    private Vector3 destination;

    private Rigidbody2D rb;

    private bool keyboardMove = false;

    void Start()
    {
        //Referencia do rigidbody e reset da posição
        rb = this.GetComponent<Rigidbody2D>();
        destination = rb.position;
    }
    private void Update()
    {
        MouseMove();
    }

    /// <summary>
    /// Detecta a colisão do player
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Collision")//Verifica se o jogador colidiu com um objeto com a tag "Collision" definida no objeto colision dos tilemaps
        {
            destination = rb.position;//Cancela a movimentação do jogador caso ele tenha colidido
        }
    }

    void FixedUpdate()
    {
        KeyboardMove();

        if (!keyboardMove)//Testa se o teclado não esta sendo apertado para mover o jogador pelo click do mouse
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, movementSpeed * Time.deltaTime);//Movimenta o jogador para o destino que foi definido no click
        }
    }

    /// <summary>
    /// Função responsavel pela movimentação do jogador pelo teclado
    /// </summary>
    private void KeyboardMove()
    {
        Vector2 currentPos = rb.position;
        float horizontal = Input.GetAxisRaw("Horizontal");//Recebe o input do jogador no eixo Horizontal(A/D,Seta para esquerda/Seta para direita)
        float vertical = Input.GetAxisRaw("Vertical");//Recebe o input do jogador no eixo Vertical(W/S,Seta para cima/Seta para baixo)
        Vector2 inputVector = new Vector2(horizontal, vertical);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);//Corrige velocidade de movimentação na diagonal
        Vector2 movement = inputVector * movementSpeed;
        Vector2 newPos = currentPos + movement * Time.deltaTime;
        rb.MovePosition(newPos);//Move o player

        if (horizontal != 0 || vertical != 0)//Detecta quando o jogador estiver usando o teclado
        {
            keyboardMove = true;
            destination = currentPos;
        }
        else
        {
            keyboardMove = false;
        }
    }

    private void MouseMove()
    {
        //Detecta se o botao esquerdo do mouse foi precionado
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;//Detecta a posição do mouse
            pos = Camera.main.ScreenToWorldPoint(pos);//Converte a posição com relação a camera
            Vector3Int gridPos = tilemap.WorldToCell(pos);//pega o index do tile que foi clicado

            gridPos = new Vector3Int(gridPos.x, gridPos.y, 0);//Correção do movimento em z

            if (tilemap.HasTile(gridPos))//Verifica se o tile clicado existe
            {
                destination = tilemap.GetCellCenterWorld(gridPos) + offsetPosition;//Define a posição com referencia ao tile que foi clicado
                gameManager.MoveParticle(pos);
            }
        }
    }
}
