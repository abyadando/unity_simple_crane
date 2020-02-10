using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendCommand : MonoBehaviour
{

    public arm Arm;
    public Text Input;

    public void Send()
    {
        StartCoroutine(Arm.ApplyCommand(Input.text));
    }
}
