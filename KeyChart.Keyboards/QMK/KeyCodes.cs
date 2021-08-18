using System;
using System.Collections.Generic;
using System.Text;

namespace KeyChart.Keyboards.QMK
{
    public static class KeyCodes
    {
        public static LayerKey? AsLayerKey(string keyCode)
        {
            const StringComparison ct = StringComparison.InvariantCultureIgnoreCase;

            if (keyCode.StartsWith("KC_", ct)) return null;

            var parenOpen = keyCode.IndexOf('(');
            var parenClose = keyCode.IndexOf(')');

            if (parenClose <= 0 || parenOpen <= 1) return null;

            var args = keyCode.Substring(parenOpen + 1, (parenClose - parenOpen)-1).Split(',');
            if (args.Length < 1 || !int.TryParse(args[0], out var layer)) return null;

            return keyCode.Substring(0, parenOpen) switch
            {
                "DF" => new LayerKey(layer, LayerKeyMode.SetDefault),
                "MO" => new LayerKey(layer, LayerKeyMode.MomentaryActivation),
                "LM" => new LayerKey(layer, LayerKeyMode.MomentaryActivation,
                    (LayerModifier) Enum.Parse(typeof(LayerModifier), args[1], ignoreCase: true)),
                "OSL" => new LayerKey(layer, LayerKeyMode.OneShot),
                "TG" => new LayerKey(layer, LayerKeyMode.Toggle),
                "TO" => new LayerKey(layer, LayerKeyMode.Replace),
                "TT" => new LayerKey(layer, LayerKeyMode.MomentaryActivation), // TODO: Add correct type
                _ => null!,
            };
        }

        public static string? GetString(KeyCode keyCode)
        {
            if (keyCode is >= KeyCode.QK_MODS and < KeyCode.QK_MODS_MAX)
            {
                var lowKey = GetString(keyCode & KeyCode.QK_BASIC_MAX);
                
                return (KeyCode)((ushort)keyCode & 0xff00) switch
                {
                    KeyCode.QK_LCTL => $"LCTL({lowKey})",
                    KeyCode.QK_LSFT => (keyCode & KeyCode.QK_BASIC_MAX) switch 
                    {
                        KeyCode.GRAVE => "TILD",  // ~
                        KeyCode.D1 => "EXLM",  // !
                        KeyCode.D2 => "AT",  // @
                        KeyCode.D3 => "HASH",  // #
                        KeyCode.D4 => "DLR",  // $
                        KeyCode.D5 => "PERC",  // %
                        KeyCode.D6 => "CIRC",  // ^
                        KeyCode.D7 => "AMPR",  // &
                        KeyCode.D8 => "ASTR",  // *
                        KeyCode.D9 => "LPRN",  // (
                        KeyCode.D0 => "RPRN",  // )
                        KeyCode.MINUS => "UNDS",  // _
                        KeyCode.EQUAL => "PLUS",  // +
                        KeyCode.LBRACKET => "LCBR",  // {
                        KeyCode.RBRACKET => "RCBR",  // }
                        KeyCode.COMMA => "LABK",  // <
                        KeyCode.DOT => "RABK",  // >
                        KeyCode.SCOLON => "COLN",  // :
                        KeyCode.BSLASH => "PIPE",  // |
                        //KeyCode.COMMA => "LT",  // <
                        //KeyCode.DOT => "GT",  // >
                        KeyCode.SLASH => "QUES",  // ?
                        KeyCode.QUOTE => "DQT",  // "
                        _ => $"LSFT({lowKey})",
                    },
                    KeyCode.QK_LALT => $"LALT({lowKey})",
                    KeyCode.QK_LGUI => $"LGUI({lowKey})",
                    KeyCode.QK_RCTL => $"RCTL({lowKey})",
                    KeyCode.QK_RSFT => $"RSFT({lowKey})",
                    KeyCode.QK_RALT => $"RALT({lowKey})",
                    KeyCode.QK_RGUI => $"RGUI({lowKey})",
                    _ => $"?{keyCode:X}",
                };
            }

            return keyCode switch
            {
                KeyCode.ROLL_OVER => "TRNS",
                KeyCode.GRAVE => "GRV",
                KeyCode.BSPACE => "BSPC",
                KeyCode.BSLASH => "BSLS",
                KeyCode.ESCAPE => "ESC",
                KeyCode.SCOLON => "SCLN",
                KeyCode.QUOTE => "QUOT",
                KeyCode.LSHIFT => "LSFT",
                KeyCode.COMMA => "COMM",
                KeyCode.SLASH => "SLSH",
                KeyCode.ENTER => "ENT",
                KeyCode.RSHIFT => "RSFT",
                KeyCode.LCTRL => "LCTL",
                KeyCode.SPACE => "SPC",
                KeyCode.RIGHT => "RGHT",
                KeyCode.LBRACKET => "LBRC",
                KeyCode.RBRACKET => "RBRC",
                KeyCode.DELETE => "DEL",
                KeyCode.PGDOWN => "PGDN",
                KeyCode.MINUS => "MINS",
                KeyCode.EQUAL => "EQL",
                KeyCode.AUDIO_MUTE => "MUTE",
                KeyCode.AUDIO_VOL_UP => "VOLU",
                KeyCode.AUDIO_VOL_DOWN => "VOLD",
                KeyCode.MEDIA_NEXT_TRACK => "MNXT",
                KeyCode.MEDIA_PLAY_PAUSE => "MPLY",
                KeyCode.MEDIA_PREV_TRACK => "MPRV",
                KeyCode.NONUS_HASH => "NUHS",
                KeyCode.NONUS_BSLASH => "NUBS",
                // Momentary switch to layer
                >= KeyCode.QK_MOMENTARY and < KeyCode.QK_MOMENTARY_MAX => $"MO({keyCode - KeyCode.QK_MOMENTARY:D})",
                // Set default layer
                >= KeyCode.QK_DEF_LAYER and < KeyCode.QK_DEF_LAYER_MAX => $"DF({keyCode - KeyCode.QK_DEF_LAYER:D})",
                // Firmware keys
                //>= (KeyCode)0x5c02 and <= (KeyCode)0x5cff => $"FW(0x{(int)keyCode & 0xff:x2})",
                >= KeyCode.D1 and <= KeyCode.D0 => $"{keyCode:g}"[1..],
                >= KeyCode.AU_ON and < KeyCode.RGB_TOG => $"ANY({keyCode:g})",
                _ => Enum.IsDefined(keyCode) ? $"{keyCode:g}" : null,
            };
        }

        public static bool Matches(string kcA, string? kcB)
            => kcB is not null && string.Equals(NormalizeKeyName(kcA),NormalizeKeyName(kcB), 
                   StringComparison.InvariantCultureIgnoreCase);

        public static string NormalizeKeyName(string key)
        {
            //if (key.StartsWith("KC_")) key = key[3..];
            key = key.Replace("KC_", "");

            return key;
        }

        public static bool Matches(string kcA, KeyCode kcB)
            => Matches(kcA, GetString(kcB));
    }

    public class LayerKey
    {
        public LayerKey(int layer, LayerKeyMode mode, LayerModifier modifier = LayerModifier.None)
        {
            Layer = layer;
            Mode = mode;
            Modifier = modifier;
        }

        public int Layer { get; }
        public LayerKeyMode Mode { get; }
        public LayerModifier Modifier { get; }

    }

    public enum LayerModifier
    {
        None,
        MOD_LCTL, 
        MOD_LSFT, 
        MOD_LALT, 
        MOD_LGUI
    }

    [Flags]
    public enum Modifier
    {
        None = 0,
        // LCTL(kc)	C(kc)	Hold Left Control and press kc
        LeftCtrl = 1 << 0,
        // LSFT(kc)	S(kc)	Hold Left Shift and press kc
        LeftShift = 1 << 1,
        // LALT(kc)	A(kc), LOPT(kc)	Hold Left Alt and press kc
        LeftAlt = 1 << 2,
        // LGUI(kc)	G(kc), LCMD(kc), LWIN(kc)	Hold Left GUI and press kc
        LeftGui = 1 << 3,
        // RCTL(kc)		Hold Right Control and press kc
        RightCtrl = 1 << 4,
        // RSFT(kc)		Hold Right Shift and press kc
        RightShift = 1 << 5,
        // RALT(kc)	ROPT(kc), ALGR(kc)	Hold Right Alt and press kc
        RightAlt = 1 << 6,
        // RGUI(kc)	RCMD(kc), LWIN(kc)	Hold Right GUI and press kc
        RightGui = 1 << 7,
        // SGUI(kc)	SCMD(kc), SWIN(kc)	Hold Left Shift and GUI and press kc
        LeftShiftGui = LeftShift | LeftGui,
        // LCA(kc)		Hold Left Control and Alt and press kc
        LeftCtrlAlt = LeftCtrl | LeftAlt,
        // LSA(kc)		Hold Left Shift and Left Alt and press kc
        LeftShiftAlt = LeftShift | LeftAlt,
        // RSA(kc)	SAGR(kc)	Hold Right Shift and Right Alt (AltGr) and press kc
        RightShiftAlt = RightShift | RightAlt,
        // RCS(kc)		Hold Right Control and Right Shift and press kc
        RightCtrlShift = RightCtrl | RightShift,
        // LCAG(kc)		Hold Left Control, Alt and GUI and press kc
        // MEH(kc)		Hold Left Control, Shift and Alt and press kc
        // HYPR(kc)		Hold Left Control, Shift, Alt and GUI and press kc




    }

    public enum LayerKeyMode
    {
        // DF
        SetDefault,

        // MO, LM, LT
        MomentaryActivation,

        // TG
        Toggle,

        // TO
        Replace,

        // OSL
        OneShot,
    }
}
