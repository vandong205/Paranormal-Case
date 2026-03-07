using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool canUse;
    public bool canEquip;
    public bool canDrop;

    public void Use()
    {
        Debug.Log("Use " + itemName);
    }

    public void Equip()
    {
        Debug.Log("Equip " + itemName);
    }

    public void Drop()
    {
        Debug.Log("Drop " + itemName);
    }
}