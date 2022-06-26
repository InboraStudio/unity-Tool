using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene  : MonoBehaviour
{
 void OnTriggerEnter(Collider other) 
 
 {
     SceneManager.LoadScene(0);         
 }
}
