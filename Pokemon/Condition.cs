using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name { get; set; }
    public string Descripption { get; set; }
    public string StartMessage { get; set; }

    public ConditionID Id { get; set; }
    public Action<Pokemon> OnStart { get; set; }
    //Func can reference method with return value
    public Func<Pokemon, bool> OnBeforeMove { get; set; }
    //Action cannot reference a method with a return value
    public Action<Pokemon> OnAfterTurn { get; set; }
}
