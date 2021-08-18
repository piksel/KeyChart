using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

#nullable enable

namespace KeyChart.Keyboards.QMK
{
    public class KeyboardInfo
    {
        [JsonPropertyName("keyboard_name")]
        public string KeyboardName { get; set; } = "";
        public string? Url { get; set; }
        public string? Maintainer { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Dictionary<string, KeyLayout> Layouts { get; set; } = new();


        [JsonPropertyName("keyboard_folder")]
        public string? KeyboardFolder { get; set; }

        public Dictionary<string, KeyMap> Keymaps { get; set; } = new();

        [JsonPropertyName("layout_aliases")]
        public Dictionary<string, string> LayoutAliases { get; set; } = new();


        public int Debounce { get; set; }
        public Usb? USB { get; set; }

        [JsonPropertyName("diode_direction")]
        public string? DiodeDirection { get; set; }
        public string? Manufacturer { get; set; }
        public Rgblight? RgbLight { get; set; }

        [JsonPropertyName("matrix_size")]
        public MatrixSize? MatrixSize { get; set; }

        [JsonPropertyName("matrix_pins")]
        public MatrixPins? MatrixPins { get; set; }
        public string? Processor { get; set; }
        
        [JsonPropertyName("processor_type")]
        public string? ProcessorType { get; set; }

        public string? Platform { get; set; }
        public string? Protocol { get; set; }
        public string? Bootloader { get; set; }

        [JsonPropertyName("community_layouts")]
        public string[] CommunityLayouts { get; set; } = Array.Empty<string>();

        [JsonPropertyName("config_h_features")]
        public Features? ConfigFeatures { get; set; }
        public Features? Features { get; set; }

        public string ProcessorInfo => $"{Processor ?? "Unknown"} ({ProcessorType ?? "Unknown"})";
    }

    public class KeyLayout
    {
        [JsonPropertyName("key_count")]
        public int KeyCount { get; set; }
        public Key[] Layout { get; set; } = Array.Empty<Key>();
        public string? Filename { get; set; }

        [JsonPropertyName("c_macro")]
        public bool CMacro { get; set; }
    }

    public class Key
    {
        // private static readonly ImmutableArray<byte> DefaultMatrix = ImmutableArray.Create<byte>(0, 0);
        
        public float X { get; set; }
        public float Y { get; set; }

        [DefaultValue(1)]
        public float W { get; set; } = 1;

        public string? Label { get; set; }
        
        [JsonPropertyName("matrix")]
        public short[] MatrixArray { get; set; } = Array.Empty<short>();

        [JsonIgnore]
        public (byte X, byte Y) Matrix =>
            MatrixArray?.Length == 2
                ? ((byte)MatrixArray[1], (byte)MatrixArray[0])
                : (byte.MaxValue, byte.MaxValue);
    }

    public class KeyboardQueryResult
    {
        [JsonPropertyName("git_hash")]
        public string? GitHash { get; set; }

        [JsonPropertyName("last_updated")]
        public string? LastUpdated { get; set; }

        [JsonPropertyName("keyboards")]
        public Dictionary<string, KeyboardInfo> Keyboards { get; set; } = new();
    }


    public class Usb
    {
        [JsonPropertyName("device_ver")]
        public string? DeviceVer { get; set; }

        [JsonPropertyName("pid")]

        public string? ProductId { get; set; }

        [JsonPropertyName("vid")]

        public string? VendorId { get; set; }
    }

    public class Rgblight
    {
        public string? Pin { get; set; }

        [JsonPropertyName("led_count")]
        public int LedCount { get; set; }

        public Animations? Animations { get; set; }

        [JsonPropertyName("max_brightness")]
        public int MaxBrightness { get; set; }

        [JsonPropertyName("hue_steps")]
        public int HueSteps { get; set; }

        [JsonPropertyName("saturation_steps")]
        public int SaturationSteps { get; set; }

        [JsonPropertyName("brightness_steps")]
        public int BrightnessSteps { get; set; }
        public bool Sleep { get; set; }
    }

    public class Animations
    {
        public bool All { get; set; }
    }

    public class MatrixSize
    {
        public int Cols { get; set; }
        public int Rows { get; set; }
    }

    public class MatrixPins
    {
        public string[] Cols { get; set; } = Array.Empty<string>();
        public string[] Rows { get; set; } = Array.Empty<string>();
    }

    public static class FeatureFlags
    {
        public const int SleepLed = 0;
        public const int Nkro = 1;
        public const int BackLight = 2;
        public const int RgbLight = 3;
        public const int Midi = 4;
        public const int Unicode = 5;
        public const int Bluetooth = 6;
        public const int Audio = 7;
        public const int BootmagicLite = 8;
        public const int Command = 9;
        public const int Console = 10;
        public const int ExtraKey = 11;
        public const int MouseKey = 12;
    }

    public record FlagItem
    {
        public FlagItem(string name, bool enabled = false)
        {
            Name = name;
            Enabled = enabled;
        }
        public string Name { get; }
        public bool Enabled { get; set; }
    }


    public class Features
    {
        [JsonIgnore]
        public IReadOnlyList<FlagItem> Values => new ReadOnlyCollection<FlagItem>(Flags);

        public FlagItem[] Flags = {
            new(nameof(SleepLed)),
            new(nameof(Nkro)),
            new(nameof(BackLight)),
            new(nameof(RgbLight)),
            new(nameof(Midi)),
            new(nameof(Unicode)),
            new(nameof(Bluetooth)),
            new(nameof(Audio)),
            new(nameof(BootmagicLite)),
            new(nameof(Command)),
            new(nameof(Console)),
            new(nameof(ExtraKey)),
            new(nameof(MouseKey)),
        };

        [JsonPropertyName("sleep_led")]
        public bool SleepLed { get => Flags[FeatureFlags.SleepLed].Enabled; set => Flags[FeatureFlags.SleepLed].Enabled = value; }
        public bool Nkro { get => Flags[FeatureFlags.Nkro].Enabled; set => Flags[FeatureFlags.Nkro].Enabled = value; }
        public bool BackLight { get => Flags[FeatureFlags.BackLight].Enabled; set => Flags[FeatureFlags.BackLight].Enabled = value; }
        public bool RgbLight { get => Flags[FeatureFlags.RgbLight].Enabled; set => Flags[FeatureFlags.RgbLight].Enabled = value; }
        public bool Midi { get => Flags[FeatureFlags.Midi].Enabled; set => Flags[FeatureFlags.Midi].Enabled = value; }
        public bool Unicode { get => Flags[FeatureFlags.Unicode].Enabled; set => Flags[FeatureFlags.Unicode].Enabled = value; }
        public bool Bluetooth { get => Flags[FeatureFlags.Bluetooth].Enabled; set => Flags[FeatureFlags.Bluetooth].Enabled = value; }
        public bool Audio { get => Flags[FeatureFlags.Audio].Enabled; set => Flags[FeatureFlags.Audio].Enabled = value; }
        
        [JsonPropertyName("bootmagic_lite")]
        public bool BootmagicLite { get => Flags[FeatureFlags.BootmagicLite].Enabled; set => Flags[FeatureFlags.BootmagicLite].Enabled = value; }
        public bool Command { get => Flags[FeatureFlags.Command].Enabled; set => Flags[FeatureFlags.Command].Enabled = value; }
        public bool Console { get => Flags[FeatureFlags.Console].Enabled; set => Flags[FeatureFlags.Console].Enabled = value; }
        public bool ExtraKey { get => Flags[FeatureFlags.ExtraKey].Enabled; set => Flags[FeatureFlags.ExtraKey].Enabled = value; }
        public bool MouseKey { get => Flags[FeatureFlags.MouseKey].Enabled; set => Flags[FeatureFlags.MouseKey].Enabled = value; }

        public override string ToString() 
            => string.Join(", ", Flags.Where(kv => kv.Enabled).Select(kv => kv.Name));

        public static Features All =>
            new()
            {
                SleepLed = true,
                Nkro = true,
                BackLight = true,
                RgbLight = true,
                Midi = true,
                Unicode = true,
                Bluetooth = true,
                Audio = true,
                BootmagicLite = true,
                Command = true,
                Console = true,
                ExtraKey = true,
                MouseKey = true
            };
    }

}
#nullable restore