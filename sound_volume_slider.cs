using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sound_volume_slider : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
          PlayerPrefs.SetFloat("musicVolume", 1);
          Load();
        }


        else
        {
           Load();
        }
    }

     public void ChangeVolume()
     {
         AudioListener.volume = volumeSlider.value;
         save();
     }

     private void Load()
     {
       volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
     }

     private void save()
     {
       PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
     }
   }
