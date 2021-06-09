using System;
using System.Reactive;
using ReactiveUI;

namespace KeyChart.GUI.Platforms
{
    public record PopupMenuItem(Action Selected, string Text)
    {
        public static PopupMenuItem Separator { get; } = new (() => { }, "--");
    }
}