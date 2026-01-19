using UnityEngine;

public interface IItemFactory
{
    GameObject CreateItem(string itemId, Vector3 position);
}