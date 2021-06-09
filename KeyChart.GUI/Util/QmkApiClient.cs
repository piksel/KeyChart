using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using KeyChart.Keyboards.QMK;

namespace KeyChart.GUI.Util
{
    public class QmkApiClient: CachedApiClient
    {
        public async ValueTask<KeyboardQueryResult?> KeyboardInfo([NotNull] string keyboardName, CancellationToken? token = null)
            => await GetJson<KeyboardQueryResult>($"https://api.qmk.fm/v1/keyboards/{keyboardName}", "keyboards", token ?? CancellationToken.None);
        
        public async ValueTask<string[]?> Keyboards(CancellationToken? token = null)
            => await GetJson<string[]>($"https://api.qmk.fm/v1/keyboards", "keyboards", token ?? CancellationToken.None);

        public QmkApiClient([NotNull] DirectoryInfo cacheDir, [NotNull] IJsonSerializer jsonSerializer) 
            : base(cacheDir.CreateSubdirectory("qmk"), jsonSerializer) { }
        
        
    }
}