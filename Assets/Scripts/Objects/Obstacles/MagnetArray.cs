using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetArray : Obstacle
{
    public List<MagnetPanel> MagnetPanels;

    public override void Start(){
        base.Start();
        MagnetPanels = new List<MagnetPanel>(GetComponentsInChildren<MagnetPanel>());
    }
}
