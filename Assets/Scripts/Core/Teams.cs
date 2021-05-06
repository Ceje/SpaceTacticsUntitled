using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teams
{
    
    private static TeamManager _teamManager;

    public static TeamManager GetManagerInstance(){
        if (_teamManager is null){
            _teamManager = new TeamManager();
        }
        return _teamManager;
    }

    public class TeamManager
    {
        public class Team
        {
            private bool _ai;
            private string _name;
            private HashSet<Interactable> _members;

            public Team(string name, bool ai, HashSet<Interactable> members){
                _name = name;
                _ai = ai;
                _members = members;
            }


            public void Add(Interactable unit){
                _members.Add(unit);
            }

            public void Remove(Interactable unit){
                _members.Remove(unit);
            }

            public bool Contains(Interactable unit){
                return _members.Contains(unit);
            }

            public string GetName(){
                return _name;
            }

            public HashSet<Interactable> GetMembers(){
                return _members;
            }

            public bool IsAI(){
                return _ai;
            }
        }
        private Dictionary<String, Team> _teams;
        private List<String> _teamOrder;

        public TeamManager(){
            _teams = new Dictionary<string, Team>();
            _teamOrder = new List<String>();
        }

        public Team GetTeam(String name, bool AITeam = false){
            CreateIfAbsent(name, AITeam);
            return _teams[name];
        }

        private void CreateIfAbsent(string name, bool AITeam){
            if (!_teams.ContainsKey(name)){
                _teams[name] = new Team(name, AITeam, new HashSet<Interactable>());
                _teamOrder.Add(name);
                
            }
        }

        public void AddToTeam(String team, Interactable unit){
            CreateIfAbsent(team, false);
            _teams[team].Add(unit);
        }

        public void AddToAITeam(String team, Interactable unit){
            CreateIfAbsent(team, true);
            _teams[team].Add(unit);
        }

        public void RemoveFromTeam(String team, Interactable unit){
            CreateIfAbsent(team, false);
            _teams[team].Remove(unit);
            MusicManager.UpdateBgm();

            if (_teams[team].GetMembers().Count == 0){
                _teams.Remove(team);
                CheckForVictory();
            }
        }

        private void CheckForVictory(){
            if (!_teams.ContainsKey("_Nano")){
                SceneManager.LoadScene("VictoryScene");
            }
        }

        public HashSet<Team> GetUnitTeams(Interactable unit){
            HashSet<Team> teams = new HashSet<Team>();
            foreach (var keyValuePair in _teams){
                if (keyValuePair.Value.Contains(unit)){
                    teams.Add(keyValuePair.Value);
                }
            }

            return teams;
        }

        public bool ShareTeams(Interactable unitA, Interactable unitB){
            var sharedTeams = GetUnitTeams(unitA);
            sharedTeams.IntersectWith(GetUnitTeams(unitB));
            return sharedTeams.Count > 0;
        }

        public void SetTeamOrder(List<Team> teamOrder){
            var newOrder = new List<String>();
            teamOrder.ForEach(team => {
                if (_teams.ContainsKey(team.GetName())){
                    newOrder.Add(team.GetName());
                }
            });
            foreach (var team in _teams.Values){
                if (!newOrder.Contains(team.GetName())){
                    newOrder.Add(team.GetName());
                }
            }

            _teamOrder = newOrder;
        }

        public List<string> GetTeamOrder(){
            return _teamOrder;
        }
    }
}