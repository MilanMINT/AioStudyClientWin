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
    /// Interaction logic for ModuleCard.xaml
    /// </summary>
    public partial class ModuleCard : UserControl
    {
        public ModuleCard()
        {
            InitializeComponent();
        }



        public Module Module
        {
            get { return (Module)GetValue(ModuleProperty); }
            set { SetValue(ModuleProperty, value); }
        }

        public static readonly DependencyProperty ModuleProperty =
            DependencyProperty.Register("Module", typeof(Module), typeof(ModuleCard), new PropertyMetadata(null));



        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ModuleCard), new PropertyMetadata(null));


    }
}
