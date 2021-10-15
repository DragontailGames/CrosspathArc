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
                _instance = FindObjectOfType<Manager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = obj.AddComponent<Manager>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null || ReferenceEquals(this, _instance))
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public GameManager gameManager;

    public CanvasManager canvasManager;

    public EnemyManager enemyManager;

    public CharacterController characterController;

    public SceneLoadManager sceneLoadManager;

    public ConfigManager configManager;

    public TimeManager timeManager;

    public MouseTipsManager mouseTipsManager;
}
