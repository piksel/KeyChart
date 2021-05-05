using System;
using System.Collections.Generic;
using System.Text;

namespace KeyChart.Keyboards.QMK
{
    public static class KeyCodes
    {
        public static bool IsLayerKey(string keyCode, out LayerKey layerKey)
        {
            const StringComparison ct = StringComparison.InvariantCultureIgnoreCase;
            layerKey = null;

            if (keyCode.StartsWith("KC_", ct)) return false;

            var parenOpen = keyCode.IndexOf('(');
            var parenClose = keyCode.IndexOf(')');

            if (parenClose <= 0 || parenOpen <= 1) return false;

            var args = keyCode.Substring(parenOpen + 1, (parenClose - parenOpen)-1).Split(',');
            if (args.Length < 1 || !int.TryParse(args[0], out int layer)) return false;

            layerKey = keyCode.Substring(0, parenOpen) switch
            {
                "DF" => new LayerKey(layer, LayerKeyMode.SetDefault),
                "MO" => new LayerKey(layer, LayerKeyMode.MomentaryActivation),
                "LM" => new LayerKey(layer, LayerKeyMode.MomentaryActivation,
                    (LayerModifier) Enum.Parse(typeof(LayerModifier), args[1], true)),
                "OSL" => new LayerKey(layer, LayerKeyMode.OneShot),
                "TG" => new LayerKey(layer, LayerKeyMode.Toggle),
                "TO" => new LayerKey(layer, LayerKeyMode.Replace),
                "TT" => new LayerKey(layer, LayerKeyMode.MomentaryActivation), // TODO: Add correct type
                _ => null
            };

            return layerKey != null;
        }
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
