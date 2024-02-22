using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class KioskInterruptGenerator : MonoBehaviour
{

    private KioskController kioskController;
    
    private bool isWating = true;

    private string questionText, answerText, videoPath;
    
    public bool MakeInterrupt(string _questionText, string _videoPath, string _additionalText = "")
    {
        if (isWating)
            return false;
        isWating = true;
        answerText = "";
        questionText = _questionText;
        videoPath = _videoPath;
        kioskController.InvokeInterrupt(_questionText, _videoPath, _additionalText);
        return true;
    }

    public void InitInterruptor(KioskController kioskC)
    {
        kioskController = kioskC;
        isWating = false;
    }

    public string AnswerARequest(string message)
    {
        //Message 정답 체크 코드 추가하기
        isWating = false;
        return answerText;
    }
    
    
    
}
