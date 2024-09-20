using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnchorPoint
{
    Head = 0,
    Body = 1,
    Foot = 2,
    LeftHand = 3,
    RightHand = 4,
    Weapon = 5,
    HeadHigh = 6,
    Stun = 7,
    Custom = 8,
}

public class Const 
{
    public static Quaternion QuaternionIdentity = Quaternion.identity;
    public const float HidePos = -9999.0f;
    public const int AOI_SIZE = 1500;
    public static readonly float WorldLogicFactor = UFloat.Round(0.01f);

    public static Color ColorRed = Color.red;
    public static Color ColorWhite = Color.white;
    public static Color ColorBlue = Color.blue;
    public static Color ColorGreen = Color.green;
    public static Color ColorYellow = Color.yellow;
    public static Color ColorBlack = Color.black;
    public static Color ColorGray = Color.gray;
    public static Color ColorCyan = Color.cyan;
    public static Color ColorCDPurple = new Color(1, 0.455f, 1, 1);
    public static Color ColorCDYellow = new Color(0.99f, 0.8f, 0.094f, 1);
    public static Color ColorAlphaWhite = new Color(1, 1, 1, 0);

    public static Color DefaultFloorColor = new Color(61f / 255f, 127f / 255f, 193f / 255f, 113f / 255f);
    public static Color DefaultFloorColor_Circle = new Color(50f / 255f, 101f / 255f, 171f / 255f, 86f / 255f);
    public static Color RedFloorColor = new Color(1f, 0, 0, 60f / 255f);

    public const float JoystickInvalidDistance = 0.3f;
}
