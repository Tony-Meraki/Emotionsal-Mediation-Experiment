using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class OperatorManager : MonoBehaviour
{
    protected static OperatorManager instance = null;
    public static OperatorManager GetInstance() => instance;

    [SerializeField] protected OperatorUIManager operatorUIManager;
    public OperatorUIManager GetOperatorUIManager() => operatorUIManager;
    [SerializeField] protected GameObject kioskPrefab;
    [SerializeField] protected AudioSource audioSource;

    [SerializeField]
    protected List<KioskController> kioskControllers = new List<KioskController>();
    protected List<int> kioskInterruptCallSequence = new List<int>();
    protected int currentSelectedKioskNumber = 0;
    protected ConditionManager conditionManager;

    public ConditionManager GetConditionManager() => conditionManager;

    protected float trailStartTime;
    protected float trialEndTime;

    protected int numberOfWaitingKiosk;


    protected float interruptInterval;
    protected float targetInterruptCheckTime;

    
    [SerializeField] protected string questionDataPath;
    [SerializeField] protected List<int> questionSequence = new List<int>();
    protected string[] questionList;
    
    [SerializeField] protected string videoDataPath;
    [SerializeField] protected List<int> videoSequence = new List<int>();
    protected string[] videoList;
    [SerializeField] protected int questionIndex = 0;
    [SerializeField] protected int videoIdx = 0;

    public void AddInvokedKiosk()
    {
        numberOfWaitingKiosk++;
        audioSource.Play();
    }

    public int GetNumberOfWaitingKiosk() => numberOfWaitingKiosk;

    protected string logPath;
    protected StreamWriter logger;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time > trialEndTime)
        {
            EndTrial();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            EndTrial();
        }
        InturruptGenerator();
    }

    protected virtual void InturruptGenerator()
    {
        
        if (Time.time < targetInterruptCheckTime)
            return;
        Debug.Log("Inturrupt Message");
        kioskInterruptCallSequence.Sort((a, b) => Random.Range(0f, 1f) > 0.5f ? 1 : -1);
        foreach (int sequence in kioskInterruptCallSequence)
        {
            
            
            //에러. 이거뜨면 멈춤 썅
            if (kioskControllers[sequence].GetInterruptGenerator()
                .MakeInterrupt(questionList[questionSequence[questionIndex]], 
                    videoList[videoSequence[videoIdx]]))
            {
                questionIndex++;
                videoIdx++;
                targetInterruptCheckTime = Time.time + interruptInterval;
                Debug.Log("Invoked : " + sequence.ToString());
                break;
            }
        }
    }

    protected void EndTrial()
    {
        WriteTrialLog();
        foreach (KioskController controller in kioskControllers)
        {
            List<string> controllerData = controller.EndTrial();
        }
        
        StartCoroutine(waitLogging());
    }

    protected void WriteTrialLog()
    {
        List<string> logData = new List<string>();
        
        logPath = conditionManager.GetLogPath() + "TrialLog.csv";
        
        logData.Add(
            "KioskNumber, QuestionNumber,Invoked Time,Is Checked,Checked Time,Number of Waiting Kiosk,Answerd Time,Used Time,QuestionType,Question,Correct Answer,User Answer,Is Right Answer" +
            "\r\n");
        
        foreach (KioskController controller in kioskControllers)
        {
            List<string> kioskLogData = controller.GetLoggingData();
            for (int i = 1; i < kioskLogData.Count; i++)
            {
                logData.Add(kioskLogData[i]);
            }
        }
        GameManager.GetGameManager().GetLoggingManager().WriteLog(logPath, logData);
    }

    IEnumerator waitLogging()
    {
        yield return new WaitForEndOfFrame();
        GameManager.GetGameManager().EndTrial();
    }

    public void InitTrial(ConditionManager _conditionManager, float condition1Value,
        int condition2Value, float condition3Value)
    {
        conditionManager = _conditionManager;
        trailStartTime = Time.time;
        trialEndTime = Time.time + conditionManager.GetTrialTime();
        logPath = conditionManager.GetLogPath() + "TrialState.csv";

        //현재 Trial State Log 저장
        List<string> logDatas = new List<string>();
        logDatas.Add(
            "Trial Number,Condition 1,Condition 2,Start Time,End Time" +
            "\r\n");
        logDatas.Add(
            conditionManager.GetCurrentTrialNumber().ToString() + "," + condition1Value.ToString() + "," +
            condition2Value.ToString() + "," +
            trailStartTime.ToString() + "," + trialEndTime.ToString() + "," +
            "\r\n");
        GameManager.GetGameManager().GetLoggingManager().WriteLog(logPath, logDatas);

        //Trial 초기화
        numberOfWaitingKiosk = 0;
        operatorUIManager.SetTrialConditionText("Control HASS - " + conditionManager.GetCurrentTrialNumber());

        //Kiosk 생성 및 초기화
        for (int i = 0; i < condition2Value; i++)
        {
            GameObject newKiosk = Instantiate(kioskPrefab);
            KioskController newKioskController = newKiosk.GetComponent<KioskController>();
            KioskInterruptGenerator newKioskInterruptGenerator = newKiosk.GetComponent<KioskInterruptGenerator>();
            
            newKioskController.InitKioskController((i+1), (i+1).ToString(), newKioskInterruptGenerator, this, trailStartTime);
            newKioskInterruptGenerator.InitInterruptor(newKioskController);
            operatorUIManager.AddKiosk(newKiosk);
            kioskControllers.Add(newKioskController);
        }
        
        //Kiosk Interrupt Interval 설정
        interruptInterval = condition1Value; 
        targetInterruptCheckTime = Time.time + interruptInterval;

        //Kiosk 호출 순서 List 저장
        for (int i = 0; i < kioskControllers.Count; i++)
        {
            kioskInterruptCallSequence.Add(i);
        }
        
        //Question Sequence Init
        string text = File.ReadAllText(conditionManager.GetDatasetPath() + questionDataPath);
        questionList = text.Split("\r\n");
        
        questionSequence.Clear();
        for (int i = 0; i < questionList.Length; i++)
        {
            questionSequence.Add(i);
        }
        questionSequence.Sort((a, b) => Random.Range(0f, 1f) > 0.5f ? 1 : -1);
        questionIndex = 0;
        
        //Video Sequence Init
        videoList = Directory.GetFiles(conditionManager.GetDatasetPath() + videoDataPath, "*.mp4");
        videoSequence.Clear();
        for (int i = 0; i < videoList.Length; i++)
        {
            videoSequence.Add(i);
        }
        videoSequence.Sort((a, b) => Random.Range(0f, 1f) > 0.5f ? 1 : -1);
        videoIdx = 0;
        
        InitAdditionalData();
    }

    protected virtual void InitAdditionalData()
    {
        
    }

    public void ShowUPKioskMessage(int selectedKioskNumber, string kioskNamge)
    {
        if (currentSelectedKioskNumber > 0)
        {
            kioskControllers[currentSelectedKioskNumber-1].UnSelectKiosk();
        }

        currentSelectedKioskNumber = selectedKioskNumber;
        operatorUIManager.SetKioskChat(kioskNamge);
    }
    
    public void ShowUPKioskMessage(int selectedKioskNumber, string kioskNamge, string kioskMessage, string additionalMessage = "")
    {
        if (currentSelectedKioskNumber > 0)
        {
            kioskControllers[currentSelectedKioskNumber-1].UnSelectKiosk();
        }

        currentSelectedKioskNumber = selectedKioskNumber;
        operatorUIManager.SetKioskChat(kioskNamge, kioskMessage, additionalMessage);
    }

    public void ShowUPKioskVideo(int selectedKioskNumber, string kioskNamge)
    {
        operatorUIManager.SetKioskVideo();
    }
    

    public void ShowUPKioskVideo(int selectedKioskNumber, string kioskNamge, string kioskVideoPath)
    {
        operatorUIManager.SetKioskVideo(kioskVideoPath);
    }

    public void SendMessageToKiosk(string sendMessage)
    {
        if (currentSelectedKioskNumber > 0)
        {
            if (kioskControllers[currentSelectedKioskNumber - 1].AnswerARequest(sendMessage))
            {
                numberOfWaitingKiosk--;
            }
        }
    }
    
    public void SendMessageToKiosk(int kioskNum, string sendMessage)
    {
        if (kioskControllers[kioskNum - 1].AnswerARequest(sendMessage))
        {
            numberOfWaitingKiosk--;
        }
    }
    
    
}
