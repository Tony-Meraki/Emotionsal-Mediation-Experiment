using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class OperatorUIManager : MonoBehaviour
{

    [SerializeField] private Transform kioskParents;
    [SerializeField] private TextMeshProUGUI tiralConditionText;
    [SerializeField] private TextMeshProUGUI chatKioskName, chatKioskText, chatOperatorText;
    [SerializeField] private TMP_InputField operatorInputText;
    [SerializeField] private List<TextMeshProUGUI> additionalTextList;
    [SerializeField] private GameObject chatKioskObject, chatOperatorObject;

    [SerializeField] private GameObject userChatArea, kioskChatArea;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Scrollbar chatScrollBar;

    [SerializeField] private VideoPlayer videoPlayer;

    //230408
    //public ScrollRect scrollRect;

    public void SetTrialConditionText(string conditionText) => tiralConditionText.text = conditionText;


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (operatorInputText.text != "")
            {
                SendOperatorMessage();         
            }
        }
    }

    public void AddKiosk(GameObject newKiosk)
    {
        newKiosk.transform.SetParent(kioskParents);
    }

    //230408
    //void ScrollToBottomOfView()
    //{
    //    // verticalNormalizedPosition 속성을 0으로 설정하여 스크롤 뷰를 가장 아래로 이동시킵니다.
    //    scrollRect.verticalNormalizedPosition = 0f;
    //}

    public void SetKioskChat(string kioskNamge)
    {

        var childs = contentRect.GetComponentInChildren<Transform>();
        foreach (Transform child in childs)
        {
            if (child != contentRect.transform)
            {
                Destroy(child.gameObject);
            }
        }

        chatKioskName.text = kioskNamge;


        /*
        chatKioskObject.SetActive(false);
        chatOperatorObject.SetActive(false);
        */
        operatorInputText.text = "";
        foreach (var textUI in additionalTextList)
        {
            textUI.text = "";
        }
    }

    public void SetKioskChat(string kioskNamge, string kioskMessage, string additionalMessage = "")
    {
        var childs = contentRect.GetComponentInChildren<Transform>();
        foreach (Transform child in childs)
        {
            if (child != contentRect.transform)
            {
                Destroy(child.gameObject);
            }
        }

        AddChat(true, "안녕하세요");

        string[] kioskChats = kioskMessage.Split("##");
        bool isUserChat = false;
        foreach (string chatText in kioskChats)
        {
            AddChat(isUserChat, chatText);
            isUserChat = !isUserChat;
        }

        chatKioskName.text = kioskNamge;
        AddChat(true, "관리자님의 개입이 필요합니다");
        /*
        chatKioskText.text = kioskMessage;
        chatKioskObject.SetActive(true);
        chatOperatorObject.SetActive(false);
        */
        operatorInputText.text = "";

        //230408 스크롤 뷰를 가장 아래로 이동
        //ScrollToBottomOfView();

        if (additionalMessage != "")
        {
            string[] additionalMessages = additionalMessage.Split("##");
            for (int i = 0; i < additionalMessages.Length; i++)
            {
                additionalTextList[i].text = additionalMessages[i];
            }
        }
        
    }

    public void SetKioskVideo()
    {
        videoPlayer.gameObject.SetActive(false);
        videoPlayer.Stop();
    }

    public void SetKioskVideo(string kioskVideoPath)
    {
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.url = kioskVideoPath;
        videoPlayer.Play();
    }



    public void SendOperatorMessage()
    {
        string inputText = operatorInputText.text;
        SetKioskVideo();
        Invoke("ScrollDelay", 0.05f);
        AddChat(true, inputText);
        //대화답변 여러개 받아서 저장하는 부분으로 수정필요
        OperatorManager.GetInstance().SendMessageToKiosk(inputText);

        operatorInputText.text = "";
    }
    
    
    public void SendOperatorMessage(string userAnswer)
    {
        SetKioskVideo();
        Invoke("ScrollDelay", 0.05f);
        AddChat(true, userAnswer);
        //대화답변 여러개 받아서 저장하는 부분으로 수정필요
        OperatorManager.GetInstance().SendMessageToKiosk(userAnswer);

        operatorInputText.text = "";
    }

    void ScrollDelay() => chatScrollBar.value = 0f;

    void Fit(RectTransform rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

    public void AddChat(bool isUserChat, string text)
    {
        bool isBottom = chatScrollBar.value <= 0.0001f;

        ChatTextSetter chat = Instantiate(isUserChat ? userChatArea : kioskChatArea).GetComponent<ChatTextSetter>();
        chat.transform.SetParent(contentRect.transform, false);

        float boxSizeX = MathF.Min(50 + (text.Length * 20), 650);
        chat.boxRect.sizeDelta = new Vector2(boxSizeX, chat.boxRect.sizeDelta.y);
        chat.userText.text = text;
        Fit(chat.boxRect);
        
        Invoke("ScrollDelay", 0.05f);
    }
}
