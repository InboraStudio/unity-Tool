using System.Collections;
using System.Collections.Generic;
using UnityEngine;
Using inputsystm;

public class Animation_Triggers : MonoBehaviour
{
  [SerializeField] private Animator myAnimationController;

  private void OnTriggerEnter (Collider other)
  {
    if (other.CompareTag("Player"))
     {                               //ADD YOUR PARAMETERS THERE TO PLAY ANIMATION
       myAnimationController. SetBool("YOUR PARAMETERS", true); 
     } 
  }    
      
   private void OnTriggerExit (Collider other)
  {
       if (other.CompareTag("Player"))
        { 
         myAnimationController.SetBool("playSpin2", false);
        }
  }      
}
