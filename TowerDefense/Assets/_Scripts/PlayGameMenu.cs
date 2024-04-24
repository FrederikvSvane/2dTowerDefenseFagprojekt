using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameMenu : MonoBehaviour
{
    public void PlaySinglePlayer(){
        //SceneManager.LoadScene("SinglePlayer");
        Debug.Log("Press Play Single Player");
    }

    public void PlayMultiPlayer(){
        SceneManager.LoadScene("LoadingScene");
    }
}
