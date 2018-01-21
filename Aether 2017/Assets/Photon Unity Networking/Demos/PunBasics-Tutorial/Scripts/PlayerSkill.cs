using UnityEngine;
using System.Collections;

public class PlayerSkill {

    private string skillName;
    private SkillType skillType;
    private float effectMultiplier;
    private string skillClass;
    private int index;
    private float cooldown;
    private float cooldownStart = 0f;
    private bool canCastOnSelf;
    private bool requiresTarget;
    private float skillEffectDuration;
    private bool areaOfEffect;

    // TODO animation

    public PlayerSkill(string name, string skillClass, 
                       SkillType skillType, float effectMultiplier, 
                       float cooldown, float skillEffectDuration, bool areaOfEffect)
    {

        this.skillClass = skillClass;
        this.skillName = name;
        this.skillType = skillType;
        this.effectMultiplier = effectMultiplier;
        this.cooldown = cooldown;
        this.skillEffectDuration = skillEffectDuration;
        this.areaOfEffect = areaOfEffect;
    }

    public string getSkillClass()
    {
        return this.skillClass;
    }

    public SkillType getSkillType()
    {
        return this.skillType;
    }

    public string getSkillName()
    {
        return this.skillName;
    }

    public void show(GameObject user, GameObject target)
    {
        // TODO paint this skill, using this skill's this.animation or something
    }

    public float getEffectMultiplier()
    {
        return this.effectMultiplier;
    }
    public float getCooldown()
    {
        return cooldown;
    }
    public float getCooldownStart()
    {
        return cooldownStart;
    }
    public void setCooldownStart(float newCooldownStart)
    {
        cooldownStart = newCooldownStart;
    }
    public float getSkillEffectDuration()
    {
        return skillEffectDuration;
    }
    public bool getAOE()
    {
        return areaOfEffect;
    }
}