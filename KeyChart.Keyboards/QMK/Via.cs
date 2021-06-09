namespace KeyChart.Keyboards.QMK
{
    public class Via
    {
        
    }

    public enum ViaCommand: byte
    {
        get_protocol_version                 = 0x01,  // always 0x01
        get_keyboard_value                   = 0x02,
        set_keyboard_value                   = 0x03,
        dynamic_keymap_get_keycode           = 0x04,
        dynamic_keymap_set_keycode           = 0x05,
        dynamic_keymap_reset                 = 0x06,
        lighting_set_value                   = 0x07,
        lighting_get_value                   = 0x08,
        lighting_save                        = 0x09,
        eeprom_reset                         = 0x0A,
        bootloader_jump                      = 0x0B,
        dynamic_keymap_macro_get_count       = 0x0C,
        dynamic_keymap_macro_get_buffer_size = 0x0D,
        dynamic_keymap_macro_get_buffer      = 0x0E,
        dynamic_keymap_macro_set_buffer      = 0x0F,
        dynamic_keymap_macro_reset           = 0x10,
        dynamic_keymap_get_layer_count       = 0x11,
        dynamic_keymap_get_buffer            = 0x12,
        dynamic_keymap_set_buffer            = 0x13,
        unhandled                            = 0xFF,
    }
}