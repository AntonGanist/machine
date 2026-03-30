using System;
using UnityEngine;

[Serializable] public enum ItemType { Weapen, Consumable, CarSegment, Trap}

public abstract class Item : MonoBehaviour
{
    [SerializeField] int _id;
    [SerializeField] ItemType _itemType;
    
    public virtual int GetId() => _id;
    public virtual ItemType GetType() => _itemType;
    public virtual void SetActive(bool active) => gameObject.SetActive(active);
}
