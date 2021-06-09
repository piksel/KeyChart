using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using KeyChart.Keyboards;
using Microsoft.Toolkit.Uwp.Helpers;
using QMK = KeyChart.Keyboards.QMK;
using QmkClient = KeyChart.Keyboards.QMK.QmkClient;


namespace KeyChart
{
#nullable enable
    public static class KeyboardHelper
    {
        private const string QmkKeyboardsKey = "qmk-keyboards.json";
        public const string DefaultLayout = "LAYOUT";

        private static readonly LocalObjectStorageHelper Store = new(new JsonObjectSerializer());
        private static StorageFolder Root => ApplicationData.Current.LocalFolder;

        public static async Task<IReadOnlyCollection<string>> ListKeyboards(bool force = false)
        {
            if (!force && await Store.FileExistsAsync(QmkKeyboardsKey))
            {
                return await Store.ReadFileAsync<string[]>(QmkKeyboardsKey);
            }

            var keyboards = await QmkClient.Keyboards();
            if (keyboards is null)
            {
                return Array.Empty<string>();
            }

            await Store.SaveFileAsync(QmkKeyboardsKey, keyboards);

            return keyboards;
        }
        //

        public static async Task<IReadOnlyList<StorageFile>> GetCustomKeyMapFiles(string keyboard, string layout = "LAYOUT")
        {
            if (await Root.TryGetItemAsync(KeyToPath($"{keyboard}/keymaps/{layout}")) is not StorageFolder folder)
            {
                return Array.Empty<StorageFile>();
            }

            return await folder.GetFilesAsync(CommonFileQuery.DefaultQuery);
        }

        public static async Task<IReadOnlyCollection<string>> ListCustomKeyMaps(string keyboard, string layout = "LAYOUT")
        {
            return (await GetCustomKeyMapFiles(keyboard, layout)).Select(f => f.Name).ToImmutableArray();

        }

        public static async Task<QMK.KeyboardInfo?> GetKeyboardInfo(string keyboardId, bool force = false)
        {
            var keyboardKey = KeyToPath($"keyboards/{keyboardId}_info.json");

            if (!force && await Store.FileExistsAsync(keyboardKey))
            {
                return await Store.ReadFileAsync<QMK.KeyboardInfo>(keyboardKey);
            }

            var keyboardInfo = await QmkClient.KeyboardInfo(keyboardId);
            if (keyboardInfo is {})
            {
                await Store.SaveFileAsync(keyboardKey, keyboardInfo);
            }

            return keyboardInfo;
        }

        private static string KeyToPath(string fileKey) => fileKey.Replace('/', Path.DirectorySeparatorChar);
        private static string KeyboardSourceToPath(string source) => KeyToPath($"models/{source}");

        public static void MarkBeginLoadingKeyboard(string keyboard) 
            => Store.Save("LoadingKeyboardStarted", keyboard);

        public static void MarkLoadingKeyboardDone()
            => Store.Save("LoadingKeyboardStarted", (string?)null);

        public static string? KeyboardFailedToLoad() 
            => Store.Read<string?>("LoadingKeyboardStarted", null);

        public static async ValueTask AddCustomKeyMap(StorageFile file)
        {
            var hash = await FileSha256(file);

            QMK.KeyMap? keymap = null;
            using (var stream = await file.OpenStreamForReadAsync())
            {
                keymap = await QmkClient.ParseCustomKeymap(stream);
            }

            if (keymap is null) throw new Exception("Could not parse keymap file");

            keymap.Source = "Custom";

            var keyboard = keymap.Keyboard ?? throw new Exception("Keyboard missing from keymap");
            var layout = keymap.Layout ?? "LAYOUT";

            var key = $"{keyboard}/keymaps/{layout}/{keymap.Keymap ?? "Custom"}_{keymap.Version}_{hash}.json";


            await Store.SaveFileAsync(KeyToPath(key), keymap);
        }

        private static async Task<string> FileSha256(StorageFile file) 
            => string.Concat(SHA256.Create().ComputeHash(await file.OpenStreamForReadAsync()).Select(b => $"{b:x2}"));

        public static async IAsyncEnumerable<QMK.KeyMap> ListKeyMaps(string keyboardId, string layout = "LAYOUT")
        {
            var defaultKey = KeyToPath($"{keyboardId}/keymaps/default.json");

            QMK.KeyMap? defaultKeyMap;
            if (await Store.FileExistsAsync(defaultKey))
            {
                defaultKeyMap = await Store.ReadFileAsync<QMK.KeyMap>(defaultKey);
            }
            else
            {
                defaultKeyMap = await QmkClient.DefaultKeymap(keyboardId);
                if (defaultKeyMap is { })
                {
                    defaultKeyMap.Source = "default, config.qmk.fm";
                    await Store.SaveFileAsync(defaultKey, defaultKeyMap);
                }
            }

            if (defaultKeyMap is { } && defaultKeyMap.Layout == layout)
            {
                yield return defaultKeyMap;
            }

            foreach (var keymapId in await ListCustomKeyMaps(keyboardId, layout))
            {
                var key = KeyToPath($"{keyboardId}/keymaps/{layout}/{keymapId}");
                var keymap = await Store.ReadFileAsync<QMK.KeyMap>(key);
                if (keymap is { })
                {
                    yield return keymap;
                }
            }
        }

        public static async Task AddKeyboardModel(KeyboardModel kbdModel)
        {
            var guid = Guid.NewGuid();
            var key = $"models/{guid}.json";
            await Store.SaveFileAsync(KeyToPath(key), kbdModel);
        }

        public static async IAsyncEnumerable<KeyboardModel> ListKeyboardModels()
        {
            var root = ApplicationData.Current.LocalFolder;
            if (await root.TryGetItemAsync("models") is not StorageFolder folder)
            {
                yield break;
            }

            foreach (var file in await folder.GetFilesAsync(CommonFileQuery.DefaultQuery))
            {
                var model = await GetKeyboardModel(file.Name);
                if (model != null)
                {
                    yield return model;
                }
            }
        }

        public static async Task<KeyboardModel?> GetKeyboardModel()
            => GetSelectedKeyboardModel() is { } source ? await GetKeyboardModel(source) : null;
        public static async Task<KeyboardModel?> GetKeyboardModel(string source)
        {
            var model = await Store.ReadFileAsync<KeyboardModel>(KeyboardSourceToPath(source));
            model.Source = source;
            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = Path.GetFileNameWithoutExtension(source);
            }

            await model.BuildKeyLayout();

            return model;
        }

        public static async Task<Size?> GetSelectedKeyboardSize()
        {
            if (GetSelectedKeyboardModel() is not { } source) return null;

            var model = await Store.ReadFileAsync<KeyboardModel>(KeyboardSourceToPath(source));
            var layout = model.Info.Layouts[model.KeyMap.Layout ?? "LAYOUT"].Layout;

            var maxRight = 0f;
            var maxBottom = 0f;
            foreach (var key in layout)
            {
                var bounds = Key.GetKeyBounds(key.X, key.Y, key.W);
                maxRight = Math.Max(maxRight, bounds.Right);
                maxBottom = Math.Max(maxBottom, bounds.Bottom);
            }

            return new Size(maxRight, maxBottom);
        }

        public static void SetKeyboardModel(KeyboardModel kbdModel) 
            => Store.Save("KeyboardModelSource", kbdModel.Source);

        public static void ClearSelectedKeyboardModel()
            => Store.Save("KeyboardModelSource", (string?) null);

        public static string? GetSelectedKeyboardModel()
            => Store.Read<string>("KeyboardModelSource");

        public static async Task RenameKeyboard(string source, string newName)
        {
            var path = KeyboardSourceToPath(source);
            var model = await Store.ReadFileAsync<KeyboardModel>(path);
            model.Name = newName;
            await Store.SaveFileAsync(path, model);
        }

        public static async Task DeleteKeyboard(string source)
        {
            if (await Root.TryGetItemAsync(KeyboardSourceToPath(source)) is StorageFile sf)
            {
                if(GetSelectedKeyboardModel() == source) ClearSelectedKeyboardModel();
                await sf.DeleteAsync();
            }
        }

        public static async Task RebuildKeyboard(string source)
        {
            var path = KeyboardSourceToPath(source);
            var oldModel = await Store.ReadFileAsync<KeyboardModel>(path);
            var model = KeyboardModel.FromKeyboardKeyMap(oldModel.Info, oldModel.KeyMap);
            model.Name = oldModel.Name;
            model.Source = source;
            await Store.SaveFileAsync(path, model);
        }
    }


    public class JsonObjectSerializer : IObjectSerializer
    {
        public object Serialize<T>(T value) => JsonSerializer.Serialize(value);
        public T? Deserialize<T>(object value) => value is string s ? JsonSerializer.Deserialize<T>(s) : default;
    }
    
}
