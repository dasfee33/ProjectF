using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
  public List<Creature> Creatures { get; } = new List<Creature> ();
  public List<Env> Envs { get; } = new List<Env> ();
  public List<Structure> Structures { get; } = new List<Structure> ();
  public List<ItemHolder> ItemHolders { get; } = new List<ItemHolder> ();
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

  public T Spawn<T>(Vector3 position, int dataID, string prefabName = null, bool addToCell = true) where T : BaseObject
  {
    if(string.IsNullOrEmpty(prefabName)) prefabName = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(prefabName);
    go.name = prefabName;
    go.transform.position = position + Managers.Map.LerpObjectPos;

    BaseObject obj = go.GetComponent<BaseObject>();
    var cellPos = Managers.Map.World2Cell(position);

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

      env.SetInfo(dataID);
    }
    else if (obj.ObjectType == FObjectType.ItemHolder)
    {
      obj.transform.parent = ItemHolderRoot;
      ItemHolder itemholder = obj as ItemHolder;
      ItemHolders.Add(itemholder);

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
}
