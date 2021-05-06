using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public OpenMenu parent;
    
    public void closeMenu(){
        parent.ClosePauseMenu();
    }

    public void closeGame(){
        Application.Quit();
    }

    public void newGame(){
        parent.NewGame();
    }
}
