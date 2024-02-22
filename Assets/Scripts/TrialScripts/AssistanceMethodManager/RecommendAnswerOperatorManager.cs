using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecommendAnswerOperatorManager : OperatorManager
{
    [SerializeField] private string recommandDataPath;

    [SerializeField] protected string[] recommendList;

    protected override void InturruptGenerator()
    {
        if (Time.time < targetInterruptCheckTime)
            return;
        Debug.Log("Inturrupt Message");
        kioskInterruptCallSequence.Sort((a, b) => Random.Range(0f, 1f) > 0.5f ? 1 : -1);
        
        foreach (int sequence in kioskInterruptCallSequence)
        {
            if (kioskControllers[sequence].GetInterruptGenerator()
                .MakeInterrupt(questionList[questionSequence[questionIndex]], 
                    videoList[videoSequence[videoIdx]], recommendList[questionSequence[questionIndex]]))
            {
                questionIndex = (questionIndex + 1) % questionList.Length;
                videoIdx = (videoIdx + 1) % videoList.Length;
                targetInterruptCheckTime = Time.time + interruptInterval;
                Debug.Log("Invoked : " + sequence.ToString());
                break;
            }
        }
    }

    protected override void InitAdditionalData()
    {
        //Summary Data Init
        string recommendText = File.ReadAllText(conditionManager.GetDatasetPath() + recommandDataPath);
        recommendList = recommendText.Split("\r\n");
    }
}