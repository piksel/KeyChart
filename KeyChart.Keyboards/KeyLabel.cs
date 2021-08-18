using System.Collections.Generic;
using System.Text.Json.Serialization;
using Piksel.TextSymbols;

#nullable enable

namespace KeyChart.Keyboards
{
    public struct KeyLabel
    {
        public KeyLabel(string text, bool symbol = false)
        {
            Text = text;
            Symbol = symbol;
            TargetLayer = null;
        }

        public KeyLabel(TextSymbol symbol)
        {
            Text = symbol;
            Symbol = true;
            TargetLayer = null;
        }

        [JsonIgnore]
        public string Text { get; set; }

        [JsonIgnore]
        public bool Symbol { get; set; }

        [JsonPropertyName("Sym")]
        public string? SymbolText
        {
            get => Symbol ? Text : null;
            set
            {
                if (value is null or "")
                {
                    Symbol = false;
                }
                else
                {
                    Symbol = true;
                    Text = value;
                }
            }
        }

        [JsonPropertyName("Txt")]
        public string? TextText
        {
            get => Symbol ? null : Text;
            set
            {
                if (value is null or "")
                {
                    Symbol = true;
                }
                else
                {
                    Symbol = false;
                    Text = value;
                }
            }
        }


        public static KeyLabel FromKeycode(string kc)
        {
            if (kc.StartsWith("KC_"))
            {
                kc = kc.Substring(3);
            }

            return keyNames.GetValueOrDefault(kc, new KeyLabel(kc));
        }
    

        private static readonly Dictionary<string, KeyLabel> keyNames = new ()
        {
            {"TRNS",KeyLabel.TextLabel(" ")},
            {"TAB", new( "", true)},
            {"ESC", new("Esc")},
            {"CTRL", new( "Ctrl")},
            {"ALT", new("Alt")},
            {"LGUI", new( "", true)},
            {"RGUI", new( "", true)},
            {"GUI", new( "", true)},

            {"LALT", new( "", true)},
            {"RALT", new( "", true)},
            {"BSPC", new( "", true)},

            {"GRV", new( "`")},
            {"SLSH", new( "/")},
            {"QUOT", new( "\"")},
            {"COMM", new( ",")},
            {"DOT", new( ".")},

            {"PGUP", new( "", true)},
            {"PGDN", new( "", true)},
            {"ENT", new( "", true)},
            {"LSFT", new( "", true)},
            {"RSFT", new(Fa6.Heart){TargetLayer = 3}},
            {"DEL", new("Del")},
            // {"", "", false)},

            {"EQL", new( "=")},

            { "RGB_TOG", new( "", true)},
          { "RGB_MOD", new( "", true)},
          { "RGB_HUI", new( "", true)},
          { "RGB_VAI", new( "", true)},
          { "RGB_SPI", new( "", true)},
          { "RESET", new("reset")},
          { "DEBUG", new("", true)},
          { "ANY(TERM_ON)", new("", true)},
          { "ANY(TERM_OFF)", new( "", true)},
          { "ANY(MU_MOD)", new("", true)},
          { "ANY(AU_ON)", new("", true)},
          { "ANY(AU_OFF)", new("", true)},
          { "AG_NORM", new("", true)},
          { "AG_SWAP", new("", true)},
          { "DF(0)", new("", true)},
          { "DF(1)", new("", true)},
          { "DF(2)", new("", true)},
          { "ANY(MUV_DE)", new("MUV-")},
          { "ANY(MUV_IN)", new("MUV+")},
          { "ANY(MU_ON)", new("", true)},
          { "ANY(MU_OFF)", new("", true)},
          { "ANY(MI_ON)", new("", true)},
          { "ANY(MI_OFF)", new("", true)},
          { "RALT(KC_Q)", new("Ä")},
          { "RALT(KC_W)", new("Å")},
          { "RALT(KC_P)", new("Ö")},
          { "RALT(KC_E)", new("É")},
          { "RALT(KC_M)", new("μ")},
          // TODO: Fix combo keys with KC_ 😩
          { "RALT(E)", new("É")},
          { "RALT(M)", new("μ")},
          { "MINS", new( "-")},
          { "LBRC", new("[")},
          { "RBRC", new("]")},

          { "MUTE", new(Fa6.VolumeOff)},
          { "VOLU", new(Fa6.VolumeHigh)},
          { "VOLD", new(Fa6.VolumeLow)},

          
          { "MO(1)", new(Fa6.ChevronsDown){TargetLayer = 1}},
          { "MO(2)", new(Fa6.ChevronsUp){TargetLayer = 2}},
          { "MO(3)", new("KB"){TargetLayer = 3}},
          // { "MO(3)", new("Lower", false)},
          // { "MO(4)", new("Raise", false)},
          // { "MO(5)", new("KB", false)},

          { "MPLY", new( "", true)},
          { "MPRV", new( "", true)},
          { "MNXT", new( "", true)},

          { "TILD", new( "~", true)},

          { "LPRN", new("(")},
          { "RPRN", new(")")},
          { "LCBR", new("{")},
          { "RCBR", new("}")},

          { "UNDS", new("_")},
          { "PLUS", new("+")},
          { "PIPE", new("|")},
          { "LSFT(KC_NUHS)", new("!US #")},
          { "LSFT(KC_NUBS)", new("!US \\")},
          { "BSLS", new("\\")},

          { "HOME", new("Home")},
          { "END", new("End")},
          { "SCLN", new(";")},

          { "UP", new(Fa6.ArrowUp)},
          { "LEFT", new(Fa6.ArrowLeft)},
          { "DOWN", new(Fa6.ArrowDown)},
          {"RGHT", new( Fa6.ArrowRight)},

          { "BL_STEP", new("Fn")},
          { "LCTL", new("Ctrl")},
          { "SPC", new("Space")},
          
          {"VIA_MACRO00", new (Fa6.Square1)},
          {"VIA_MACRO01", new (Fa6.Circle2)},
            {"VIA_MACRO02", new (Fa6.Circle3)},
            {"VIA_MACRO03", new (Fa6.Circle4)},
            {"VIA_MACRO04", new (Fa6.Circle5)},
            {"VIA_MACRO05", new (Fa6.Circle6)},
            {"VIA_MACRO06", new (Fa6.Circle7)},
            {"VIA_MACRO07", new (Fa6.Circle8)},
            {"VIA_MACRO08", new (Fa6.Circle9)},
            {"VIA_MACRO09", new (Fa6.Circle0)},
           
        };

        public byte? TargetLayer { get; set; }

        private static KeyLabel TextLabel(string text)
            => new ()
            {
                Symbol = false,
                Text = text,
            };
        
        private static KeyLabel SymbolLabel(string text)
            => new ()
            {
                Symbol = false,
                Text = text,
            };
    }
}
