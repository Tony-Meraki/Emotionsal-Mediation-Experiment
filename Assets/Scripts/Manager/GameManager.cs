using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager GetGameManager() => instance;

    [SerializeField] private ConditionManager conditionManager;
    public ConditionManager GetConditionManager() => conditionManager;
    [SerializeField] private LoggingManager loggingManager;
    [SerializeField] private AudioSource audioSource;
    public LoggingManager GetLoggingManager() => loggingManager;

    private OperatorManager operatorManager = null;
    private LobbyManager lobbyManager = null;

    public bool isTrialScene = false;
    
    

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                GameObject.Destroy(gameObject);
            }
            
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (isTrialScene)
        {
            lobbyManager = null;
            operatorManager = OperatorManager.GetInstance();
            operatorManager.InitTrial(conditionManager,
                conditionManager.GetCurrentTaskInterruptionProbability(), conditionManager.GetCurrentNumberOfKiosk(),
                conditionManager.GetInterruptInterval());
        }
        else
        {
            operatorManager = null;
            lobbyManager = LobbyManager.GetInstance();
            lobbyManager.InitLobby();
            audioSource.Play();
        }
    }

    public void StartTrial()
    {
        string nextConditionScene = conditionManager.SetNextTrialCondition(); 
        if (nextConditionScene != "")
        {
            isTrialScene = true;
            SceneManager.LoadScene(nextConditionScene); //TrialScene
        }
    }

    public void EndTrial()
    {
        isTrialScene = false;
        SceneManager.LoadScene("Lobby");
    }

}