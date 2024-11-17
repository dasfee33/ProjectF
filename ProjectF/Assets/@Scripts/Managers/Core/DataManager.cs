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

  public Dictionary<int, Data.EquipmentItemData> EquipmentDic { get; private set; } = new Dictionary<int, Data.EquipmentItemData>();
  public Dictionary<int, Data.ConsumableItemData> ConsumableDic { get; private set; } = new Dictionary<int, Data.ConsumableItemData>();
  public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();

  public Dictionary<int, Data.DropTableData> DropDic { get; private set; } = new Dictionary<int, Data.DropTableData>();
  public Dictionary<int, Data.StructureData> StructDic { get; private set; } = new Dictionary<int, Data.StructureData>();

  public Dictionary<int, Data.ResearchData> ResearchDic { get; private set; } = new Dictionary<int, Data.ResearchData>();
  public Dictionary<string, Data.TextData> TextDic { get; private set; } = new Dictionary<string, Data.TextData>();

  public void Init()
  {
    TestDic = LoadJson<Data.TestDataLoader, int, Data.TestData>("TestData").MakeDict();
    CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureData").MakeDict();
    SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
    EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();

    EquipmentDic = LoadJson<Data.ItemDataLoader<Data.EquipmentItemData>, int, Data.EquipmentItemData>("Item_EquipmentData").MakeDict();
    ConsumableDic = LoadJson<Data.ItemDataLoader<Data.ConsumableItemData>, int, Data.ConsumableItemData>("Item_ConsumableData").MakeDict();

    ItemDic.Clear();
    foreach (var item in EquipmentDic)
      ItemDic.Add(item.Key, item.Value);
    foreach (var item in ConsumableDic)
      ItemDic.Add(item.Key, item.Value);

    DropDic = LoadJson<Data.DropTableDataLoader, int, Data.DropTableData>("DropTableData").MakeDict();
    StructDic = LoadJson<Data.StructureDataLoader, int, Data.StructureData>("StructureData").MakeDict();
    ResearchDic = LoadJson<Data.ResearchDataLoader, int, Data.ResearchData>("ResearchData").MakeDict();
    TextDic = LoadJson<Data.TextDataLoader, string, Data.TextData>("TextData").MakeDict();
  }

  private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
  {
    TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
    return JsonConvert.DeserializeObject<Loader>(textAsset.text);
  }
}

