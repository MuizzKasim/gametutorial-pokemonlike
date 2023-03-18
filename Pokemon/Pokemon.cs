using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base
    {
        get
        {
            return _base;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
    }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    public void Init()
    {
        Moves = new List<Move>();

        //Generate Moves based on Level
        foreach(var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
                
            if (Moves.Count >= 4)
                break;
        }
        CalculateStats();
        HP = MaxHp;
        ResetStatBoost();
        CureStatus();
        CureVolatileStatus();
    }

    void CalculateStats()//Performs Base Stat Calcs once for the entire battle, improves efficiency
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefence, Mathf.FloorToInt((Base.SpDefence * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level;
    }
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defence, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefence, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 }
        };
    }
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f,1.5f,2f,2.5f,3f,3.5f,4f };

        if(boost >= 0)
        {
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        }
        else
        {
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        }

        return statVal;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int MaxHp
    {
        get; private set;
    }
    public int Attack{
    get { return GetStat(Stat.Attack); }
    }
    public int Defence
    {
        get { return GetStat(Stat.Defence); }
    } 
    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int SpDefence
    {
        get { return GetStat(Stat.SpDefence); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (UnityEngine.Random.value * 100f < 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            Type = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defence = (move.Base.Category == MoveCategory.Special) ? SpDefence : Defence;

        float modifiers = UnityEngine.Random.Range(0.85f,1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defence) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage,0,MaxHp);
        HpChanged = true;
    }
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null)
        {
            StatusChanges.Enqueue($"But {Base.Name} is already {Status.Name}...");
            return;
        }

        Status =  ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);//null conditional operator to prevent crash if status does not have OnStart method
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");

        OnStatusChanged?.Invoke();
    }
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null)
        {
            StatusChanges.Enqueue($"But {Base.Name} is already {VolatileStatus.Name}...");
            return;
        }

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);//null conditional operator to prevent crash if volatile status does not have OnStart method
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }
    public Move GetRandomMove()
    {
        int r = UnityEngine.Random.Range(0, Moves.Count);
        return Moves[r];
    }
    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        //.Invoke is not used since it can return null. Null is not a valid value for bool return type.
        if(Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;   
        }
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }
        return canPerformMove;
    }
    public void OnAfterTurn()
    {
        //Will only run this code if the expression is not Null
        //Status such as SLEEP, CONFUSION, FREEZE do not have on after turn effect hence it will return null
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);

    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float Type { get; set; }
}
