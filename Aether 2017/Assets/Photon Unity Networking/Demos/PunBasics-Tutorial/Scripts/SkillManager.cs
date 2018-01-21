using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Demos.DemoAnimator;

public class SkillManager : MonoBehaviour
{
    public string checkClassURL = null;    
    public string SecureKey = null;

    private string userID;
    private Dictionary<string, PlayerSkill> allSkills = new Dictionary<string, PlayerSkill>();
    private List<PlayerSkill> chooseableSkills = new List<PlayerSkill>();
    private string playerClass;
    private List<string> ourSkills = new List<string>();

    

    //private Dictionary<int, Skill> ourSkills = new Dictionary<int, Skill>();

    void Start()
    {
        userID = PlayerPrefs.GetString("userID").ToString();
        if (!PlayerPrefs.HasKey("Class"))
        {
            StartCoroutine(CheckClass());
            
        } else
        {
            this.playerClass = PlayerPrefs.GetString("Class");
        }
        
        
        addSkill("Gamma Ray", new PlayerSkill("Gamma Ray", "DPS", SkillType.DAMAGING, 2f , 2f, 1f, false));
        addSkill("EMP", new PlayerSkill("EMP", "DPS", SkillType.DAMAGING, 3f, 4f, 1f, false));
        addSkill("Supernova", new PlayerSkill("Supernova", "DPS", SkillType.DAMAGING, 4f, 6f, 1f, false));
        addSkill("Radiation", new PlayerSkill("Radiation", "DPS", SkillType.DAMAGING, 5f, 10f, 1f, false));
        addSkill("Speed Boost", new PlayerSkill("Speed Boost", "DPS", SkillType.BOOSTSPD, 2f, 5f, 15f, true));
        addSkill("Solar Beam", new PlayerSkill("Solar Beam", "healer", SkillType.DAMAGING, 1f, 2f, 1f, false));
        addSkill("Solar Wind", new PlayerSkill("Solar Wind", "healer", SkillType.HEALING, 2f, 5f, 1f, true));
        addSkill("Particles Blast", new PlayerSkill("Particles Blast", "healer", SkillType.HEALING, 3f, 10f, 1f, true));
        addSkill("Ionization", new PlayerSkill("Ionization", "healer", SkillType.DAMAGINGORHEALING, 2f, 3f, 1f, false));
        addSkill("Energy Boost", new PlayerSkill("Energy Boost", "healer", SkillType.BOOSTENG, 2f, 3f, 15f, true));
        addSkill("Strike", new PlayerSkill("Strike", "tank", SkillType.DAMAGING, 2f, 1f, 1f, false));
        addSkill("Taunt", new PlayerSkill("Taunt", "tank", SkillType.OTHER, 2f, 3f, 1f, false));
        addSkill("Earthquake", new PlayerSkill("Earthquake", "tank", SkillType.DAMAGING, 3f, 3f, 1f, false));
        addSkill("Diamond Skin", new PlayerSkill("Diamond Skin", "tank", SkillType.BOOSTDEF, 2f, 3f, 15f, true));
        addSkill("HP Boost", new PlayerSkill("HP Boost", "tank", SkillType.BOOSTHP, 2f, 5f, 15f, true));
    }
    IEnumerator CheckClass()
    {
        
        WWW query = new WWW(checkClassURL + "?id=" + userID + "&secure=" + SecureKey);
        yield return query;
        Debug.Log(query.ToString());
        PlayerPrefs.SetString("Class", query.ToString());
        this.playerClass = PlayerPrefs.GetString("Class");

    }

    private void addSkill(string name, PlayerSkill newSkill)
    {

        allSkills.Add(newSkill.getSkillName(), newSkill);

        if (newSkill.getSkillClass() == playerClass)
        {
            this.ourSkills.Add(name);
        }
    }

    public List<string> getOurSkillNames()
    {
        return this.ourSkills;
    }

    public PlayerSkill getSkill(string skillName)
    {

        return allSkills[skillName];
    }

}
public enum SkillType
{
    HEALING,
    DAMAGING,
    DEFENDING,
    BOOSTDAMAGE,
    BOOSTSPD,
    DAMAGINGORHEALING,
    BOOSTENG,
    BOOSTDEF,
    OTHER,
    BOOSTHP
}
