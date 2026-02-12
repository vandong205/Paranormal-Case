using TMPro;
using UnityEngine;
using System;
public class ImmobileDoor : MonoBehaviour, IInteracable
{
    [SerializeField] private Outline outliner;
    [SerializeField] DoorSign doorSign;
    [SerializeField] bool canEditSign = false;
    [SerializeField] int correctSignNumber = 42;
    [SerializeField] TextMeshPro labelNumber;

    public event Action OnDoorUnlocked;
    private bool isSignShown = false;
    private void Awake()
    {
        if (outliner == null)
            outliner = GetComponentInChildren<Outline>();

        if (outliner != null)
            outliner.enabled = false;
    }
    public void Interact()
    {
        Debug.Log("Interacted with immobile door: " + gameObject.name);
        if(doorSign != null)
        {
            doorSign.editable = canEditSign;
            doorSign.BindOwner(this);
            if (canEditSign) doorSign.ClearData();
            else doorSign.SetNumber(correctSignNumber);
            if (!isSignShown)
            {
                VDGlobal.Instance.DisableMoveAction();
                doorSign.Show();
            }


            else
            {
                doorSign.Hide();
                VDGlobal.Instance.EnableMoveAction();
            }
               
            isSignShown = !isSignShown;
        }
    }

    public void Highlight(bool highlight)
    {
        if (outliner == null)
            outliner = GetComponentInChildren<Outline>();

        if (outliner != null)
            outliner.enabled = highlight;
    }
    public bool CheckSignNumber(int signnum)
    {
        if(doorSign != null)
        {
            if(correctSignNumber== signnum)
            {
                DialogBox.Instance.PushText("The door clicks open as you enter the correct number.");
                doorSign.Hide();
                VDGlobal.Instance.EnableMoveAction();
                labelNumber.text = correctSignNumber.ToString();
                isSignShown = false;
                canEditSign = false;
                if(OnDoorUnlocked != null)
                    OnDoorUnlocked.Invoke();
                else
                {
                    Debug.LogWarning("ImmobileDoor: OnDoorUnlocked event has no subscribers.");
                }
                return true;
            }
            else
            {
                DialogBox.Instance.PushText("The door remains firmly shut. The number on the sign must be incorrect.");
                return false;
            }
        }else
            return false;
    }
}
