using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AioStudy.UI.WpfServices
{
    public static class SoundService
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static MediaPlayer? _currentPlayer;

        public static double Volume { get; set; } = 0.5;

        public static async Task PlaySoundAsync(string soundFileName, double? volume = null)
        {
            await _semaphore.WaitAsync();
            try
            {
                var uri = new Uri($"pack://application:,,,/AioStudy.UI;component/Sounds/{soundFileName}");
                var streamInfo = Application.GetResourceStream(uri);

                if (streamInfo == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[Sound] ❌ File not found: {soundFileName}");
                    return;
                }

                var tcs = new TaskCompletionSource<bool>();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        _currentPlayer = new MediaPlayer();
                        _currentPlayer.Volume = volume ?? Volume;

                        _currentPlayer.MediaEnded += (s, e) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"[Sound] ✅ Finished: {soundFileName}");
                            tcs.TrySetResult(true);
                        };

                        _currentPlayer.MediaFailed += (s, e) =>
                        {
                            System.Diagnostics.Debug.WriteLine($"[Sound] ❌ Failed: {e.ErrorException?.Message}");
                            tcs.TrySetResult(false);
                        };

                        // ✅ Speichere Stream in temporärer Datei (MediaPlayer braucht das)
                        var tempFile = Path.Combine(Path.GetTempPath(), $"sound_{Guid.NewGuid()}.wav");
                        using (var fileStream = File.Create(tempFile))
                        {
                            streamInfo.Stream.CopyTo(fileStream);
                        }

                        System.Diagnostics.Debug.WriteLine($"[Sound] Playing: {soundFileName} (Volume: {_currentPlayer.Volume * 100}%)");
                        _currentPlayer.Open(new Uri(tempFile));
                        _currentPlayer.Play();

                        // Lösche temp-Datei nach Abspielen
                        _currentPlayer.MediaEnded += (s, e) =>
                        {
                            try { File.Delete(tempFile); } catch { }
                        };
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[Sound] ❌ Error: {ex.Message}");
                        tcs.TrySetResult(false);
                    }
                });

                await Task.WhenAny(tcs.Task, Task.Delay(10000));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static void PlaySoundSync(string soundFileName, double? volume = null)
        {
            _ = PlaySoundAsync(soundFileName, volume);
        }
    }
}