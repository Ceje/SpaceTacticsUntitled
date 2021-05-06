using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : Obstacle
{
    public abstract void TriggerEffect(Interactable unit);
}
