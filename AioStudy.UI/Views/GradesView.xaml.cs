using System;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace AioStudy.UI.Views
{
    public partial class GradesView : UserControl
    {
        // breakpoint in pixels — adjust to your needs
        private const double WideThreshold = 900.0;

        // Add a field to reference the root element in the XAML
        // This assumes your XAML root element has x:Name="LayoutRoot"
        public GradesView()
        {
            InitializeComponent();
            Loaded += GradesView_Loaded;
            SizeChanged += GradesView_SizeChanged;
        }

        private void GradesView_Loaded(object? sender, RoutedEventArgs e)
        {
            UpdateAdaptiveState(this.ActualWidth);
        }

        private void GradesView_SizeChanged(object sender, SizeChangedEventArgs e)
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