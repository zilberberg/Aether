using UnityEngine;
using System.Collections;
using System;

public class ProjectileManager : MonoBehaviour {

    //public SkillManager skillMGR;
    private string skillName;
    private float effectMultiplier;
    private float playerPower;
    private SkillType skillType;
    private float skillEffectDuration;
    private float critChance;
    private float critDamage;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void setProjectileValues(string pSkillName, float pPower, float eMultiplier,
                                    SkillType skillType, float skillEffectDuration, 
                                    float critChance, float critDamage)
    {
        this.skillName = pSkillName;
        this.playerPower = pPower;
        this.effectMultiplier = eMultiplier;
        this.skillEffectDuration = skillEffectDuration;
        this.critChance = critChance;
        this.critDamage = critDamage;
    }
    public string getSkillName()
    {
        return skillName;
    }

    public float getEffectMultiplier()
    {
        return effectMultiplier;
    }
    public float getPlayerPower()
    {
        return playerPower;
    }
    public SkillType getSkillType()
    {
        return skillType;
    }

    public float getCritDamage()
    {        
        return critDamage;
    }

    public float getSkillEffectDuration()
    {
        return skillEffectDuration;
    }

    public float getCritChance()
    {
        return critChance;
    }


}
