

using UnityEngine;
using System.Collections.Generic;

public class ItemFactory : MonoBehaviour, IItemFactory
{
    [System.Serializable]
    public class ItemEntry
    {
        public string id;
        public GameObject prefab;
    }

    public List<ItemEntry> items;
    private Dictionary<string, GameObject> itemMap;

    private void Awake()
    {
        itemMap = new Dictionary<string, GameObject>();
        foreach (var item in items)
        {
            itemMap[item.id] = item.prefab;
        }

        ServiceLocator.Register<IItemFactory>(this);
    }

    public GameObject CreateItem(string itemId, Vector3 position)
    {
        if (!itemMap.ContainsKey(itemId))
        {
            Debug.LogError($"Item {itemId} not found");
            return null;
        }

        return Instantiate(itemMap[itemId], position, Quaternion.identity);
    }
}
