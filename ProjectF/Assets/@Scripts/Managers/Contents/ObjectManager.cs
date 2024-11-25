using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
  public List<Creature> Creatures { get; } = new List<Creature> ();
  public List<Env> Envs { get; } = new List<Env> ();
  public List<Structure> Structures { get; } = new List<Structure> ();
  public List<ItemHolder> ItemHolders { get; } = new List<ItemHolder> ();
  public Dictionary<int, float> ItemStorage { get; } = new Dictionary<int, float> ();
  public List<BuildObject> BuildObjects { get; } = new List<BuildObject> ();

  public List<BaseObject> Workables { get; } = new List<BaseObject>();

  public List<Structure> Pipes { get; } = new List<Structure> ();
  public List<Structure> Furnitures { get; } = new List<Structure> ();
  public List<Structure> Bases { get; } = new List<Structure> ();
  public List<Structure> Electronincs { get; } = new List<Structure> ();
  public List<Structure> Stations { get; } = new List<Structure> ();

  public List<int> PossPipes { get; } = new List<int>();
  public List<int> PossFurnitures { get; } = new List<int>();
  public List<int> PossBases { get; } = new List<int>();
  public List<int> PossElectronics { get; } = new List<int>();
  public List<int> PossStations { get; } = new List<int>();
  public List<int> PossCooks { get; } = new List<int>();

  public Transform GetRootTransform(string name)
  {
    GameObject root = GameObject.Find (name);
    if(root == null)
    {
      root = new GameObject { name = name };
    }
    return root.transform;
  }

  public Transform CreatureRoot { get { return GetRootTransform("@Creatures"); } }
  public Transform EnvRoot { get { return GetRootTransform("@Envs"); } }
  public Transform ItemHolderRoot { get { return GetRootTransform("@ItemHolders"); } }
  public Transform StructureRoot { get { return GetRootTransform("@Structures"); } }
  public Transform BuildObjectRoot { get { return GetRootTransform("@BuildObjects"); } }

  public T Spawn<T>(Vector3 position, int dataID, string prefabName = null, bool addToCell = true, Vector3 dropPos = default(Vector3), BaseObject Owner = null, bool isFarm = false) where T : BaseObject
  {
    if(string.IsNullOrEmpty(prefabName)) prefabName = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(prefabName);
    var cellPos = Managers.Map.World2Cell(position);
    go.name = prefabName;
    if (addToCell) go.transform.position = Managers.Map.Cell2World(cellPos) + Managers.Map.LerpObjectPos;
    else go.transform.position = position;
    
     //= position;// + Managers.Map.LerpObjectPos;

    BaseObject obj = go.GetComponent<BaseObject>();
    

    if (obj.ObjectType == FObjectType.Creature)
    {
      Creature creature = go.GetComponent<Creature>();

      switch (creature.CreatureType)
      {
        case FCreatureType.Warrior:
          obj.transform.parent = CreatureRoot;
          Warrior warrior = creature as Warrior;
          Creatures.Add(warrior);
          break;
      }

      creature.SetInfo(dataID);
    }
    else if (obj.ObjectType == FObjectType.Env)
    {
      obj.transform.parent = EnvRoot;
      Env env = obj as Env;
      Envs.Add(env);
      Workables.Add(obj);

      env.SetInfo(dataID, isFarm);
    }
    else if (obj.ObjectType == FObjectType.ItemHolder)
    {
      obj.transform.parent = ItemHolderRoot;
      ItemHolder itemholder = obj as ItemHolder;
      itemholder.Owner = Owner;
      itemholder.SetInfo(-999, dataID, dropPos);
      ItemHolders.Add(itemholder);
      AddItem(itemholder);
    }
    else if (obj.ObjectType == FObjectType.Structure)
    {
      obj.transform.parent = StructureRoot;
      Structure structure = obj as Structure;
      Structures.Add(structure);
      Workables.Add(obj);

      if (structure.StructureType == FStructureType.Pipe)
      {
        Pipes.Add(structure);
      }
      else if (structure.StructureType == FStructureType.Furniture)
      {
        Furnitures.Add(structure);
      }
      else if (structure.StructureType == FStructureType.Electronic)
      {
        Electronincs.Add(structure);
      }
      else if (structure.StructureType == FStructureType.Base)
      {
        Bases.Add(structure);
      }



      structure.SetInfo(dataID);
    }
    else if (obj.ObjectType == FObjectType.BuildObject)
    {
      obj.transform.parent = BuildObjectRoot;
      BuildObject build = obj as BuildObject;
      BuildObjects.Add(build);
      Workables.Add(obj);

      build.SetInfo(dataID, Managers.Data.StructDic[dataID].buildItemId, Managers.Data.StructDic[dataID].buildItemMass);
    }
    //TODO

    //FIXME
    //if(obj.ObjectType != FObjectType.ItemHolder)
    if (addToCell) Managers.Map.AddObject(obj, cellPos);

    return obj as T;
  }

  public void Despawn<T>(T obj) where T : BaseObject
  {
    FObjectType objectType = obj.ObjectType;
    var cellPos = Managers.Map.World2Cell(obj.transform.position);

    if(obj.ObjectType == FObjectType.Creature)
    {
      Creature creature = obj as Creature;
      switch(creature.CreatureType)
      {
        case FCreatureType.Warrior:
          Warrior warrior = creature as Warrior;
          Creatures.Remove(warrior);
          break;
      }
    }
    else if(obj.ObjectType == FObjectType.Env)
    {
      Env env = obj as Env;
      Envs.Remove(env);
      Workables.Remove(obj);
    }
    else if (obj.ObjectType == FObjectType.ItemHolder)
    {
      ItemHolder itemHolder = obj as ItemHolder;
      ItemHolders.Remove(itemHolder);
      RemoveItem(itemHolder);
    }
    else if (obj.ObjectType == FObjectType.Structure)
    {
      Structure structure = obj as Structure;
      Structures.Remove(structure);
      Workables.Remove(obj);
    }
    else if(obj.ObjectType == FObjectType.BuildObject)
    {
      BuildObject buildObject = obj as BuildObject;
      BuildObjects.Remove(buildObject);
      Workables.Remove(obj);
    }
    //TODO

    //if (obj.ObjectType != FObjectType.ItemHolder)
    var bo = Managers.Map.GetObject(cellPos);
    if(bo == obj) Managers.Map.ClearObject(cellPos);

    Managers.Resource.Destroy(obj.gameObject);
  }

  public void Despawn<T>(T obj, Vector3 originPos) where T : BaseObject
  {
    FObjectType objectType = obj.ObjectType;
    var cellPos = Managers.Map.World2Cell(originPos);

    if (obj.ObjectType == FObjectType.Creature)
    {
      Creature creature = obj as Creature;
      switch (creature.CreatureType)
      {
        case FCreatureType.Warrior:
          Warrior warrior = creature as Warrior;
          Creatures.Remove(warrior);
          break;
      }
    }
    else if (obj.ObjectType == FObjectType.Env)
    {
      Env env = obj as Env;
      Envs.Remove(env);
      Workables.Remove(obj);
    }
    else if (obj.ObjectType == FObjectType.ItemHolder)
    {
      ItemHolder itemHolder = obj as ItemHolder;
      ItemHolders.Remove(itemHolder);
      RemoveItem(itemHolder);
    }
    else if (obj.ObjectType == FObjectType.Structure)
    {
      Structure structure = obj as Structure;
      Structures.Remove(structure);
      Workables.Remove(obj);
    }
    //TODO

    //if (obj.ObjectType != FObjectType.ItemHolder)
    var bo = Managers.Map.GetObject(cellPos);
    if(bo == obj) Managers.Map.ClearObject(cellPos);

    Managers.Resource.Destroy(obj.gameObject);
  }

  public void AddItem(ItemHolder item)
  {
    if (ItemStorage.ContainsKey(item.dataTemplateID))
      ItemStorage[item.dataTemplateID] += item.data.Mass;
    else ItemStorage.Add(item.dataTemplateID, item.data.Mass);
  }

  public void AddItem(ItemHolder item, float mass)
  {
    ItemStorage[item.dataTemplateID] += mass;
  }

  public void AddItem(int id, float mass)
  {
    ItemStorage[id] += mass;
  }

  public void RemoveItem(ItemHolder item)
  {
    if (!ItemStorage.ContainsKey(item.dataTemplateID)) return;
    ItemStorage[item.dataTemplateID] -= item.data.Mass;
    if (ItemStorage[item.dataTemplateID] < 0) ItemStorage.Remove(item.dataTemplateID);
  }

  public void RemoveItem(ItemHolder item, float mass)
  {
    if (!ItemStorage.ContainsKey(item.dataTemplateID)) return;
    ItemStorage[item.dataTemplateID] -= mass;
    if (ItemStorage[item.dataTemplateID] < 0) ItemStorage.Remove(item.dataTemplateID);
  }

  public void RemoveItem(int id, float mass)
  {
    if (!ItemStorage.ContainsKey(id)) return;
    ItemStorage[id] -= mass;
  }
}
