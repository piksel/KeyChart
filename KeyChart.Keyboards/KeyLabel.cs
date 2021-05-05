using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;


namespace KeyChart.Keyboards
{
    public struct KeyLabel
    {
        public KeyLabel(string text, bool symbol)
        {
            Text = text;
            Symbol = symbol;
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
                if (string.IsNullOrEmpty(value))
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
                if (string.IsNullOrEmpty(value))
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

            if (keyNames.ContainsKey(kc))
            {
                return keyNames[kc];
            }
            else
            {
                // Debug.WriteLine($" Missing Keycode: \"{kc}\" ");
                return new(kc, false);
            }
        }
    

        private static readonly Dictionary<string, KeyLabel> keyNames = new ()
        {
            {"TRNS",new( " ",false)},
            {"TAB", new( "", true)},
            {"ESC", new("Esc", false)},
            {"CTRL", new( "Ctrl", false)},
            {"ALT", new("Alt", false)},
            {"LGUI", new( "", true)},
            {"RGUI", new( "", true)},
            {"GUI", new( "", true)},

            {"LALT", new( "", true)},
            {"RALT", new( "", true)},
            {"BSPC", new( "", true)},

            {"GRV", new( "`", false)},
            {"SLSH", new( "/", false)},
            {"QUOT", new( "\"", false)},
            {"COMM", new( ",", false)},
            {"DOT", new( ".", false)},

            {"PGUP", new( "", true)},
            {"PGDN", new( "", true)},
            {"ENT", new( "", true)},
            {"LSFT", new( "", true)},
            {"DEL", new("Del", false)},
            // {"", "", false)},

            {"EQL", new( "=", false)},

            { "RGB_TOG", new( "", true)},
          { "RGB_MOD", new( "", true)},
          { "RGB_HUI", new( "", true)},
          { "RGB_VAI", new( "", true)},
          { "RGB_SPI", new( "", true)},
          { "RESET", new("reset", false)},
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
          { "ANY(MUV_DE)", new("MUV-", false)},
          { "ANY(MUV_IN)", new("MUV+", false)},
          { "ANY(MU_ON)", new("", true)},
          { "ANY(MU_OFF)", new("", true)},
          { "ANY(MI_ON)", new("", true)},
          { "ANY(MI_OFF)", new("", true)},
          { "RALT(KC_Q)", new("Ä", false)},
          { "RALT(KC_W)", new("Å", false)},
          { "RALT(KC_P)", new("Ö", false)},
          { "MINS", new( "-", false)},
          { "LBRC", new("LBRC", false)},
          { "RBRC", new("RBRC", false)},

          { "MUTE", new( "", true)},
          { "VOLU", new( "", true)},
          { "VOLD", new( "", true)},

          { "MO(3)", new("Lower", false)},
          { "MO(4)", new("Raise", false)},
          { "MO(5)", new("KB", false)},

          { "MPLY", new( "", true)},
          { "MPRV", new( "", true)},
          { "MNXT", new( "", true)},

          { "TILD", new( "~", true)},

          { "LPRN", new("(", false)},
          { "RPRN", new(")", false)},
          { "LCBR", new("{", false)},
          { "RCBR", new("}", false)},

          { "UNDS", new("_", false)},
          { "PLUS", new("+", false)},
          { "PIPE", new("|", false)},
          { "LSFT(KC_NUHS)", new("!US #", false)},
          { "LSFT(KC_NUBS)", new("!US \\", false)},
          { "BSLS", new("\\", false)},

          { "HOME", new("Home", false)},
          { "END", new("End", false)},
          { "SCLN", new(";", false)},

          { "UP", new("", true)},
          { "LEFT", new("", true)},
          { "DOWN", new("", true)},
          {"RGHT", new( "", true)},

          { "BL_STEP", new("Fn", false)},
          { "LCTL", new("Ctrl", false)},
          { "SPC", new("Space", false)},
        };
    }
}
