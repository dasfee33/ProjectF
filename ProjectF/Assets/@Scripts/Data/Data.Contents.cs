using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
  #region TestData
  [Serializable]
  public class TestData
  {
    public int Level;
    public int Exp;
    public List<int> Skills;
    public float Speed;
    public string Name;
  }

  [Serializable]
  public class TestDataLoader : ILoader<int, TestData>
  {
    public List<TestData> tests = new List<TestData>();

    public Dictionary<int, TestData> MakeDict()
    {
      Dictionary<int, TestData> dict = new Dictionary<int, TestData>();
      foreach (TestData testData in tests)
        dict.Add(testData.Level, testData);

      return dict;
    }
  }
  #endregion

  #region CreatureData
  [Serializable]
  public class CreatureData
  {
    public int DataId;
    public string Name;
    public string DescriptionTextID;
    public string Label;
    public float maxHp;
    public float Speed;

    public string Idle;
    public string Move;
    public string Hurt;
    public string Dead;
    public List<int> SkillList = new List<int>();

  }

  [Serializable]
  public class CreatureDataLoader : ILoader<int, CreatureData>
  {
    public List<CreatureData> creatures = new List<CreatureData>();

    public Dictionary<int, CreatureData> MakeDict()
    {
      Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
      foreach (CreatureData creatureData in creatures)
        dict.Add(creatureData.DataId, creatureData);

      return dict;
    }
  }

  #endregion

  #region SkillData
  [Serializable]
  public class SkillData
  {
    public int DataId;
    public string Name;
    public string DescriptionTextID;
    public string Label;
    public string AnimName;
    public float CoolTime;
    public float DamageMultiflier;

  }

  [Serializable]
  public class SkillDataLoader : ILoader<int, SkillData>
  {
    public List<SkillData> skills = new List<SkillData> ();

    public Dictionary<int, SkillData> MakeDict()
    {
      Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
      foreach (SkillData skill in skills)
        dict.Add(skill.DataId, skill);
      return dict;
    }
  }

  #endregion

  #region EnvData
  [Serializable]
  public class EnvData
  {
    public int DataId;
    public string Name;
    public string DescriptionTextID;
    public string Label;
    public float maxHp;
    public float RegenTime;

    public string Idle;
    public string type;
  }

  [Serializable]
  public class EnvDataLoader : ILoader<int, EnvData>
  {
    public List<EnvData> envs = new List<EnvData> ();

    public Dictionary<int, EnvData> MakeDict()
    {
      Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
      foreach (EnvData env in envs)
        dict.Add(env.DataId, env);
      return dict;
    }

  }

  #endregion
}
