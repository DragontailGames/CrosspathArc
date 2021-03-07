using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform particleClick;

    private bool inPause;

    public bool InPause { get => this.inPause; set => this.inPause = value; }

    public void MoveParticle(Vector2 position)
    {
        particleClick.transform.position = position;
        particleClick.GetComponent<ParticleSystem>().Play();
    }
}
