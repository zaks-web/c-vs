using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using GantsPlace.Models;
using GantsPlace.Services;

namespace GantsPlace.Views
{
    public partial class ExplorerPage : Page
    {
        private readonly MainWindow _main;
        private string _filtreType = "Tous";
        private string _filtreCapacite = "Toutes";

        public ExplorerPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            Loaded += (_, _) => { BuildFiltres(); Filtrer(); };
        }

        private void BuildFiltres()
        {
            FiltreTypePanel.Children.Clear();
            FiltreCapacitePanel.Children.Clear();

            // Types
            var types = new[] { "Tous", "Salle de réunion", "Salle de cours", "Amphithéâtre" };
            foreach (var t in types)
            {
                var btn = CreateFilterBtn(t, t == _filtreType);
                btn.Tag = t;
                btn.Click += (s, e) => { _filtreType = (string)((Button)s).Tag; BuildFiltres(); Filtrer(); };
                FiltreTypePanel.Children.Add(btn);
            }

            // Capacités
            var caps = new[] { "Toutes", "Moins de 20", "20 à 50", "Plus de 50" };
            foreach (var c in caps)
            {
                var btn = CreateFilterBtn(c, c == _filtreCapacite);
                btn.Tag = c;
                btn.Click += (s, e) => { _filtreCapacite = (string)((Button)s).Tag; BuildFiltres(); Filtrer(); };
                FiltreCapacitePanel.Children.Add(btn);
            }
        }

        private static Button CreateFilterBtn(string text, bool active)
        {
            return new Button
            {
                Content = text, Margin = new Thickness(0, 0, 0, 6),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Padding = new Thickness(12, 9, 12, 9), Cursor = Cursors.Hand,
                Background = active ? new SolidColorBrush(Color.FromRgb(0x14, 0x20, 0x45)) : Brushes.Transparent,
                Foreground = active ? new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7)) : new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)),
                BorderThickness = new Thickness(1),
                BorderBrush = active ? new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7)) : new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45)),
                FontFamily = new FontFamily("Segoe UI"), FontSize = 13,
                Template = GetFlatBtnTemplate()
            };
        }

        private static ControlTemplate GetFlatBtnTemplate()
        {
            var t = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetBinding(Border.BorderBrushProperty,  new System.Windows.Data.Binding("BorderBrush")  { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetBinding(Border.BorderThicknessProperty, new System.Windows.Data.Binding("BorderThickness") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetBinding(Border.PaddingProperty, new System.Windows.Data.Binding("Padding") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
            var cp = new FrameworkElementFactory(typeof(ContentPresenter));
            cp.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            cp.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            border.AppendChild(cp);
            t.VisualTree = border;
            return t;
        }

        private void Filtrer()
        {
            var q = DataService.Salles.AsEnumerable();
            if (_filtreType != "Tous") q = q.Where(s => s.TypeSalle == _filtreType);
            q = _filtreCapacite switch
            {
                "Moins de 20" => q.Where(s => s.Capacite < 20),
                "20 à 50"     => q.Where(s => s.Capacite >= 20 && s.Capacite <= 50),
                "Plus de 50"  => q.Where(s => s.Capacite > 50),
                _ => q
            };
            var list = q.ToList();
            TxtCount.Text = $"{list.Count} salle{(list.Count > 1 ? "s" : "")}";
            SallesPanel.Children.Clear();
            foreach (var s in list) SallesPanel.Children.Add(CreateCard(s));
        }

        private UIElement CreateCard(Salle salle)
        {
            var card = new Border
            {
                Width = 260, Margin = new Thickness(0, 0, 18, 18),
                Background = new SolidColorBrush(Color.FromRgb(0x1C, 0x20, 0x30)),
                CornerRadius = new CornerRadius(14), BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45)),
                Cursor = Cursors.Hand,
                Effect = new DropShadowEffect { ShadowDepth = 0, BlurRadius = 16, Opacity = 0.25, Color = Colors.Black }
            };

            var stack = new StackPanel();

            // Image
            var imgB = new Border { Height = 150, CornerRadius = new CornerRadius(14, 14, 0, 0), ClipToBounds = true, Background = new SolidColorBrush(Color.FromRgb(0x12, 0x18, 0x28)) };
            var brush = salle.ImagePath != null ? ImageHelper.LoadImageBrush(salle.ImagePath) : null;
            if (brush != null) imgB.Background = brush;
            else imgB.Child = new TextBlock { Text = salle.TypeSalle == "Amphithéâtre" ? "🏛️" : salle.TypeSalle == "Salle de cours" ? "📚" : "🤝", FontSize = 36, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            stack.Children.Add(imgB);

            var info = new StackPanel { Margin = new Thickness(14, 12, 14, 14) };
            info.Children.Add(new TextBlock { Text = salle.Nom, FontSize = 14, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(0xEE, 0xF0, 0xF8)), FontFamily = new FontFamily("Segoe UI"), TextTrimming = TextTrimming.CharacterEllipsis });

            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 10) };
            row.Children.Add(new TextBlock { Text = $"👥 {salle.Capacite} pers.", FontSize = 11, Foreground = new SolidColorBrush(Color.FromRgb(0x8A, 0x94, 0xB0)), FontFamily = new FontFamily("Segoe UI"), Margin = new Thickness(0, 0, 12, 0) });
            var typeBadge = new Border { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(salle.TypeSalle == "Amphithéâtre" ? "#142045" : salle.TypeSalle == "Salle de cours" ? "#0D2A1A" : "#1E1030")), CornerRadius = new CornerRadius(4), Padding = new Thickness(6, 2, 6, 2) };
            typeBadge.Child = new TextBlock { Text = salle.TypeSalle, FontSize = 10, Foreground = new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7)), FontFamily = new FontFamily("Segoe UI") };
            row.Children.Add(typeBadge);
            info.Children.Add(row);

            var btn = new Button { Content = "Voir →", Style = (Style)Application.Current.FindResource("SecondaryButtonStyle"), HorizontalAlignment = HorizontalAlignment.Stretch, Padding = new Thickness(0, 8, 0, 8) };
            btn.Click += (s, e) => { _main.NavigateTo("DetailSalle", salle); _main.UpdateNavButtons(""); };
            info.Children.Add(btn);
            stack.Children.Add(info);
            card.Child = stack;

            card.MouseEnter += (s, e) => {
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(0x4F, 0x8E, 0xF7));
                var st = new ScaleTransform(1, 1); card.RenderTransform = st; card.RenderTransformOrigin = new Point(0.5, 0.5);
                var a = new DoubleAnimation(1, 1.02, TimeSpan.FromMilliseconds(120));
                st.BeginAnimation(ScaleTransform.ScaleXProperty, a);
                st.BeginAnimation(ScaleTransform.ScaleYProperty, a);
            };
            card.MouseLeave += (s, e) => {
                card.BorderBrush = new SolidColorBrush(Color.FromRgb(0x25, 0x2D, 0x45));
                var a = new DoubleAnimation(1.02, 1, TimeSpan.FromMilliseconds(120));
                if (card.RenderTransform is ScaleTransform st) { st.BeginAnimation(ScaleTransform.ScaleXProperty, a); st.BeginAnimation(ScaleTransform.ScaleYProperty, a); }
            };
            return card;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            _filtreType = "Tous"; _filtreCapacite = "Toutes";
            FiltreTypePanel.Children.Clear(); FiltreCapacitePanel.Children.Clear();
            BuildFiltres(); Filtrer();
        }
    }
}
