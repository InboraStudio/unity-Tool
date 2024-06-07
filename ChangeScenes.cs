using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
//xaimo
public class ChangeScenes  : MonoBehaviour
{
 void OnTriggerEnter(Collider other) 
 
 {
     SceneManager.LoadScene(0);         
 }
}
