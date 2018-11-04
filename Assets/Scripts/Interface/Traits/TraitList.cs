using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TraitList : MonoBehaviour
{
    public List<Trait> traits = new List<Trait>();
    public List<TraitLine> traitLines = new List<TraitLine>();

    void Awake()
    {
        // Create TraitLineList
        traitLines.Add(new TraitLine("Sword", 0, 20));
        traitLines.Add(new TraitLine("Shield", 1));
        traitLines.Add(new TraitLine("Hunter", 2));
        traitLines.Add(new TraitLine("Heavy Armor", 3));
        traitLines.Add(new TraitLine("Fire Magic", 4));
        traitLines.Add(new TraitLine("Defence", 5));
        traitLines.Add(new TraitLine("Health", 6));
        traitLines.Add(new TraitLine("Controlle", 7));
        traitLines.Add(new TraitLine("Illusion", 8));
        traitLines.Add(new TraitLine("Leader", 9));
        traitLines.Add(new TraitLine("Black Magic", 10));
        traitLines.Add(new TraitLine("Diebeskunst", 11));
        traitLines.Add(new TraitLine("Runenherstellung", 12));
        traitLines.Add(new TraitLine("Alchemie", 13));
        traitLines.Add(new TraitLine("Schmieden", 14));
        traitLines.Add(new TraitLine("Bogen", 15));
        traitLines.Add(new TraitLine("Armbrust", 16));
        traitLines.Add(new TraitLine("Zweihand", 17));
        traitLines.Add(new TraitLine("Einhand", 18));


        //Create TraitList
        traits.Add(new Trait("Fireball", 0, "may cast fireballs", 0f, 1, 4));
        traits.Add(new Trait("Block", 1, "Block Attacks", 25f, 5, 0));
        traits.Add(new Trait("Row", 2, "hit multiple times in a row", 50f, 10, 0));
        traits.Add(new Trait("Block II", 3, "Heigher damage reduction", 10, traits[1]));
        traits.Add(new Trait("Hitspeed", 4, "less time between hits", 25, traits[2]));
        traits.Add(new Trait("Block III", 5, "less strumble", 50, traits[1]));
        traits.Add(new Trait("Combo", 6, "do Combo Hits", 75, traits[2]));
        traits.Add(new Trait("Parrieren", 7, "may conter attacks", 75f, 100, 0));


        //Add Skills to TraitLines

        for (int i = 0; i < traits.Count; i++)
        {
            traitLines[traits[i].line].lineTraits.Add(traits[i]);
        }
    }

    public List<TraitLine> GetTraitLineList()
    {
        return traitLines;
    }
}
