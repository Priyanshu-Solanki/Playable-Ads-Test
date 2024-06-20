using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenURL()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.PriyanshusCreations.ULTIMATEWORDLE");
    }
}
