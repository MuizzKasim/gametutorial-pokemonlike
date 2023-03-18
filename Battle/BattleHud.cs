using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Text statusText;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;
    [SerializeField] Color slpColor;

    Dictionary<ConditionID, Color> statusColors;

    Pokemon _pokemon;
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.Psn, psnColor },
             {ConditionID.Brn, brnColor },
              {ConditionID.Par, parColor },
               {ConditionID.Frz, frzColor },
                {ConditionID.Slp, slpColor },
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
    }
     
    void SetStatusText() 
    { 
        if(_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {

            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }
}
