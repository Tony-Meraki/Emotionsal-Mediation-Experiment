using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SummaryOperatorManager : OperatorManager
{
    [SerializeField] private string summaryDataPath;

    protected string[] summaryList;

    protected override void InturruptGenerator()
    {
        if (Time.time < targetInterruptCheckTime)
            return;
        Debug.Log("Inturrupt Message");
        kioskInterruptCallSequence.Sort((a, b) => Random.Range(0f, 1f) > 0.5f ? 1 : -1); // kioskInterruptCallSequence 리스트의 순서를 무작위로 재정렬한다.
        
        Debug.Log("Sequence Length : " + kioskInterruptCallSequence.Count);
        Debug.Log("Controllers : " + kioskControllers.Count);
        foreach (int sequence in kioskInterruptCallSequence)
        {
            //Debug.Log("current sequence : " + sequence + ", question: " + questionIndex + ", video : " + videoIdx);
            // 에러이 쫑간나 얘도 뜨면 멈춤
            if (kioskControllers[sequence].GetInterruptGenerator()
                .MakeInterrupt(questionList[questionSequence[questionIndex]], 
                    videoList[videoSequence[videoIdx]], summaryList[questionSequence[questionIndex]]))
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
        string summaryText = File.ReadAllText(conditionManager.GetDatasetPath() + summaryDataPath);
        summaryList = summaryText.Split("\r\n");
    }
}