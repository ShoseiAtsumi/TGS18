using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class select_button : MonoBehaviour {

    public void Click_stage1()
    {
        SceneManager.LoadScene("Main");
    }
    public void Click_stage2()
    {
        SceneManager.LoadScene("stage2");
    }
    public void Click_stage3()
    {
        SceneManager.LoadScene("stage3");
    }

}
