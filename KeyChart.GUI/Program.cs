﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using KeyChart.GUI.Platforms;
using KeyChart.GUI.Util;

namespace KeyChart.GUI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            // .AfterPlatformServicesSetup(ab =>
            // {
            //     AvaloniaLocator.CurrentMutable.Bind<IWindowingPlatform>()
            //         .ToConstant(new CustomWin32WindowingPlatform());
            // })
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.UseHotKeys()
                //.UseStatusIcon()
                .LogToTrace()
                .UseReactiveUI();
    }
}