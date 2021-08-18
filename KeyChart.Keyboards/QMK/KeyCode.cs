// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

using System;

namespace KeyChart.Keyboards.QMK
{
    internal static class Sequencer
    {
        internal const ushort Steps = 16;
        internal const ushort Tracks = 8;
        internal const ushort Resolutions = 9;
    }

    [Flags]
    public enum KeyCode: ushort
    {

    NO = 0x00,
    ROLL_OVER,
    POST_FAIL,
    UNDEFINED,
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,  // 0x10
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    D1,
    D2,
    D3,  // 0x20
    D4,
    D5,
    D6,
    D7,
    D8,
    D9,
    D0,
    ENTER,
    ESCAPE,
    BSPACE,
    TAB,
    SPACE,
    MINUS,
    EQUAL,
    LBRACKET,
    RBRACKET,  // 0x30
    BSLASH,
    NONUS_HASH,
    SCOLON,
    QUOTE,
    GRAVE,
    COMMA,
    DOT,
    SLASH,
    CAPSLOCK,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,  // 0x40
    F8,
    F9,
    F10,
    F11,
    F12,
    PSCREEN,
    SCROLLLOCK,
    PAUSE,
    INSERT,
    HOME,
    PGUP,
    DELETE,
    END,
    PGDOWN,
    RIGHT,
    LEFT,  // 0x50
    DOWN,
    UP,
    NUMLOCK,
    KP_SLASH,
    KP_ASTERISK,
    KP_MINUS,
    KP_PLUS,
    KP_ENTER,
    KP_1,
    KP_2,
    KP_3,
    KP_4,
    KP_5,
    KP_6,
    KP_7,
    KP_8,  // 0x60
    KP_9,
    KP_0,
    KP_DOT,
    NONUS_BSLASH,
    APPLICATION,
    POWER,
    KP_EQUAL,
    F13,
    F14,
    F15,
    F16,
    F17,
    F18,
    F19,
    F20,
    F21,  // 0x70
    F22,
    F23,
    F24,
    EXECUTE,
    HELP,
    MENU,
    SELECT,
    STOP,
    AGAIN,
    UNDO,
    CUT,
    COPY,
    PASTE,
    FIND,
    _MUTE,
    _VOLUP,  // 0x80
    _VOLDOWN,
    LOCKING_CAPS,
    LOCKING_NUM,
    LOCKING_SCROLL,
    KP_COMMA,
    KP_EQUAL_AS400,
    INT1,
    INT2,
    INT3,
    INT4,
    INT5,
    INT6,
    INT7,
    INT8,
    INT9,
    LANG1,  // 0x90
    LANG2,
    LANG3,
    LANG4,
    LANG5,
    LANG6,
    LANG7,
    LANG8,
    LANG9,
    ALT_ERASE,
    SYSREQ,
    CANCEL,
    CLEAR,
    PRIOR,
    RETURN,
    SEPARATOR,
    OUT,  // 0xA0
    OPER,
    CLEAR_AGAIN,
    CRSEL,
    EXSEL,
    
  // ***************************************************************
  // These keycodes are present in the HID spec, but are           *
  // nonfunctional on modern OSes. QMK uses this range (0xA5-0xDF) *
  // for the media and function keys instead - see below.          *
  // ***************************************************************
  /* Media and Function keys */
        /* Generic Desktop Page (0x01) */
        SYSTEM_POWER = 0xA5,
        SYSTEM_SLEEP,
        SYSTEM_WAKE,

        /* Consumer Page (0x0C) */
        AUDIO_MUTE,
        AUDIO_VOL_UP,
        AUDIO_VOL_DOWN,
        MEDIA_NEXT_TRACK,
        MEDIA_PREV_TRACK,
        MEDIA_STOP,
        MEDIA_PLAY_PAUSE,
        MEDIA_SELECT,
        MEDIA_EJECT,  // 0xB0
        MAIL,
        CALCULATOR,
        MY_COMPUTER,
        WWW_SEARCH,
        WWW_HOME,
        WWW_BACK,
        WWW_FORWARD,
        WWW_STOP,
        WWW_REFRESH,
        WWW_FAVORITES,
        MEDIA_FAST_FORWARD,
        MEDIA_REWIND,
        BRIGHTNESS_UP,
        BRIGHTNESS_DOWN,

        /* Fn keys */
        FN0 = 0xC0,
        FN1,
        FN2,
        FN3,
        FN4,
        FN5,
        FN6,
        FN7,
        FN8,
        FN9,
        FN10,
        FN11,
        FN12,
        FN13,
        FN14,
        FN15,
        FN16,  // 0xD0
        FN17,
        FN18,
        FN19,
        FN20,
        FN21,
        FN22,
        FN23,
        FN24,
        FN25,
        FN26,
        FN27,
        FN28,
        FN29,
        FN30,
        FN31,

        /* Modifiers */
    LCTRL = 0xE0,
    LSHIFT,
    LALT,
    LGUI,
    RCTRL,
    RSHIFT,
    RALT,
    RGUI,

    // **********************************************
    // * 0xF0-0xFF are unallocated in the HID spec. *
    // * QMK uses these for Mouse Keys - see below. *
    // **********************************************
    
    QK_BASIC                = 0x0000,
    QK_BASIC_MAX            = 0x00FF,
    QK_MODS                 = 0x0100,
    QK_LCTL                 = 0x0100,
    QK_LSFT                 = 0x0200,
    QK_LALT                 = 0x0400,
    QK_LGUI                 = 0x0800,
    QK_RMODS_MIN            = 0x1000,
    QK_RCTL                 = 0x1100,
    QK_RSFT                 = 0x1200,
    QK_RALT                 = 0x1400,
    QK_RGUI                 = 0x1800,
    QK_MODS_MAX             = 0x1FFF,
    QK_FUNCTION             = 0x2000,
    QK_FUNCTION_MAX         = 0x2FFF,
    QK_MACRO                = 0x3000,
    QK_MACRO_MAX            = 0x3FFF,
    QK_LAYER_TAP            = 0x4000,
    QK_LAYER_TAP_MAX        = 0x4FFF,
    QK_TO                   = 0x5000,
    QK_TO_MAX               = 0x50FF,
    QK_MOMENTARY            = 0x5100,
    QK_MOMENTARY_MAX        = 0x51FF,
    QK_DEF_LAYER            = 0x5200,
    QK_DEF_LAYER_MAX        = 0x52FF,
    QK_TOGGLE_LAYER         = 0x5300,
    QK_TOGGLE_LAYER_MAX     = 0x53FF,
    QK_ONE_SHOT_LAYER       = 0x5400,
    QK_ONE_SHOT_LAYER_MAX   = 0x54FF,
    QK_ONE_SHOT_MOD         = 0x5500,
    QK_ONE_SHOT_MOD_MAX     = 0x55FF,
    QK_SWAP_HANDS           = 0x5600,
    QK_SWAP_HANDS_MAX       = 0x56FF,
    QK_TAP_DANCE            = 0x5700,
    QK_TAP_DANCE_MAX        = 0x57FF,
    QK_LAYER_TAP_TOGGLE     = 0x5800,
    QK_LAYER_TAP_TOGGLE_MAX = 0x58FF,
    QK_LAYER_MOD            = 0x5900,
    QK_LAYER_MOD_MAX        = 0x59FF,
    QK_STENO                = 0x5A00,
    QK_STENO_BOLT           = 0x5A30,
    QK_STENO_GEMINI         = 0x5A31,
    QK_STENO_MAX            = 0x5A3F,
    // 0x5C00 - 0x5FFF are reserved, see below
    QK_MOD_TAP             = 0x6000,
    QK_MOD_TAP_MAX         = 0x7FFF,
    QK_UNICODE             = 0x8000,
    QK_UNICODE_MAX         = 0xFFFF,
    QK_UNICODEMAP          = 0x8000,
    QK_UNICODEMAP_MAX      = 0xBFFF,
    QK_UNICODEMAP_PAIR     = 0xC000,
    QK_UNICODEMAP_PAIR_MAX = 0xFFFF,

    // Loose keycodes - to be used directly
    RESET = 0x5C00,
    DEBUG,  // 5C01

    // Magic
    MAGIC_SWAP_CONTROL_CAPSLOCK,       // 5C02
    MAGIC_CAPSLOCK_TO_CONTROL,         // 5C03
    LAG_SWP,              // 5C04
    RAG_SWP,              // 5C05
    MAGIC_NO_GUI,                      // 5C06
    MAGIC_SWAP_GRAVE_ESC,              // 5C07
    MAGIC_SWAP_BACKSLASH_BACKSPACE,    // 5C08
    MAGIC_HOST_NKRO,                   // 5C09
    AG_SWAP,                // 5C0A
    MAGIC_UNSWAP_CONTROL_CAPSLOCK,     // 5C0B
    MAGIC_UNCAPSLOCK_TO_CONTROL,       // 5C0C
    LAG_NRM,            // 5C0D
    RAG_NRM,            // 5C0E
    MAGIC_UNNO_GUI,                    // 5C0F
    MAGIC_UNSWAP_GRAVE_ESC,            // 5C10
    MAGIC_UNSWAP_BACKSLASH_BACKSPACE,  // 5C11
    MAGIC_UNHOST_NKRO,                 // 5C12
    AG_NORM,              // 5C13
    MAGIC_TOGGLE_NKRO,                 // 5C14
    AG_TOGG,              // 5C15

    // Grave Escape
    GRAVE_ESC,  // 5C16

    // Auto Shift
    KC_ASUP,   // 5C17
    KC_ASDN,   // 5C18
    KC_ASRP,   // 5C19
    KC_ASTG,   // 5C1A
    KC_ASON,   // 5C1B
    KC_ASOFF,  // 5C1C

    // Audio
    AU_ON,   // 5C1D
    AU_OFF,  // 5C1E
    AU_TOG,  // 5C1F

    // Audio Clicky
    CLICKY_TOGGLE,   // 5C20
    CLICKY_ENABLE,   // 5C21
    CLICKY_DISABLE,  // 5C22
    CLICKY_UP,       // 5C23
    CLICKY_DOWN,     // 5C24
    CLICKY_RESET,    // 5C25

    // Music mode
    MU_ON,   // 5C26
    MU_OFF,  // 5C27
    MU_TOG,  // 5C28
    MU_MOD,  // 5C29
    MUV_IN,  // 5C2A
    MUV_DE,  // 5C2B

    // MIDI
    MI_ON,   // 5C2C
    MI_OFF,  // 5C2D
    MI_TOG,  // 5C2E

    MI_C,   // 5C2F
    MI_Cs,  // 5C30
    MI_Db = MI_Cs,
    MI_D,   // 5C31
    MI_Ds,  // 5C32
    MI_Eb = MI_Ds,
    MI_E,   // 5C33
    MI_F,   // 5C34
    MI_Fs,  // 5C35
    MI_Gb = MI_Fs,
    MI_G,   // 5C36
    MI_Gs,  // 5C37
    MI_Ab = MI_Gs,
    MI_A,   // 5C38
    MI_As,  // 5C39
    MI_Bb = MI_As,
    MI_B,  // 5C3A

    MI_C_1,   // 5C3B
    MI_Cs_1,  // 5C3C
    MI_Db_1 = MI_Cs_1,
    MI_D_1,   // 5C3D
    MI_Ds_1,  // 5C3E
    MI_Eb_1 = MI_Ds_1,
    MI_E_1,   // 5C3F
    MI_F_1,   // 5C40
    MI_Fs_1,  // 5C41
    MI_Gb_1 = MI_Fs_1,
    MI_G_1,   // 5C42
    MI_Gs_1,  // 5C43
    MI_Ab_1 = MI_Gs_1,
    MI_A_1,   // 5C44
    MI_As_1,  // 5C45
    MI_Bb_1 = MI_As_1,
    MI_B_1,  // 5C46

    MI_C_2,   // 5C47
    MI_Cs_2,  // 5C48
    MI_Db_2 = MI_Cs_2,
    MI_D_2,   // 5C49
    MI_Ds_2,  // 5C4A
    MI_Eb_2 = MI_Ds_2,
    MI_E_2,   // 5C4B
    MI_F_2,   // 5C4C
    MI_Fs_2,  // 5C4D
    MI_Gb_2 = MI_Fs_2,
    MI_G_2,   // 5C4E
    MI_Gs_2,  // 5C4F
    MI_Ab_2 = MI_Gs_2,
    MI_A_2,   // 5C50
    MI_As_2,  // 5C51
    MI_Bb_2 = MI_As_2,
    MI_B_2,  // 5C52

    MI_C_3,   // 5C53
    MI_Cs_3,  // 5C54
    MI_Db_3 = MI_Cs_3,
    MI_D_3,   // 5C55
    MI_Ds_3,  // 5C56
    MI_Eb_3 = MI_Ds_3,
    MI_E_3,   // 5C57
    MI_F_3,   // 5C58
    MI_Fs_3,  // 5C59
    MI_Gb_3 = MI_Fs_3,
    MI_G_3,   // 5C5A
    MI_Gs_3,  // 5C5B
    MI_Ab_3 = MI_Gs_3,
    MI_A_3,   // 5C5C
    MI_As_3,  // 5C5D
    MI_Bb_3 = MI_As_3,
    MI_B_3,  // 5C5E

    MI_C_4,   // 5C5F
    MI_Cs_4,  // 5C60
    MI_Db_4 = MI_Cs_4,
    MI_D_4,   // 5C61
    MI_Ds_4,  // 5C62
    MI_Eb_4 = MI_Ds_4,
    MI_E_4,   // 5C63
    MI_F_4,   // 5C64
    MI_Fs_4,  // 5C65
    MI_Gb_4 = MI_Fs_4,
    MI_G_4,   // 5C66
    MI_Gs_4,  // 5C67
    MI_Ab_4 = MI_Gs_4,
    MI_A_4,   // 5C68
    MI_As_4,  // 5C69
    MI_Bb_4 = MI_As_4,
    MI_B_4,  // 5C6A

    MI_C_5,   // 5C6B
    MI_Cs_5,  // 5C6C
    MI_Db_5 = MI_Cs_5,
    MI_D_5,   // 5C6D
    MI_Ds_5,  // 5C6E
    MI_Eb_5 = MI_Ds_5,
    MI_E_5,   // 5C6F
    MI_F_5,   // 5C70
    MI_Fs_5,  // 5C71
    MI_Gb_5 = MI_Fs_5,
    MI_G_5,   // 5C72
    MI_Gs_5,  // 5C73
    MI_Ab_5 = MI_Gs_5,
    MI_A_5,   // 5C74
    MI_As_5,  // 5C75
    MI_Bb_5 = MI_As_5,
    MI_B_5,  // 5C76

    MI_OCT_N2,  // 5C77
    MI_OCT_N1,  // 5C78
    MI_OCT_0,   // 5C79
    MI_OCT_1,   // 5C7A
    MI_OCT_2,   // 5C7B
    MI_OCT_3,   // 5C7C
    MI_OCT_4,   // 5C7D
    MI_OCT_5,   // 5C7E
    MI_OCT_6,   // 5C7F
    MI_OCT_7,   // 5C80
    MI_OCTD,    // 5C81
    MI_OCTU,    // 5C82

    MI_TRNS_N6,  // 5C83
    MI_TRNS_N5,  // 5C84
    MI_TRNS_N4,  // 5C85
    MI_TRNS_N3,  // 5C86
    MI_TRNS_N2,  // 5C87
    MI_TRNS_N1,  // 5C88
    MI_TRNS_0,   // 5C89
    MI_TRNS_1,   // 5C8A
    MI_TRNS_2,   // 5C8B
    MI_TRNS_3,   // 5C8C
    MI_TRNS_4,   // 5C8D
    MI_TRNS_5,   // 5C8E
    MI_TRNS_6,   // 5C8F
    MI_TRNSD,    // 5C90
    MI_TRNSU,    // 5C91

    MI_VEL_0,  // 5C92
    MI_VEL_1 = MI_VEL_0,  // 5C93, Or not? wtf VIA?
    MI_VEL_2,   // 5C94
    MI_VEL_3,   // 5C95
    MI_VEL_4,   // 5C96
    MI_VEL_5,   // 5C97
    MI_VEL_6,   // 5C98
    MI_VEL_7,   // 5C99
    MI_VEL_8,   // 5C9A
    MI_VEL_9,   // 5C9B
    MI_VEL_10,  // 5C9C
    MI_VELD,    // 5C9D
    MI_VELU,    // 5C9E

    MI_CH1,   // 5C9F
    MI_CH2,   // 5CA0
    MI_CH3,   // 5CA1
    MI_CH4,   // 5CA2
    MI_CH5,   // 5CA3
    MI_CH6,   // 5CA4
    MI_CH7,   // 5CA5
    MI_CH8,   // 5CA6
    MI_CH9,   // 5CA7
    MI_CH10,  // 5CA8
    MI_CH11,  // 5CA9
    MI_CH12,  // 5CAA
    MI_CH13,  // 5CAB
    MI_CH14,  // 5CAC
    MI_CH15,  // 5CAD
    MI_CH16,  // 5CAE
    MI_CHD,   // 5CAF
    MI_CHU,   // 5CB0

    MI_ALLOFF,  // 5CB1

    MI_SUS,   // 5CB2
    MI_PORT,  // 5CB3
    MI_SOST,  // 5CB4
    MI_SOFT,  // 5CB5
    MI_LEG,   // 5CB6

    MI_MOD,    // 5CB7
    MI_MODSD,  // 5CB8
    MI_MODSU,  // 5CB9

    MI_BENDD,  // 5CBA
    MI_BENDU,  // 5CBB

    // Backlight
    BL_ON,    // 5CBC
    BL_OFF,   // 5CBD
    BL_DEC,   // 5CBE
    BL_INC,   // 5CBF
    BL_TOGG,  // 5CC0
    BL_STEP,  // 5CC1
    BL_BRTG,  // 5CC2

    // RGB underglow/matrix
    RGB_TOG,            // 5CC3
    RGB_MOD,   // 5CC4 (RGB_MODE_FORWARD)
    RGB_MODE_REVERSE,   // 5CC5
    RGB_HUI,            // 5CC6
    RGB_HUD,            // 5CC7
    RGB_SAI,            // 5CC8
    RGB_SAD,            // 5CC9
    RGB_VAI,            // 5CCA
    RGB_VAD,            // 5CCB
    RGB_SPI,            // 5CCC
    RGB_SPD,            // 5CCD
    RGB_MODE_PLAIN,     // 5CCE
    RGB_MODE_BREATHE,   // 5CCF
    RGB_MODE_RAINBOW,   // 5CD0
    RGB_MODE_SWIRL,     // 5CD1
    RGB_MODE_SNAKE,     // 5CD2
    RGB_MODE_KNIGHT,    // 5CD3
    RGB_MODE_XMAS,      // 5CD4
    RGB_MODE_GRADIENT,  // 5CD5
    RGB_MODE_RGBTEST,   // 5CD6

    // Velocikey
    VLK_TOG,  // 5CD7

    // Space Cadet
    KC_LSPO,    // 5CD8
    KC_RSPC,    // 5CD9
    KC_SFTENT,  // 5CDA

    // Thermal Printer
    PRINT_ON,   // 5CDB
    PRINT_OFF,  // 5CDC

    // Bluetooth: output selection
    OUT_AUTO,  // 5CDD
    OUT_USB,   // 5CDE

    // Clear EEPROM
    EEPROM_RESET,  // 5CDF

    // Unicode
    UNICODE_MODE_FORWARD,  // 5CE0
    UNICODE_MODE_REVERSE,  // 5CE1
    UNICODE_MODE_MAC,      // 5CE2
    UNICODE_MODE_LNX,      // 5CE3
    UNICODE_MODE_WIN,      // 5CE4
    UNICODE_MODE_BSD,      // 5CE5
    UNICODE_MODE_WINC,     // 5CE6

    // Haptic
    HPT_ON,    // 5CE7
    HPT_OFF,   // 5CE8
    HPT_TOG,   // 5CE9
    HPT_RST,   // 5CEA
    HPT_FBK,   // 5CEB
    HPT_BUZ,   // 5CEC
    HPT_MODI,  // 5CED
    HPT_MODD,  // 5CEE
    HPT_CONT,  // 5CEF
    HPT_CONI,  // 5CF0
    HPT_COND,  // 5CF1
    HPT_DWLI,  // 5CF2
    HPT_DWLD,  // 5CF3

    // Space Cadet (continued)
    KC_LCPO,  // 5CF4
    KC_RCPC,  // 5CF5
    KC_LAPO,  // 5CF6
    KC_RAPC,  // 5CF7

    // Combos
    CMB_ON,   // 5CF8
    CMB_OFF,  // 5CF9
    CMB_TOG,  // 5CFA

    // Magic (continued)
    MAGIC_SWAP_LCTL_LGUI,    // 5CFB
    MAGIC_SWAP_RCTL_RGUI,    // 5CFC
    MAGIC_UNSWAP_LCTL_LGUI,  // 5CFD
    MAGIC_UNSWAP_RCTL_RGUI,  // 5CFE
    MAGIC_SWAP_CTL_GUI,      // 5CFF
    MAGIC_UNSWAP_CTL_GUI,    // 5D00
    MAGIC_TOGGLE_CTL_GUI,    // 5D01
    MAGIC_EE_HANDS_LEFT,     // 5D02
    MAGIC_EE_HANDS_RIGHT,    // 5D03

    // Dynamic Macros
    DYN_REC_START1,   // 5D04
    DYN_REC_START2,   // 5D05
    DYN_REC_STOP,     // 5D06
    DYN_MACRO_PLAY1,  // 5D07
    DYN_MACRO_PLAY2,  // 5D08

    // Joystick
    JS_BUTTON0,   // 5D09
    JS_BUTTON1,   // 5D0A
    JS_BUTTON2,   // 5D0B
    JS_BUTTON3,   // 5D0C
    JS_BUTTON4,   // 5D0D
    JS_BUTTON5,   // 5D0E
    JS_BUTTON6,   // 5D0F
    JS_BUTTON7,   // 5D10
    JS_BUTTON8,   // 5D11
    JS_BUTTON9,   // 5D12
    JS_BUTTON10,  // 5D13
    JS_BUTTON11,  // 5D14
    JS_BUTTON12,  // 5D15
    JS_BUTTON13,  // 5D16
    JS_BUTTON14,  // 5D17
    JS_BUTTON15,  // 5D18
    JS_BUTTON16,  // 5D19
    JS_BUTTON17,  // 5D1A
    JS_BUTTON18,  // 5D1B
    JS_BUTTON19,  // 5D1C
    JS_BUTTON20,  // 5D1D
    JS_BUTTON21,  // 5D1E
    JS_BUTTON22,  // 5D1F
    JS_BUTTON23,  // 5D20
    JS_BUTTON24,  // 5D21
    JS_BUTTON25,  // 5D22
    JS_BUTTON26,  // 5D23
    JS_BUTTON27,  // 5D24
    JS_BUTTON28,  // 5D25
    JS_BUTTON29,  // 5D26
    JS_BUTTON30,  // 5D27
    JS_BUTTON31,  // 5D28

    // Leader Key
    KC_LEAD,  // 5D29

    // Bluetooth: output selection (continued)
    OUT_BT,  // 5D2A

    // Lock Key
    KC_LOCK,  // 5D2B

    // Terminal
    TERM_ON,   // 5D2C
    TERM_OFF,  // 5D2D

    // Sequencer
    SQ_ON,   // 5D2E
    SQ_OFF,  // 5D2F
    SQ_TOG,  // 5D30

    SQ_TMPD,  // 5D31
    SQ_TMPU,  // 5D32

    SQ_RESD,  // 5D33
    SQ_RESU,  // 5D34

    SQ_SALL,  // 5D35
    SQ_SCLR,  // 5D36

    SEQUENCER_STEP_MIN,  // 5D37
    SEQUENCER_STEP_MAX = SEQUENCER_STEP_MIN + Sequencer.Steps,

    SEQUENCER_RESOLUTION_MIN,
    SEQUENCER_RESOLUTION_MAX = SEQUENCER_RESOLUTION_MIN + Sequencer.Resolutions,

    SEQUENCER_TRACK_MIN,
    SEQUENCER_TRACK_MAX = SEQUENCER_TRACK_MIN + Sequencer.Tracks,

// #define SQ_S(n) (n < SEQUENCER_STEPS ? SEQUENCER_STEP_MIN + n : KC_NO)
// #define SQ_R(n) (n < SEQUENCER_RESOLUTIONS ? SEQUENCER_RESOLUTION_MIN + n : KC_NO)
// #define SQ_T(n) (n < SEQUENCER_TRACKS ? SEQUENCER_TRACK_MIN + n : KC_NO)

    // One Shot
    ONESHOT_ENABLE,
    ONESHOT_DISABLE,
    ONESHOT_TOGGLE,

    // RGB underglow/matrix (continued)
    RGB_MODE_TWINKLE,

    VIA_MO13 = 0x5f10,
    VIA_FN_MO23,
    VIA_MACRO00,
    VIA_MACRO01,
    VIA_MACRO02,
    VIA_MACRO03,
    VIA_MACRO04,
    VIA_MACRO05,
    VIA_MACRO06,
    VIA_MACRO07,
    VIA_MACRO08,
    VIA_MACRO09,
    VIA_MACRO10,
    VIA_MACRO11,
    VIA_MACRO12,
    VIA_MACRO13,
    VIA_MACRO14,
    VIA_MACRO15,
    
    VIA_USER00 = 0x5F80,
    VIA_USER01,
    VIA_USER02,
    VIA_USER03,
    VIA_USER04,
    VIA_USER05,
    VIA_USER06,
    VIA_USER07,
    VIA_USER08,
    VIA_USER09,
    VIA_USER10,
    VIA_USER11,
    VIA_USER12,
    VIA_USER13,
    VIA_USER14,
    VIA_USER15,

    
    // (Reverse) Aliases
    // MAGIC_SWAP_LALT_LGUI = LAG_SWP,
    // MAGIC_UNSWAP_LALT_LGUI = LAG_NRM,
    // MAGIC_SWAP_RALT_RGUI = RAG_SWP,
    // MAGIC_UNSWAP_RALT_RGUI = RAG_NRM,
    // MAGIC_SWAP_ALT_GUI = AG_SWAP,
    // MAGIC_UNSWAP_ALT_GUI = AG_NORM,
    // MAGIC_TOGGLE_ALT_GUI = AG_TOGG,
    
    
    
    // Start of custom keycode range for keyboards and keymaps - always leave at the end
    SAFE_RANGE
    
    

    }
}