using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Pokemon", menuName = "Pokemon/Create New Pokemon")]

public class PokemonBase : ScriptableObject
{
    [SerializeField] new string name;
    
    [TextArea]
    [SerializeField] string description;
    
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMoves> learnableMoves;
    public string Name { get { return name; } }
    public string Description { get { return description; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public PokemonType Type1 { get { return type1; } }
    public PokemonType Type2 { get { return type2; } }
    public int MaxHP { get { return maxHp; } }
    public int Attack { get { return attack; } }
    public int Defence { get { return defence; } }
    public int SpAttack { get { return spAttack; } }
    public int SpDefence { get { return spDefence; } }
    public int Speed { get { return speed; } }
    public List<LearnableMoves> LearnableMoves { get { return learnableMoves; } }
}
[System.Serializable]
public class LearnableMoves
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack, Defence, SpAttack,SpDefence, Speed, 
    //These two are not actual stats, they are used to calculate moveAccuracy
    Accuracy, Evasion
}
public class TypeChart
{
    static float[][] chart =
    {                       //NOR   FIR  WAT ELE   GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
        /*NOR*/ new float[] {  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,0.5f,  0f,  1f,  1f,0.5f,  1f},//Done
        /*FIR*/ new float[] {  1f,0.5f,0.5f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f,0.5f,  1f,0.5f,  1f,  2f,  1f},//Done
        /*WAT*/ new float[] {  1f,  2f,0.5f,  1f,0.5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f,0.5f,  1f,  1f,  1f},//Done
        /*ELE*/ new float[] {  1f,  1f,  2f,0.5f,0.5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f,0.5f,  1f,  1f,  1f},//Done
        /*GRA*/ new float[] {  1f,0.5f,  2f,  1f,0.5f,  1f,  1f,0.5f,  2f,0.5f,  1f,0.5f,  2f,  1f,0.5f,  1f,0.5f,  1f},//Done
        /*ICE*/ new float[] {  1f,0.5f,0.5f,  1f,  2f,0.5f,  1f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  2f,  1f,0.5f,  1f},//Done
        /*FIG*/ new float[] {  2f,  1f,  1f,  1f,  1f,  2f,  1f,0.5f,  1f,0.5f,0.5f,0.5f,  2f,  0f,  1f,  2f,  2f,0.5f},//Done
        /*POI*/ new float[] {  1f,  1f,  1f,  1f,  2f,  1f,  1f,0.5f,0.5f,  1f,  1f,  1f,0.5f,0.5f,  1f,  1f,  0f,  2f},//Done
        /*GRO*/ new float[] {  1f,  2f,  1f,  2f,0.5f,  1f,  1f,  2f,  1f,  0f,  1f,0.5f,  2f,  1f,  1f,  1f,  2f,  1f},//Done
        /*FLY*/ new float[] {  1f,  1f,  1f,0.5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,  2f,0.5f,  1f,  1f,  1f,0.5f,  1f},//Done
        /*PSY*/ new float[] {  1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f,  1f,  1f,0.5f,  1f,  1f,  1f,  1f,  0f,0.5f,  1f},//Done
        /*BUG*/ new float[] {  1f,0.5f,  1f,  1f,  2f,  1f,0.5f,0.5f,  1f,0.5f,  2f,  1f,  1f,0.5f,  1f,  2f,0.5f,0.5f},//Done
        /*ROC*/ new float[] {  1f,  2f,  1f,  1f,  1f,  2f,0.5f,  1f,0.5f,  2f,  1f,  2f,  1f,  1f,  1f,  1f,0.5f,  1f},//Done
        /*GHO*/ new float[] {  0f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f,0.5f,  1f,  1f},//Done
        /*DRA*/ new float[] {  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,0.5f,  0f},//Done
        /*DAR*/ new float[] {  1f,  1f,  1f,  1f,  1f,  1f,0.5f,  1f,  1f,  1f,  2f,  1f,  1f,  2f,  1f,0.5f,  1f,0.5f},//Done
        /*STE*/ new float[] {  1f,0.5f,0.5f,0.5f,  1f,  2f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,0.5f,0.5f},//Done
        /*FAI*/ new float[] {  1f,0.5f,  1f,  1f,  1f,  1f,  2f,0.5f,  1f,  1f,  1f,  1f,  1f,  1f,  2f,  2f,0.5f,  1f},//Done
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenceType)
    {
        if(attackType == PokemonType.None || defenceType == PokemonType.None)
            return 1f;

        int row = (int)attackType - 1;
        int col = (int)defenceType - 1;

        return chart[row][col];
    }
}
