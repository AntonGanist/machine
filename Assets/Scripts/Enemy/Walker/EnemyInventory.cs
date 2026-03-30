using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : MonoBehaviour
{
    [SerializeField] Transform _throwAwayPoint;
    [SerializeField] int _maxItems;
    List<Item> _items;

    public void TakeItem(Item item)
    {
        if(_items.Count+1 > _maxItems) return;
        _items.Add(item);
        item.SetActive(false);
    }

    public Item FindItemToId(int index)
    {
        for(int i = 0; i < _items.Count; i++)
        {
            if (_items[i].GetId() == index) return _items[i];
        }
        return null;
    }
    public List<Item> FindItemToType(ItemType itemType)
    {
        List <Item> items = new List <Item>();
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].GetType() == itemType) 
                items.Add(_items[i]);
        }
        return items;
    }

    public void ThrowAwayItem(int id)
    {
        Item item = null;
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].GetId() == id) item = _items[i]; break;
        }
        item.transform.position = _throwAwayPoint.position;
        item.SetActive(true);
    }

    public void UseItem()
    {

    }
    public void GetWeapon()
    {

    }
}
