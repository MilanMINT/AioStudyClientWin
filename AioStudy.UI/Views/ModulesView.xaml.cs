using AioStudy.UI.WpfServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AioStudy.UI.Views
{
    /// <summary>
    /// Interaction logic for ModulesView.xaml
    /// </summary>
    public partial class ModulesView : UserControl
    {
        private SoundPlayer? _soundPlayer;

        public ModulesView()
        {
            InitializeComponent();
        }


        private async Task PlaySoundAsync()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/AioStudy.UI;component/Sounds/start_timer_sound.wav");
                System.Diagnostics.Debug.WriteLine($"URI: {uri}");

                var streamInfo = System.Windows.Application.GetResourceStream(uri);
                System.Diagnostics.Debug.WriteLine($"Stream: {(streamInfo != null ? "✅ GEFUNDEN" : "❌ NULL")}");

                if (streamInfo != null)
                {
                    _soundPlayer = new SoundPlayer(streamInfo.Stream);

                    var tcs = new TaskCompletionSource<bool>();

                    _soundPlayer.LoadCompleted += (s, args) =>
                    {
                        try
                        {
                            _soundPlayer.Play();
                            System.Diagnostics.Debug.WriteLine("Sound abgespielt! 🔊");
                            tcs.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    };

                    _soundPlayer.LoadAsync();

                    await tcs.Task;
                }
                else
                {
                    MessageBox.Show("Sound-Datei nicht gefunden!\n\n" +
                                  "Prüfe:\n" +
                                  "1. Liegt die Datei in /Sounds/?\n" +
                                  "2. Build Action = Resource?\n" +
                                  "3. Projekt neu gebaut?",
                                  "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}",
                               "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }
        }
    }
}
