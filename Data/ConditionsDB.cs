using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId; 
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions {get;set;} = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Psn,
            new Condition()
            {
                Name = "Poisoned",
                StartMessage = "has been poisoned!",
                //Lambda Functon
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by poison");
                }
            }
        }
        ,
        {
            ConditionID.Brn,
            new Condition()
            {
                Name = "Burned",
                StartMessage = "has been burned!",
                //Lambda Functon
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHp/16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is hurt by burn");
                }
            }
        }
        ,
        {
            ConditionID.Par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed!",
                //Lambda Functon
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is unable to move due to paralysis");
                        return false;
                    }
                    return true;
                }
            }
        }
        ,

        {
            ConditionID.Frz,
            new Condition()
            {
                Name = "Frozen",
                StartMessage = "has been frozen solid!",
                //Lambda Functon
                OnBeforeMove = (Pokemon pokemon) =>
                {
                    if(Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} has thawed out");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is frozen solid");
                    return false;
                }
            }
        }
        ,
        {
            ConditionID.Slp,
            new Condition()
            {
                Name = "Sleeping",
                StartMessage = "has fallen asleep",
                //Lambda Functon
                OnStart = (Pokemon pokemon) =>
                {
                    //Pokemon Should Sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"{pokemon.Base.Name} will be asleep for {pokemon.StatusTime} turns");
                },
                //Lambda Functon
                OnBeforeMove = (Pokemon pokemon) =>
                {

                    if(pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is fast asleep");
                    return false;
                }
            }
        }
        ,//Volatile Status Conditions
        {
            ConditionID.Confusion,
            new Condition()
            {
                Name = "Confused",
                StartMessage = "became confused!",
                //Lambda Functon
                OnStart = (Pokemon pokemon) =>
                {
                    //Pokemon Should Confuse for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"{pokemon.Base.Name} will be confused for {pokemon.VolatileStatusTime} turns");
                },
                //Lambda Functon
                OnBeforeMove = (Pokemon pokemon) =>
                {

                    if(pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} snapped out of confusion");
                        return true;
                    }
                    pokemon.VolatileStatusTime--;

                    //50% chance to do a move
                    if(Random.Range(1,3) == 1)
                        return true;
                    
                    //Hurst by confusion
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"It hurt itself due to confusion");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    None, Psn, Brn, Slp, Par, Frz,Confusion,Infatuation,
}
