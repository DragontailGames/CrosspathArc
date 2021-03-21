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

    private void Awake()
    {
        Manager.Instance.gameManager = this;
    }

    public Transform particleClick;

    private bool inPause;

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
}
