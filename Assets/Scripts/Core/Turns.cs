using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turns
{
    private static Teams.TeamManager.Team _currentTeam;

    public static void NextTurn(){
        var teamManager = Teams.GetManagerInstance();
        var teamOrder = teamManager.GetTeamOrder();
        if (_currentTeam is null){
            _currentTeam = teamManager.GetTeam(teamOrder[0]);
        }
        else{
            var teamIndex = teamOrder.IndexOf(_currentTeam.GetName());
            teamIndex++;
            if (teamIndex > teamOrder.Count - 1){
                teamIndex = 0;
            }
            
            var nextTeam = teamManager.GetTeam(teamOrder[teamIndex]);
            if (_currentTeam == null){
                Debug.Log("Starting Turn of " + nextTeam.GetName());
            }
            else{
                Debug.Log(_currentTeam.GetName() + "'s Turn Ending. Starting Turn of " + nextTeam.GetName());
                
            }

            _currentTeam = nextTeam;
        }

//        Debug.Log("Starting turn for " + _currentTeam.GetName() + "; Team AI: " + _currentTeam.IsAI());
        ActivateTeam(_currentTeam);
    }

    private static void ActivateTeam(Teams.TeamManager.Team nextTeam){
        Data.EndTurnButton.Configure(nextTeam);
        FogMap.SetVisionProviders(nextTeam.GetMembers());
        Data.UnitTracker.ClearUnit();
        foreach (var unit in nextTeam.GetMembers()){
            (unit as Unit)?.RefreshUnit();
        }
        
    }

    public static Teams.TeamManager.Team GetCurrentTeam(){
        if (_currentTeam is null){
            NextTurn();
        }

        return _currentTeam;
    }
}