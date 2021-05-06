using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Data
{


    public static GlobalGrid board;
    public static FogMap fog;
    public static UnitTracker UnitTracker;
    public static Inspector Inspector;
    public static List<Item> LooseItems = new List<Item>();
    public static List<Item> AttachedItems = new List<Item>();
    public static List<HazardZone> HazardZones = new List<HazardZone>();
    private static AIController _ai;
    

    public static ItemEntry SelectedItem;
    public static EndTurnButton EndTurnButton;

    public static OpenMenu PauseButton;
}

