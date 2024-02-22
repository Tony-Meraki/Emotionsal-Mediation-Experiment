using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatTextSetter : MonoBehaviour
{
    [SerializeField] public RectTransform areaRect, boxRect, textRect;
    [SerializeField] public TextMeshProUGUI userText;
    private string time, user;
}
