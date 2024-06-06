using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScenes  : MonoBehaviour
{
 void OnTriggerEnter(Collider other) 
 
 {
     SceneManager.LoadScene(0);         
 }
}
