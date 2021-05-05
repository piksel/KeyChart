using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KeyboardCheatSheet.Models
{
    class KeyboardConfig
    {
        [JsonPropertyName("version")]
        public int Version { get; set; }
        public string Notes { get; set; }
        public string Documentation { get; set; }
        public string Keyboard { get; set; }
        public string Keymap { get; set; }
        public string Layout { get; set; }
        public string[][] Layers { get; set; }
        public string Author { get; set; }
    }

}
