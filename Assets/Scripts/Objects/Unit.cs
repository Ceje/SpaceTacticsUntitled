using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Unit : Obstacle, IInventory, FogMap.IVisionProvider
{
    public int health;
    public int vision;
    public int peripheralVision;
    public float facing; //in degrees
    public int maxMovement;
    public int actionLimit;
    public string team;
    public float moveSpeed;

    

    public int remainingMovement;
    public int remainingActions;

    protected Animator _animator;
    private static readonly int Walking = Animator.StringToHash("Walking");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    protected static readonly int Hurt = Animator.StringToHash("Hurt");

    public HashSet<Action> actions = new HashSet<Action>();
    public List<Ailment> ailments = new List<Ailment>();

    protected AudioSource _audioSource;
    public AudioClip _walkingSound;
    
    protected new void Start(){
        base.Start();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        RefreshActions();
        
        interactName = name;

        RegisterToTeam();
        RefreshUnit();
    }

    public void SpendActions(int cost){
        remainingActions -= cost;
    }
    public void AdjustFacing(Vector3 nextPoint){
        Vector3 difference = nextPoint - transform.position;
        facing = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        FogMap.ScheduleFogUpdate();
    }
    public void AdjustFacing(float nextPoint){
        facing = nextPoint;
        FogMap.ScheduleFogUpdate();
    }
    public static void DeselectUnit(){
        InputHandler.CancelAction();
        InputHandler.ClearActionMode();
        Data.UnitTracker.ClearUnit();
    }

    public void RefreshUnit(){
        ailments.ForEach(ailment => {
            ailment.Recovery(this);
        });
        
        ailments = ailments.FindAll(ailment => ailment.Stacks > 0);

        if (ailments.Any(ailment => ailment.GetType() == typeof(Stunned))){
            return;
        }
        
        remainingMovement = maxMovement;
        remainingActions = actionLimit;
    }

    public void Attack(){
        _animator.SetTrigger(Attack1);
    }

    public void StartMove(){
        _animator.SetBool(Walking, true);
        _audioSource.clip = _walkingSound;
        _audioSource.Play();
    }

    public void EndMove(){
        _animator.SetBool(Walking, false);
        _audioSource.Stop();
    }

    public virtual void Damaged(int damage){
//        health -= damage;
        _animator.SetTrigger(Hurt);
    }

    public void CheckDeath(){
        if (health <= 0){
            Destroy(gameObject);
        }
    }

    public bool IsBusy(){
        var busy = !_animator.GetCurrentAnimatorStateInfo(0).IsTag("idle");
        return busy;
    }

    public override void OnDestroy(){
        if (!gameObject.scene.isLoaded){
            return; 
        }
        
        base.OnDestroy();

        foreach (var unitTeam in Teams.GetManagerInstance().GetUnitTeams(this)){
            Teams.GetManagerInstance().RemoveFromTeam(unitTeam.GetName(), this);
        }

        if (UnitTracker.IsActiveUnit(this)){
            DeselectUnit();
        }
    }

    public int AssessPriority(int potentialDamage){
        return (int) Math.Ceiling((maxMovement + actionLimit) * ((float)potentialDamage / health));
    }

    protected virtual void RegisterToTeam(){
        Teams.GetManagerInstance().AddToTeam(team, this);
        Teams.GetManagerInstance().SetTeamOrder(new List<Teams.TeamManager.Team>{Teams.GetManagerInstance().GetUnitTeams(this).First()});
    }

    
    
    //Inventory methods
    public Dictionary<Item, int> Items{ get; set; } = new Dictionary<Item, int>();
    public int MaxItems{ get; set; } = 10;

    public void AddItem(Item item){
        if (Items.Count >= MaxItems){
            return;
        }
        
        Transform transform1 = item.transform;
        transform1.SetParent(transform);
        item.InInventory = true;
        Items[item] = CountItem(item);
    }

    public void RemoveItem(Item item){
        if (!Items.ContainsKey(item)){
            return;
        }
        Items[item] = CountItem(item);
        if (Items[item] <= 0){
            Items.Remove(item);
        }
    }

    public int CountItem(Item item){
        int count = 0;
        foreach (var childItem in GetComponentsInChildren<Item>()){
            if (childItem.GetType() == item.GetType() && item.InInventory){
                count++;
            }
        }

        return count;
    }
    
    //Damage avoidance protocol
    public Vector3Int? RespondToTargeted(){
        var propulsions = GetComponents<Propulsion>();
        if (propulsions.Length > 0){
            var movingTowards = propulsions[0].GetTargetLocation();
            foreach (var propulsion in propulsions){
                propulsion.Interrupt();
            }
            return movingTowards;
        }
        return null;
    }

    public void RefreshActions(){
        actions = new HashSet<Action>(GetComponentsInChildren<Action>());
    }
    
    //Expose vision parameters
    public int GetPeripheralVision(){
        return peripheralVision;
    }

    public int GetForwardVision(){
        return vision;
    }

    public float GetFacing(){
        return facing;
    }

    public Transform GetTransform(){
        return transform;
    }
    
}