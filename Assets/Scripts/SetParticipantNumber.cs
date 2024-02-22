using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetParticipantNumber : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantNumberInput;
    

    public void SerParticipantNum()
    {
        if (participantNumberInput.text != "")
        {
            GameManager.GetGameManager().GetConditionManager().SetParticipantNumber(int.Parse(participantNumberInput.text));
        }
    }



}
