using SapphireXR_App.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            CancelEventArgs cancelEventArgs = new CancelEventArgs();
            ((MainViewModel)DataContext).OnClosingCommand.Execute(cancelEventArgs);
            if (cancelEventArgs.Cancel == false)
            {
                App.Current.Shutdown();
            }
        }

        private void onTabItemMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changeTabBackForeground(sender, e, (tabItem) => tabItem.IsSelected == false, (border, textBlock) =>
            {
                border.Background = BackgroundColorOnMousePressedEvent;
                textBlock.Foreground = FontColorOnMousePressedEvent;
            });
        }

        private void onTabItemMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            changeTabBackForeground(sender, e, (tabItem) => true, (border, textBlock) => {
                border.ClearValue(Border.BackgroundProperty);
                textBlock.ClearValue(TextBlock.ForegroundProperty);
            });
        }

        private void changeTabBackForeground(object sender, System.Windows.Input.MouseButtonEventArgs e, Func<TabItem, bool> selected, Action<Border, TextBlock> changeColor)
        {
            Border? border = sender as Border;
            if (border != null)
            {
                TabItem? tabItem = border.TemplatedParent as TabItem;
                if (tabItem != null && selected(tabItem) == true)
                {
                    ContentPresenter? contentPresenter = border.Child as ContentPresenter;
                    if (contentPresenter != null)
                    {
                        StackPanel? stackPanel = contentPresenter.Content as StackPanel;
                        if (stackPanel != null)
                        {
                            foreach (UIElement child in stackPanel.Children)
                            {
                                if (child is TextBlock textBlock)
                                {
                                    changeColor(border, (TextBlock)child);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Brush FontColorOnMousePressedEvent = Application.Current.Resources.MergedDictionaries[0]["Gray_800"] as Brush ?? Brushes.Red;
        private static Brush BackgroundColorOnMousePressedEvent = Application.Current.Resources.MergedDictionaries[0]["Gray_50"] as Brush ?? Brushes.Blue;
    }
}
