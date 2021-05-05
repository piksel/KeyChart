using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using static System.Windows.DependencyProperty;

namespace KeyboardCheatSheet
{
    public partial class LayerKey
    {
        private static readonly Dictionary<string, (string, bool)> keyNames = new Dictionary<string, (string, bool)>()
        {
            {"TRNS",( " ",false)},
            {"TAB",( "", true)},
            {"ESC", ("Esc", false)},
            {"CTRL",( "Ctrl", false)},
            {"ALT", ("Alt", false)},
            {"LGUI",( "", true)},
            {"RGUI",( "", true)},
            {"GUI",( "", true)},
            
            {"LALT",( "", true)},
            {"RALT",( "", true)},
            {"BSPC",( "", true)},

            {"GRV",( "`", false)},
            {"SLSH",( "/", false)},
            {"QUOT",( "\"", false)},
            {"COMM",( ",", false)},
            {"DOT",( ".", false)},

            {"PGUP",( "", true)},
            {"PGDN",( "", true)},
            {"ENT",( "", true)},
            {"LSFT",( "", true)},
            {"DEL", ("Delete", false)},
            // {"", "", false)},

            {"EQL",( "=", false)},

            { "RGB_TOG",( "", true)},
          { "RGB_MOD",( "", true)},
          { "RGB_HUI",( "", true)},
          { "RGB_VAI",( "", true)},
          { "RGB_SPI",( "", true)},
          { "RESET", ("reset", false)},
          { "DEBUG", ("", true)},
          { "ANY(TERM_ON)", ("", true)},
          { "ANY(TERM_OFF)",( "", true)},
          { "ANY(MU_MOD)", ("", true)},
          { "ANY(AU_ON)", ("", true)},
          { "ANY(AU_OFF)", ("", true)},
          { "AG_NORM", ("", true)},
          { "AG_SWAP", ("", true)},
          { "DF(0)", ("", true)},
          { "DF(1)", ("", true)},
          { "DF(2)", ("", true)},
          { "ANY(MUV_DE)", ("MUV-", false)},
          { "ANY(MUV_IN)", ("MUV+", false)},
          { "ANY(MU_ON)", ("", true)},
          { "ANY(MU_OFF)", ("", true)},
          { "ANY(MI_ON)", ("", true)},
          { "ANY(MI_OFF)", ("", true)},
          { "RALT(KC_Q)", ("Ä", false)},
          { "RALT(KC_W)", ("Å", false)},
          { "RALT(KC_P)", ("Ö", false)},
          { "MINS",( "-", false)},
          { "LBRC", ("LBRC", false)},
          { "RBRC", ("RBRC", false)},

          { "MUTE",( "", true)},
          { "VOLU",( "", true)},
          { "VOLD",( "", true)},

          { "MO(3)", ("Lower", false)},
          { "MO(4)", ("Raise", false)},
          { "MO(5)", ("Keyboard", false)},

          { "MPLY",( "", true)},
          { "MPRV",( "", true)},
          { "MNXT",( "", true)},
          
          { "TILD",( "~", true)},

          { "LPRN", ("(", false)},
          { "RPRN", (")", false)},
          { "LCBR", ("{", false)},
          { "RCBR", ("}", false)},

          { "UNDS", ("_", false)},
          { "PLUS", ("+", false)},
          { "PIPE", ("|", false)},
          { "LSFT(KC_NUHS)", ("!US #", false)},
          { "LSFT(KC_NUBS)", ("!US \\", false)},
          { "BSLS", ("\\", false)},

          { "HOME", ("Home", false)},
          { "END", ("End", false)},
          { "SCLN", (";", false)},
          
          { "UP", ("", true)},
          { "LEFT", ("", true)},
          { "DOWN", ("", true)},
          {"RGHT",( "", true)},

          { "BL_STEP", ("Fn", false)},
          { "LCTL", ("Ctrl", false)},
          { "SPC", ("Space", false)},
        };

        public static readonly DependencyProperty KeyProperty = Register("Key", typeof(Key), typeof(LayerKey));

        public Key Key
        {
            get => (Key)GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        }
        
        public static readonly DependencyProperty LayerProperty = Register("Layer", typeof(Layer), typeof(LayerKey));

        public Layer Layer
        {
            get => (Layer)GetValue(LayerProperty);
            set => SetValue(LayerProperty, value);
        }

        public static readonly DependencyProperty TextProperty = Register("Text", typeof(string), typeof(LayerKey));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty SymbolProperty = Register("Symbol", typeof(bool), typeof(LayerKey));

        public bool Symbol
        {
            get => (bool)GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        public LayerKey()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == KeyProperty || e.Property == LayerProperty)
            {
                if (Key == null || Layer == null) return;
                if (Layer.Keys.Count <= Key.Index) return;


                var keyName = Layer.Keys[Key.Index];
                if (keyNames.ContainsKey(keyName))
                {
                    var (text, fa) = keyNames[keyName];
                    Text = text;
                    Symbol = fa;
                }
                else
                {
                    Debug.WriteLine($"  {{ \"{keyName}\", \"{keyName}\" }}, ");
                    Text = keyName;
                    Symbol = false;
                }
            }
        }

    }
}