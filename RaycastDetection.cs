//Inbora studio (clam all copyrights)2020-22 All scripts are code by Inbora Studio.
//Devloped By Alok Khokhar for more information follow as on instagram @inbora.studio or ower webside. 
//https://inborastudio.wixsite.com/inborastudio

using UnityEngine;
using UnityEngine.UI;
 
public class RaycastDetection : MonoBehaviour
{
    Ray ray;
    RaycastHit raycastHit;
    Text textUI;
 
    void Awake()
    {
        textUI = GameObject.FindObjectOfType<Text>();
    }
 
    void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out raycastHit))
        {
            textUI.text = raycastHit.collider.gameObject.name;
        }
        else
        {
            textUI.text = "";
        }
    }
}
