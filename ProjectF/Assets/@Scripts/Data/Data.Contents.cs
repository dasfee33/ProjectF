using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

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
    public string Job;

    public float supplyCap;

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
    public float GrowthTime;
    public float RegenTime;

    public string Idle;
    public string type;
    public string Hurt;
    public string Dead;

    public int DropItemid;
    public int Supplyitemid;

    public int DeadEnv;
    public int RegenEnv;
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

  #region Structure
  [Serializable]
  public class StructureData
  {
    public int DataId;
    public string Name;
    public string DescriptionTextID;
    public string Label;
    public string type;
    public string subType;
    public string Sprite;

    public float maxHp;
    public float WorkTime;
    public float BuildTime;

    public string Idle;
    public string WorkStart;
    public string Work;
    public string WorkEnd;

    public List<int> buildItemId;
    public List<float> buildItemMass;

    public List<int> supplyItemid;
    public List<float> supplyItemMass;

    public float maxCapacity;
    public int extraCellX;
    public int extraCellY;

    public int option;
  }

  [Serializable]
  public class StructureDataLoader : ILoader<int, StructureData>
  {
    public List<StructureData> structures = new List<StructureData>();

    public Dictionary<int, StructureData> MakeDict()
    {
      Dictionary<int, StructureData> dict = new Dictionary<int, StructureData> ();
      foreach (StructureData structure in structures)
        dict.Add(structure.DataId, structure);
      return dict;
    }
  }

  #endregion

  #region Item
  [Serializable]
  public class ItemData
  {
    public int DataId;
    public string Name;
    public string DescirptionTextID;
    public string Label;
    public string type;

    public FItemGroupType ItemGroupType;
    public FItemType ItemType;
    public FItemSubType ItemSubType;
    public FItemGrade ItemGrade;

    public int maxStack;
    public float Mass;
    public string Sprite;
  }

  [Serializable]
  public class EquipmentItemData : ItemData
  {
    public int Damage;
    public int Defence;
    public int Speed;
  }

  [Serializable]
  public class ConsumableItemData : ItemData
  {
    public double Value;
    public double CoolTIme;
    public FJob SupplyJob;
    public int ParentEnv;
    public float Calories;
  }

  [Serializable]
  public class ItemDataLoader<T> : ILoader<int, T> where T : ItemData
  {
    public List<T> items = new List<T>();

    public Dictionary<int, T> MakeDict()
    {
      Dictionary<int, T> dict = new Dictionary<int, T>();
      foreach (T item in items)
        dict.Add(item.DataId, item);

      return dict;
    }
  }

  #endregion

  #region DropTable
  
  public class RewardData
  {
    public int itemTemplateId;
    public int Probability;
  }

  [Serializable]
  public class DropTableData_Internal
  {
    public int DataId;
    public int RewardExp;
    public int Prob1;
    public int Item1;
    public int Prob2;
    public int Item2;
    public int Prob3;
    public int Item3;
    public int Prob4;
    public int Item4;
    public int Prob5;
    public int Item5;
  }


  [Serializable]
  public class DropTableData
  {
    public int DataId;
    public int RewardExp;
    public List<RewardData> Rewards = new List<RewardData>();
  }

  [Serializable]
  public class DropTableDataLoader : ILoader<int, DropTableData>
  {
    public List<DropTableData_Internal> dropTables = new List<DropTableData_Internal>();

    public Dictionary<int, DropTableData> MakeDict()
    {
      Dictionary<int, DropTableData> dict = new Dictionary<int, DropTableData> ();
      foreach(DropTableData_Internal tempData in dropTables)
      {
        DropTableData data = new DropTableData()
        {
          DataId = tempData.DataId,
          RewardExp = tempData.RewardExp,
        };

        if(tempData.Item1 > 0)
        {
          data.Rewards.Add(new RewardData()
          {
            itemTemplateId = tempData.Item1,
            Probability = tempData.Prob1,
          });
        }

        if (tempData.Item2 > 0)
        {
          data.Rewards.Add(new RewardData()
          {
            itemTemplateId = tempData.Item2,
            Probability = tempData.Prob2,
          });
        }

        if (tempData.Item3 > 0)
        {
          data.Rewards.Add(new RewardData()
          {
            itemTemplateId = tempData.Item3,
            Probability = tempData.Prob3,
          });
        }

        if (tempData.Item4 > 0)
        {
          data.Rewards.Add(new RewardData()
          {
            itemTemplateId = tempData.Item4,
            Probability = tempData.Prob4,
          });
        }

        if (tempData.Item5 > 0)
        {
          data.Rewards.Add(new RewardData()
          {
            itemTemplateId = tempData.Item5,
            Probability = tempData.Prob5,
          });
        }

        dict.Add(data.DataId, data);
      }
      return dict;
    }
  }

  #endregion

  #region Research
  [Serializable]
  public class ResearchData
  {
    public int DataId;
    public string Name;

    public FResearchType Type;

    public List<int> Before;
    public List<int> After;
    public List<int> BuildId;

    public float Step1;
    public float Step2;
    public float Step3;
  }

  [Serializable]
  public class ResearchDataLoader : ILoader<int, ResearchData>
  {
    public List<ResearchData> researches = new List<ResearchData>();

    public Dictionary<int, ResearchData> MakeDict()
    {
      Dictionary<int, ResearchData> dict = new Dictionary<int, ResearchData>();
      foreach (ResearchData research in researches)
        dict.Add(research.DataId, research);
      return dict;
    }
  }
  #endregion

  #region TextData
  [Serializable]
  public class TextData
  {
    public string DataId;
    public string KOR;
  }

  [Serializable]
  public class TextDataLoader : ILoader<string, TextData>
  {
    public List<TextData> texts = new List<TextData>();

    public Dictionary<string, TextData> MakeDict()
    {
      Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
      foreach (TextData text in texts)
        dict.Add(text.DataId, text);
      return dict;
    }

  }

  #endregion
}
