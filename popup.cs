using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popup : MonoBehaviour
{
  public GameObject GameObject;
  bool active = true;

  public void OpenPlanl()

  {
     if (active == false)
     {
        gameObject.transform.gameObject.SetActive(false);
        active = false;
     }
     else
     {
        gameObject.transform.gameObject.SetActive(true);
        active = true ;
     }
  }
}
