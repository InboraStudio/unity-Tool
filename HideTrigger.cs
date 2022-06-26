using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideTrigger : MonoBehaviour
{
    public GameObject EnterText;
     
     void OnTriggerEnter(Collider other) 
    {
        EnterText.GetComponent<Text>().text = "Your Text";   
    }

     void OnTriggerExit(Collider other) 
    {
        EnterText.GetComponent<Text>().text = "";  
    }
        
    
}
