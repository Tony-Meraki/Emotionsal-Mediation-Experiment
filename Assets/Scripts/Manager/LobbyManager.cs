using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private static LobbyManager instance = null;

    public static LobbyManager GetInstance() => instance;
    // Start is called before the first frame update
    
    
    
    //UI Menu
    [SerializeField] private GameObject surveyEndUI;
    [SerializeField] private GameObject surveyRequestUI;
    [SerializeField] private GameObject participantNumUI;
    [SerializeField] private GameObject startButtonUI;
    [SerializeField] private TextMeshProUGUI trialNumUI;
    [SerializeField] private TextMeshProUGUI startButtonTextUI;

    public void SetTrialNumText(string trialText) => trialNumUI.text = trialText; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }


    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTrial()
    {
        int currentTrialCount = GameManager.GetGameManager().GetConditionManager().GetCurrentTrialNumber() + 1;
        int maxTrialCout = GameManager.GetGameManager().GetConditionManager().GetNumberOfTrial();
        if (currentTrialCount >= maxTrialCout)
        {
            trialNumUI.text = "실험이 종료되었습니다";
        }
        else
        {
            GameManager.GetGameManager().StartTrial();
        }
        
    }

    public void RequestToDoSurvey()
    {
        surveyRequestUI.SetActive(true);
    }

    public void InitLobby()
    {

        if (GameManager.GetGameManager().GetConditionManager().GetCurrentTrialNumber() >=
            GameManager.GetGameManager().GetConditionManager().GetNumberOfTrial() - 1)
        {
            surveyEndUI.SetActive(true);
            participantNumUI.SetActive(false);
            surveyRequestUI.SetActive(false);
            startButtonUI.SetActive(false);
            trialNumUI.text = "실험이 종료되었습니다";
        }
        else
        {
            surveyEndUI.SetActive(false);
            participantNumUI.SetActive(false);
            surveyRequestUI.SetActive(true);
            startButtonUI.SetActive(false);
            trialNumUI.text = "Trial : " +
                              (GameManager.GetGameManager().GetConditionManager().GetCurrentTrialNumber() + 1)
                              .ToString();
        }
        
    }
    
    
    
}
