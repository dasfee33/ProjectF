using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
  Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
  public Dictionary<int, Data.TestData> TestDic { get; private set; } = new Dictionary<int, Data.TestData>();
  public Dictionary<int, Data.CreatureData> CreatureDic { get; private set; } = new Dictionary<int, Data.CreatureData>();
  public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
  public Dictionary<int, Data.EnvData> EnvDic { get; private set; } = new Dictionary<int, Data.EnvData>();
  
  public void Init()
  {
    TestDic = LoadJson<Data.TestDataLoader, int, Data.TestData>("TestData").MakeDict();
    CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureData").MakeDict();
    SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
    EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
  }

  private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
  {
    TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
    return JsonConvert.DeserializeObject<Loader>(textAsset.text);
  }
}

