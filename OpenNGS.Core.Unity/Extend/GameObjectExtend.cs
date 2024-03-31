
using UnityEngine;
using UnityEngine.UI;

public static class GameObjectExtend{

	public static void SetSelfActive(this GameObject go ,bool active)
    {
        if (go)
        {
            go.SetActive(active);
        }
    }

    public static void SetSelfActive(this MonoBehaviour mono, bool active)
    {
        if (mono)
        {
            mono.gameObject.SetActive(active);
        }
    }

    public static void SetSelfEnable(this MonoBehaviour mono, bool enabled)
    {
        if (mono)
        {
            mono.enabled = enabled;
        }
    }

	public static void SafeSetTrigger(this Animator animtor, string trigger)
	{
		if (animtor)
		{
			animtor.SetTrigger(trigger);
		}
	}

    public static bool TrySetInteger(this Animator animtor, string trigger, int val)
    {
        if (animtor)
        {
            foreach(var param in animtor.parameters)
            {
                if (param.name == trigger)
                {
                    animtor.SetTrigger(trigger);
                    return true;
                }
            }
        }
        return false;
    }

    public static void SetToggleIsOn(this Toggle toggle,bool isOn)
    {
        if (toggle)
        {
            toggle.isOn = isOn;
        }
    }

    public static void SetButtonInteractable(this Button button, bool interactable)
    {
        if (button)
        {
            button.interactable = interactable;
        }
    }
}
