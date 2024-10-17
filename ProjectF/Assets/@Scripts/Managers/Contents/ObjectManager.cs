using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
  public List<Creature> Creatures { get; } = new List<Creature> ();

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

  public T Spawn<T>(string name, Vector3 position) where T : BaseObject
  {
    GameObject go = Managers.Resource.Instantiate(name);
    go.name = name;
    go.transform.position = position;

    BaseObject obj = go.GetComponent<BaseObject>();

    if(obj.ObjectType == FObjectType.Creature)
    {
      Creature creature = go.GetComponent<Creature>();

      switch(creature.CreatureType)
      {
        case FCreatureType.WARRIOR:
          obj.transform.parent = CreatureRoot;
          Player warrior = creature as Player;
          Creatures.Add(warrior);
          break;
      }
    }
    //TODO


    return obj as T;
  }

  public void Despawn<T>(T obj) where T : BaseObject
  {
    FObjectType objectType = obj.ObjectType;

    if(obj.ObjectType != FObjectType.Creature)
    {
      Creature creature = obj as Creature;
      switch(creature.CreatureType)
      {
        case FCreatureType.WARRIOR:
          Player warrior = creature as Player;
          Creatures.Remove(warrior);
          break;
      }
    }
    //TODO


    Managers.Resource.Destroy(obj.gameObject);
  }
}
