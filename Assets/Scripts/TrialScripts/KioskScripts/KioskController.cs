using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class KioskController : MonoBehaviour
{

    [SerializeField] [ColorUsage(true)] private Color idealColor;

    [SerializeField] [ColorUsage(true)] private Color selectedColor;

    [SerializeField] [ColorUsage(true)] private Color interruptedColor;

    [SerializeField] private Image kioskUI;
    [SerializeField] private TextMeshProUGUI kioskNameUI;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoPlayerUI;
    [SerializeField] private RenderTexture baseRenderTexture;
    
    [SerializeField] private string kioskName;
    
    private OperatorManager operatorManager;
    private KioskInterruptGenerator interruptor;
    public KioskInterruptGenerator GetInterruptGenerator() => interruptor;

    private int kioskNumber;

    private bool isInterrupted = false;
    private bool isSelected = false;
    private string lastMessage;
    
    
    //LoggingDatas
    //private StreamWriter logger;

    private float trialStartTime;
    
    private string logPath;
    private int requestedQustionNumber;
    private float questionInvokedTime;
    private bool isChecked;
    private float questionCheckedTime;
    private int numberOfWaitingKiosk;
    private float answeredTime;
    private float usedTime;
    private string questionType;
    private string question;
    private string correctAnswer;
    private string userAnswer;
    private string additionalMessage;

    private List<string> logData;
    
    public List<string> GetLoggingData() => logData;

    private string videoPath;
    
    public void InitKioskController(int _kioskNumber, string _kioskNamge, KioskInterruptGenerator _interruptor,
        OperatorManager _operatorManager, float _trialStartTime)
    {
        isInterrupted = false;
        kioskNumber = _kioskNumber;
        kioskName = _kioskNamge;
        interruptor = _interruptor;
        operatorManager = _operatorManager;
        kioskUI.color = idealColor;
        kioskNameUI.text = kioskName;
        lastMessage = "";
        trialStartTime = _trialStartTime;

        logData = new List<string>();
        logPath = operatorManager.GetConditionManager().GetLogPath() + kioskName + ".csv";
        logData.Add(
            "KioskNumber, QuestionNumber,Invoked Time,Is Checked,Checked Time,Number of Waiting Kiosk,Answerd Time,Used Time,QuestionType,Question,Correct Answer,User Answer,Is Right Answer" +
            "\r\n");
        /*
        logger = new StreamWriter(operatorManager.GetConditionManager().GetLogPath() + kioskName + ".csv");
        logger.Write(
            "QuestionNumber,Invoked Time,Is Checked,Checked Time,Number of Waiting Kiosk,Answerd Time,Used Time,QuestionType,Question,Correct Answer,User Answer,Is Right Answer" +
            "\r\n");
            */
        requestedQustionNumber = 0;
    }

    public void InvokeInterrupt(string interruptMessage)
    {
        //string[] interruptMessages = interruptMessage.Split(' ');
        questionType = " "; 
        kioskUI.color = interruptedColor;
        question = lastMessage = interruptMessage;
        isInterrupted = true;

        requestedQustionNumber++;
        questionInvokedTime = Time.time - trialStartTime;
        isChecked = false;
        numberOfWaitingKiosk = -1;
        operatorManager.AddInvokedKiosk();
        StartCoroutine(CheckNumberOfWaiting());

    }
    
    
    public void InvokeInterrupt(string interruptMessage, string _videoPath, string _additionalMessage)
    {
        //string[] interruptMessages = interruptMessage.Split(' ');
        questionType = " "; 
        kioskUI.color = interruptedColor;
        question = lastMessage = interruptMessage;
        isInterrupted = true;

        requestedQustionNumber++;
        questionInvokedTime = Time.time - trialStartTime;
        isChecked = false;
        numberOfWaitingKiosk = -1;
        operatorManager.AddInvokedKiosk();
        videoPath = _videoPath;
        additionalMessage = _additionalMessage;
        
        StartCoroutine(CheckNumberOfWaiting());

    }

    IEnumerator CheckNumberOfWaiting()
    {
        yield return new WaitForEndOfFrame();
        numberOfWaitingKiosk = operatorManager.GetNumberOfWaitingKiosk();
    }

    //�̺κ� �߿� �ٽ� ���������!!! �ٽ� �����ų��ϸ� ������ ��ȭ �ʱ�ȭ �ɼ�����
    public void SelectKiosk()
    {
        isSelected = true;
        if (lastMessage != "")
        {
            operatorManager.ShowUPKioskMessage(kioskNumber, kioskName, lastMessage, additionalMessage);
            operatorManager.ShowUPKioskVideo(kioskNumber, kioskName, videoPath);
            
            
            if (isInterrupted)
            {
                questionCheckedTime = Time.time - trialStartTime;
                isChecked = true;
            }
        }
        else
        {
            operatorManager.ShowUPKioskMessage(kioskNumber, kioskName);
            operatorManager.ShowUPKioskVideo(kioskNumber, kioskName);
        }
        kioskUI.color = selectedColor;
    }

    public void UnSelectKiosk()
    {
        isSelected = false;
        if (isInterrupted)
        {
            kioskUI.color = interruptedColor;
        }
        else
        {
            kioskUI.color = idealColor;
        }
    }

    public bool AnswerARequest(string replyMessage)
    {
        if (isInterrupted)
        {
            isInterrupted = false;
            if (isSelected)
            {
                kioskUI.color = selectedColor;
            }
            else
            {
                kioskUI.color = idealColor;
            }

            lastMessage = "";

            correctAnswer = interruptor.AnswerARequest(replyMessage);
            
            
            answeredTime = Time.time - trialStartTime;
            usedTime = answeredTime - questionInvokedTime;
            userAnswer = replyMessage;

            bool isRightAnswer = string.Compare(correctAnswer, userAnswer) == 0 ? true : false;

            Debug.Log(numberOfWaitingKiosk);
            if (numberOfWaitingKiosk >= 0)
            {
                /*
                logger.Write(
                    requestedQustionNumber.ToString() +","+ questionInvokedTime.ToString() +","+ isChecked.ToString() +","+ questionCheckedTime.ToString() +","+ 
                    numberOfWaitingKiosk.ToString() +","+ answeredTime.ToString() +","+ usedTime.ToString() +","+ questionType +","+ question +","+
                    correctAnswer +","+ userAnswer +","+ isRightAnswer.ToString() +","+
                    "\r\n");
                    */

                string newLogData = kioskNumber.ToString() + "," + requestedQustionNumber.ToString() + "," +
                                    questionInvokedTime.ToString() + "," + isChecked.ToString() + "," +
                                    questionCheckedTime.ToString() + "," +
                                    numberOfWaitingKiosk.ToString() + "," + answeredTime.ToString() + "," +
                                    usedTime.ToString() + "," + questionType + "," + question + "," +
                                    correctAnswer + "," + userAnswer + "," + isRightAnswer.ToString() + "," +
                                    "\r\n";
                logData.Add(newLogData);
                Debug.Log(requestedQustionNumber.ToString() +","+ questionInvokedTime.ToString() +","+ isChecked.ToString() +","+ questionCheckedTime.ToString() +","+ 
                          numberOfWaitingKiosk.ToString() +","+ answeredTime.ToString() +","+ usedTime.ToString() +","+ questionType +","+ question +","+
                          correctAnswer +","+ userAnswer +","+ isRightAnswer.ToString() +",");
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<string> EndTrial()
    {
        if (isInterrupted)
        {
            AnswerARequest("");
        }
        
        GameManager.GetGameManager().GetLoggingManager().WriteLog(logPath, logData);

        return logData;
    }



}
