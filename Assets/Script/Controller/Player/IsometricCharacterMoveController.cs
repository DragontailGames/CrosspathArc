using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricCharacterMoveController : MonoBehaviour
{
    public float movementSpeed = 1;

    private Rigidbody2D rb;

    public Tilemap tilemap;

    public Vector3 destination;

    //IsometricCharacterRenderer isometricCharacterRenderer;

    public bool keyboardMove = false;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        destination = rb.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);
            Vector3Int gridPos = tilemap.WorldToCell(pos);

            gridPos = new Vector3Int(gridPos.x, gridPos.y,0);

            if (tilemap.HasTile(gridPos) && !tilemap.GetTile(gridPos).name.Contains("Water"))
            {
                destination = tilemap.GetCellCenterWorld(gridPos);
            }
        }

        if (!keyboardMove)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, movementSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Collision")
        {
            destination = rb.position;
        }
    }

    void FixedUpdate()
    {
        Vector2 currentPos = rb.position;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 inputVector = new Vector2(horizontal, vertical);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);
        Vector2 movement = inputVector * movementSpeed;
        Vector2 newPos = currentPos + movement * Time.deltaTime;
        //isometricCharacterRenderer.SetDirection(movement);
        rb.MovePosition(newPos);

        if (horizontal != 0 || vertical != 0)
        {
            keyboardMove = true;
            destination = currentPos;
        }
        else
        {
            keyboardMove = false;
        }
    }
}
