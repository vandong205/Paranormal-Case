using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData ItemData { get; private set; }

    public int dynammicId;
    [SerializeField] Image itemIcon;
    public Vector3 OptionMenuPos;
    public void SetItem(ItemData item)
    {
        ItemData = item;
        itemIcon.sprite = item.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MainCanvas.Instance.PanelUI.Inventory.OnItemClicked(this);
    }
}