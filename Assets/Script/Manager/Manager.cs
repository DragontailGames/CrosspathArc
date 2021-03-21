using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton do manager generico para comunicação entre os componentes
/// </summary>
public class Manager : MonoBehaviour
{
    #region singleton
    private static Manager _instance;

    public static Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Manager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public GameManager gameManager;

    public CanvasManager canvasManager;

    public EnemyManager enemyManager;

    public CharacterController characterController;
}
