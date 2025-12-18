using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private const double WideThreshold = 900.0;

        public SettingsView()
        {
            InitializeComponent();
            Loaded += SettingsView_Loaded;
            SizeChanged += SettingsView_SizeChanged;
        }

        private void SettingsView_Loaded(object? sender, RoutedEventArgs e)
        {
            UpdateAdaptiveState(this.ActualWidth);
        }

        private void SettingsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateAdaptiveState(e.NewSize.Width);
        }

        private void UpdateAdaptiveState(double width)
        {
            if (LayoutRoot == null) return;
            var state = width >= WideThreshold ? "Wide" : "Narrow";
            VisualStateManager.GoToElementState(LayoutRoot, state, true);
        }
    }
}
