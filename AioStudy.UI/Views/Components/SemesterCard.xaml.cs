using AioStudy.Models;
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

namespace AioStudy.UI.Views.Components
{
    /// <summary>
    /// Interaction logic for SemesterCard.xaml
    /// </summary>
    public partial class SemesterCard : UserControl
    {
        public SemesterCard()
        {
            InitializeComponent();
        }

        public Semester Semester
        {
            get { return (Semester)GetValue(SemesterProperty); }
            set { SetValue(SemesterProperty, value); }
        }

        public static readonly DependencyProperty SemesterProperty =
            DependencyProperty.Register("Semester", typeof(Semester), typeof(SemesterCard), new PropertyMetadata(null));



        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(SemesterCard), new PropertyMetadata(null));

        public ICommand OpenSemesterOverviewCommand
        {
            get { return (ICommand)GetValue(OpenSemesterOverviewCommandProperty); }
            set { SetValue(OpenSemesterOverviewCommandProperty, value); }
        }

        public static readonly DependencyProperty OpenSemesterOverviewCommandProperty =
            DependencyProperty.Register("OpenSemesterOverviewCommand", typeof(ICommand), typeof(SemesterCard), new PropertyMetadata(null));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
