using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OpenMenu : MonoBehaviour
{
    private GameObject menuHandle;
    public GameObject pauseMenu;

    public void Start(){
        Data.PauseButton = this;
    }
    
    public void OpenPauseMenu(){
        menuHandle = Instantiate(pauseMenu, transform.position, Quaternion.identity);
        menuHandle.GetComponent<PauseMenu>().parent = this;
    }

    public void ClosePauseMenu(){
        Destroy(menuHandle);
    }

    public void Quit(){
        Application.Quit();
    }

    public void NewGame(){
        SceneManager.LoadScene("MainScene");
    }

    public bool isOpen(){
        return menuHandle != null;
    }

    
}
