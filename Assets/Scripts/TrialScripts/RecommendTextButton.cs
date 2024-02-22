using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecommendTextButton : MonoBehaviour
{
    private OperatorUIManager operatorUIManager;
    [SerializeField] private TextMeshProUGUI recommendText;
    // Start is called before the first frame update
    void Start()
    {
        operatorUIManager = OperatorManager.GetInstance().GetOperatorUIManager();
    }

    public void RecommendButtonClicked()
    {
        operatorUIManager.SendOperatorMessage(recommendText.text);
    }
}
