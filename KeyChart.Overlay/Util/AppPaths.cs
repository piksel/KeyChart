using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Nuke.Common.IO;
using SpecialFolder = System.Environment.SpecialFolder;

namespace KeyChart.Avalonia.Util
{
    public static class KnownPaths
    {
        public static AbsolutePath SpecialPath(SpecialFolder specialFolder)
            => (AbsolutePath)Environment.GetFolderPath(specialFolder);
        
        public static AbsolutePath LocalData => SpecialPath(SpecialFolder.LocalApplicationData);
        public static AbsolutePath ConfigData => SpecialPath(SpecialFolder.ApplicationData);
        public static AbsolutePath UserHome => SpecialPath(SpecialFolder.UserProfile);
        public static AbsolutePath Library => UserHome / "Library"  / "Application Support";
        public static AbsolutePath Cache => SpecialPath(SpecialFolder.InternetCache);
    }
    
    public record AppPaths(AbsolutePath BinaryPath, AbsolutePath DataPath, AbsolutePath ConfigPath, AbsolutePath CachePath)
    {
        private static DirectoryInfo GetDirectory(AbsolutePath path)
        {
            var dir = new DirectoryInfo(path);
            dir.Create();
            return dir;
        }

        public DirectoryInfo BinaryDir => GetDirectory(BinaryPath);
        public DirectoryInfo DataDir => GetDirectory(DataPath);
        public DirectoryInfo ConfigDir => GetDirectory(ConfigPath);
        public DirectoryInfo CacheDir => GetDirectory(CachePath);

        public static AppPaths FromAppMeta<TApp>()
        {
            if (typeof(TApp).Assembly is not { } assembly) 
                throw new InvalidOperationException("Could not retrieve assembly information");

            var organization = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

            if (string.IsNullOrEmpty(organization))
            {
                organization = typeof(TApp).Namespace ?? string.Empty;
            }

            if (assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product is not {} appName)
            {
                appName = assembly.GetName().Name?.Split('.').Last() ?? "Unknown";
            }

            return ForApp(appName.ToLowerInvariant(), assembly.Location, organization);

        }

        public static AppPaths ForApp<TApp>(string appName, string organization) 
            => ForApp((RelativePath)appName, typeof(TApp).Assembly.Location, organization);
        
        public static AppPaths ForApp(string appName, string binaryPath) 
            => ForApp((RelativePath)appName, binaryPath);
        
        public static AppPaths ForApp(string appName, string binaryPath, string organization) 
            => ForApp((RelativePath)organization / appName, binaryPath);

        public static AppPaths ForApp(RelativePath appPath, string binaryPath)
        {
            var binary = (AbsolutePath)(Path.GetDirectoryName(binaryPath) ?? throw new InvalidOperationException());
            AbsolutePath data, config, cache;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                data = KnownPaths.Library / appPath;
                config = data;
                cache = KnownPaths.Cache / appPath;
            }
            else
            {
                // Use the same settings for:
                // RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                // RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                data = KnownPaths.LocalData / appPath;
                config = KnownPaths.ConfigData /  appPath;
                cache = data / "cache";
            }

            return new AppPaths(binary, data, config, cache);
        }
    }
}