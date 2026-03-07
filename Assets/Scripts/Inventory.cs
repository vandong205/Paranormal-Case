using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryOptionMenu optionMenu;
    [SerializeField] private ItemSlot[] slotPool;
    private int slotdynamicID = 0;
    public void OnItemClicked(ItemSlot slot)
    {
        optionMenu.Show(slot.ItemData, slot.OptionMenuPos);
    }

    public void AddItem(ItemData item)
    {
        for (int i = 0; i < slotPool.Length; i++)
        {
            if (slotPool[i].ItemData == null)
            {
                slotPool[i].SetItem(item);
                slotPool[i].dynammicId = slotdynamicID;
                slotdynamicID++;
                return;
            }
        }

        Debug.Log("Inventory full");
    }
}