using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SystemDefine {
    // Keys - PlayerPrefs
    public const string KEY_BGM_INDEX = "key_bgm_index";

    public const string KEY_VOLUME_BGM = "key_volume_bgm";
    public const string KEY_VOLUME_SE = "key_volume_se";

    // Window
    public const string UI_WINDOW_NAME_CONGRATULATIONS = "UIWindowCongratulations";
    public const string UI_WINDOW_NAME_CREDITS = "UIWindowCredits";
    public const string UI_WINDOW_NAME_DIFFICULTY = "UIWindowDifficulty";
    public const string UI_WINDOW_NAME_MESSAGE = "UIWindowMessage";
    public const string UI_WINDOW_NAME_OPTION = "UIWindowOption";
    public const string UI_WINDOW_NAME_SERIAL_NUMBER = "UIWindowSerialNumber";
}

[System.Serializable]
public class BGMData {
    public AudioClip BGM;
    public float RepeatTime;
    public float LoopStartTime;
}

public enum Difficulty { 
    Easy,
    Normal,
    Hard,
}
