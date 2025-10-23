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
        public static readonly DependencyProperty ModuleProperty = DependencyProperty.Register("Module", typeof(object), typeof(ModuleCard), new PropertyMetadata(null, OnModuleChanged));
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ModuleCard));

        public Module Module
        {
            get { return (Module)GetValue(ModuleProperty); }
            set { SetValue(ModuleProperty, value); }
        }

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        // Dependency Properties für die Anzeige
        public static readonly DependencyProperty ModuleNameProperty =
            DependencyProperty.Register("ModuleName", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty ModuleColorProperty =
            DependencyProperty.Register("ModuleColor", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty CreditsDisplayProperty =
            DependencyProperty.Register("CreditsDisplay", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty GradeDisplayProperty =
            DependencyProperty.Register("GradeDisplay", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty GradeColorProperty =
            DependencyProperty.Register("GradeColor", typeof(Brush), typeof(ModuleCard));

        public static readonly DependencyProperty StudyTimeDisplayProperty =
            DependencyProperty.Register("StudyTimeDisplay", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty ExamDateDisplayProperty =
            DependencyProperty.Register("ExamDateDisplay", typeof(string), typeof(ModuleCard));

        public static readonly DependencyProperty ExamDateBadgeColorProperty =
            DependencyProperty.Register("ExamDateBadgeColor", typeof(Brush), typeof(ModuleCard));

        public static readonly DependencyProperty CreatedDisplayProperty =
            DependencyProperty.Register("CreatedDisplay", typeof(string), typeof(ModuleCard));

        // Properties für Binding
        public string ModuleName
        {
            get { return (string)GetValue(ModuleNameProperty); }
            set { SetValue(ModuleNameProperty, value); }
        }

        public string ModuleColor
        {
            get { return (string)GetValue(ModuleColorProperty); }
            set { SetValue(ModuleColorProperty, value); }
        }

        public string CreditsDisplay
        {
            get { return (string)GetValue(CreditsDisplayProperty); }
            set { SetValue(CreditsDisplayProperty, value); }
        }

        public string GradeDisplay
        {
            get { return (string)GetValue(GradeDisplayProperty); }
            set { SetValue(GradeDisplayProperty, value); }
        }

        public Brush GradeColor
        {
            get { return (Brush)GetValue(GradeColorProperty); }
            set { SetValue(GradeColorProperty, value); }
        }

        public string StudyTimeDisplay
        {
            get { return (string)GetValue(StudyTimeDisplayProperty); }
            set { SetValue(StudyTimeDisplayProperty, value); }
        }

        public string ExamDateDisplay
        {
            get { return (string)GetValue(ExamDateDisplayProperty); }
            set { SetValue(ExamDateDisplayProperty, value); }
        }

        public Brush ExamDateBadgeColor
        {
            get { return (Brush)GetValue(ExamDateBadgeColorProperty); }
            set { SetValue(ExamDateBadgeColorProperty, value); }
        }

        public string CreatedDisplay
        {
            get { return (string)GetValue(CreatedDisplayProperty); }
            set { SetValue(CreatedDisplayProperty, value); }
        }

        public ModuleCard()
        {
            InitializeComponent();
        }

        private static void OnModuleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModuleCard card && e.NewValue is Module module)
            {
                card.UpdateDisplayProperties(module);
            }
        }

        private void UpdateDisplayProperties(Module module)
        {
            ModuleName = module.Name;
            ModuleColor = module.Color;

            CreditsDisplay = module.ModuleCredits?.ToString() + " CP" ?? "- CP";

            GradeDisplay = module.Grade?.ToString("F1") ?? "-";
            GradeColor = module.Grade.HasValue ?
                new SolidColorBrush(Color.FromRgb(0x51, 0xCF, 0x66)) :
                new SolidColorBrush(Color.FromRgb(0x8A, 0x8D, 0x95));

            StudyTimeDisplay = $"{module.LearnedMinutes}min";

            ExamDateDisplay = module.ExamDate?.ToString("dd.MM.yyyy") ?? "Not scheduled";
            ExamDateBadgeColor = module.ExamDate.HasValue ?
                new SolidColorBrush(Color.FromRgb(0xFF, 0x8A, 0x00)) :
                new SolidColorBrush(Color.FromRgb(0x4A, 0x4D, 0x55));

            CreatedDisplay = $"Created: {module.CreatedString}";
        }
    }
}
