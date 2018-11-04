using UnityEngine;
using System.Collections;

public class Relations : MonoBehaviour {

    [SerializeField]
    private Relation [,] fractionRelations;

    
	// Use this for initialization
	void Awake () 
    {
        int fractionCount = System.Enum.GetValues(typeof(Faction)).Length;
        fractionRelations = new Relation[fractionCount, fractionCount];
        for (int i = 0; i < fractionCount; i++)
        {
            // set relation to ... 
            for (int j = 0; j < fractionCount; j++)
            {
                // ... himself
                if(i == j)
                {
                    fractionRelations[i, j] = Relation.Friendly;
                }
                // ... monsters
                else if((Faction)i == Faction.Monster || (Faction)j == Faction.Monster)
                {
                    fractionRelations[i, j] = Relation.Enemy;
                }
                // ... everything else
                else
                {
                    fractionRelations[i, j] = Relation.Neutral;
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Relation GetFractionRelation(GameObject asking,GameObject opponent)
    {
        // Debug.Log(asking + ", " + opponent);
        int one = (int)asking.GetComponent<PlayerAttributes>().GetMembership;
        int two = (int)opponent.GetComponent<PlayerAttributes>().GetMembership;
        Relation relation = fractionRelations[one, two];
        
        // Debug.Log(relation);
        return relation;
    }

    public bool IsEnemy(GameObject asking, GameObject opponent)
    {
        // fractions are hostile to each other
        return true;
        // personal relations are bad
        return true;
        // they are opponents in the arena
        return true;
    }
}

public enum Faction
{
    Fraction1,
    Fraction2,
    Monster,
    PotentialFollower
}

public enum Relation
{
    Friendly,
    Neutral,
    Enemy
}
