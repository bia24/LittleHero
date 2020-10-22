using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGModule;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(OnDiaLogueClick);
    }

   private void  OnDiaLogueClick()
    {
        Debug.Log("click");
    }
}
