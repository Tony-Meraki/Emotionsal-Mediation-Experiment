using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public static class Extensions
{
    //counterbalancing Random
    public static void Swap<T>(this List<T> list, int i, int j)
    {
        (list[i], list[j]) = (list[j], list[i]);
    }
}

public class ConditionManager : MonoBehaviour

{
    #region Variables
    [SerializeField] private float trialTime;
    [SerializeField] private float currentTaskInterruptInterval;
    [SerializeField] private int currentNumberOfKiosk;

    [Space(10)]
    [Header("Experiment Conditions")]
    [Header("Interrupt Interval")]
    [SerializeField] private List<float> Condition1Block;
    [Header("Assistance Method")]
    [SerializeField] private List<string> Condition2Block;
    [Header("Number of Kiosk")]
    [SerializeField] private int Condition3 = 8;

    [Space(10)]
    [Serialize] private List<(float, string)> conditionList; 
    

    [SerializeField] private string datasetPath;
    private int participantNumber;
    [SerializeField] private int currentTrialNumber;
    private string logSavePath;
    #endregion

    #region LambdaFunctions

    public float GetCurrentTaskInterruptionProbability() => currentTaskInterruptInterval;
    public float GetTrialTime() => trialTime;
    public int GetCurrentNumberOfKiosk() => currentNumberOfKiosk;
    public int GetCurrentTrialNumber() => currentTrialNumber;
    public int GetNumberOfTrial() => conditionList.Count;
    public float GetInterruptInterval() => Condition3;
    public string GetDatasetPath() => datasetPath;
    public string GetLogPath() => logSavePath;

    #endregion


    public void InitCondition()
    {
        currentTrialNumber = -1;

        conditionList = new List<(float, string)>();
        
        foreach (float condition1 in Condition1Block)
        {
            foreach (string condition2 in Condition2Block)
            {
                conditionList.Add((condition1, condition2));
            }
        }

        // Add Practice Trial
        conditionList.Insert(0, (15, "TrialScene_Base"));

        datasetPath = Application.dataPath + "/Resources/TaskDataset/";
    }   


    public void SetParticipantNumber(int num)
    {
        participantNumber = num;

        //변경된 Condition 종류에 맞춰서 수정 필요
        switch ((num - 1) % 6)
        {
            case 0: //summary, keyword, none
                break;
                
            case 1: // summary, none, keyword
                conditionList.Swap(2, 3);
                
                break;

            case 2: // keyword, summary, none
                conditionList.Swap(1, 2);
                break;

            case 3: // keyword, none, summary
                conditionList.Swap(1, 2);
                conditionList.Swap(2, 3);
                break;

            case 4: // none, summary, keyword
                conditionList.Swap(1, 3);
                conditionList.Swap(2, 3);
                break;

            case 5: // none, keyword, summary
                conditionList.Swap(1, 3);
                break;
            
            
        }

        List<string> conditionSequenceString = new List<string>();
        conditionSequenceString.Add("Trial Num,Condition 1,Condition2"+"\r\n");
        for (int i = 0; i < conditionList.Count; i++)
        {
            conditionSequenceString.Add(i.ToString() + "," + conditionList[i].Item1.ToString() + "," +
                                        conditionList[i].Item2.ToString()+"\r\n");
        }
        
        Directory.CreateDirectory(Application.dataPath + "/Logs/" + participantNumber.ToString());
        GameManager.GetGameManager().GetLoggingManager().WriteLog(Application.dataPath + "/Logs/" + participantNumber.ToString() + "/condition_sequence.csv", conditionSequenceString);
    }
    
    public string SetNextTrialCondition()
    {
        if (currentTrialNumber >= conditionList.Count - 1)
            return "";
        
        if (!GameManager.GetGameManager().isTrialScene)
        {
            LobbyManager.GetInstance().SetTrialNumText("Trial : " + (currentTrialNumber + 1).ToString());
        }
        
        currentTrialNumber++;
        currentTaskInterruptInterval = conditionList[currentTrialNumber].Item1;
        currentNumberOfKiosk = Condition3;
        
        logSavePath = Application.dataPath + "/Logs/" + participantNumber.ToString() + "/" + conditionList[currentTrialNumber].Item2 +"/";
        Directory.CreateDirectory(logSavePath);

        return conditionList[currentTrialNumber].Item2;
    }


    public bool SetTrialCondition(int changeValue)
    {
        if ((currentTrialNumber + changeValue > -2) && (currentTrialNumber + changeValue < conditionList.Count - 1))
        {
            currentTrialNumber = currentTrialNumber + changeValue;
            LobbyManager.GetInstance().SetTrialNumText("Trial : " + (currentTrialNumber + 1).ToString());
        }

        return true;
    }
    
    
    void Start()
    {
        InitCondition();
    }

}
