using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
  public List<Creature> Creatures { get; } = new List<Creature> ();
  public List<Env> Envs { get; } = new List<Env> ();
  public List<ItemHolder> ItemHolders { get; } = new List<ItemHolder> (); 

  public List<BaseObject> Workables { get; } = new List<BaseObject>();

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

  public T Spawn<T>(Vector3 position, int dataID, string prefabName = null) where T : BaseObject
  {
    if(string.IsNullOrEmpty(prefabName)) prefabName = typeof(T).Name;

    GameObject go = Managers.Resource.Instantiate(prefabName);
    go.name = prefabName;
    go.transform.position = position;

    BaseObject obj = go.GetComponent<BaseObject>();
    var cellPos = Managers.Map.World2Cell(position);

    if(obj.ObjectType == FObjectType.Creature)
    {
      Creature creature = go.GetComponent<Creature>();

      switch(creature.CreatureType)
      {
        case FCreatureType.WARRIOR:
          obj.transform.parent = CreatureRoot;
          Warrior warrior = creature as Warrior;
          Creatures.Add(warrior);
          break;
      }

      creature.SetInfo(dataID);
    }
    else if(obj.ObjectType == FObjectType.Env)
    {
      obj.transform.parent = EnvRoot;
      Env env = obj as Env;
      Envs.Add(env);
      Workables.Add(obj);

      env.SetInfo(dataID);
    }
    else if(obj.ObjectType == FObjectType.ItemHolder)
    {
      obj.transform.parent = ItemHolderRoot;
      ItemHolder itemholder = obj as ItemHolder;
      ItemHolders.Add(itemholder);

    }
    //TODO

    //FIXME
    //if(obj.ObjectType != FObjectType.ItemHolder)
    Managers.Map.AddObject(obj, cellPos);

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
        case FCreatureType.WARRIOR:
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
    //TODO

    //if (obj.ObjectType != FObjectType.ItemHolder)
    Managers.Map.ClearObject(cellPos);

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
        case FCreatureType.WARRIOR:
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
    //TODO

    //if (obj.ObjectType != FObjectType.ItemHolder)
    Managers.Map.ClearObject(cellPos);

    Managers.Resource.Destroy(obj.gameObject);
  }
}
