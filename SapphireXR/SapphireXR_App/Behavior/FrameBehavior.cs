﻿using Microsoft.Xaml.Behaviors;
using SapphireXR_App.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Behavior
{
    public class FrameBehavior : Behavior<Frame>
    {
        private bool _isWork;
        protected override void OnAttached()
        {
            //네비게이션 시작
            AssociatedObject.Navigating += AssociatedObject_Navigating;
            //네비게이션 종료
            AssociatedObject.Navigated += AssociatedObject_Navigated;
        }

        // 네비게이션 종료 이벤트 핸들러
        private void AssociatedObject_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _isWork = true;
            //네비게이션이 완료된 Uri를 NavigationSource에 입력
            NavigationSource = e.Uri.ToString();
            _isWork = false;
            //네비게이션이 완료된 상황을 뷰모델에 알려주기
            if (AssociatedObject.Content is Page pageContent
                && pageContent.DataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigated(sender, e);
            }
        }
        // 네비게이션 시작 이벤트 핸들러
        private void AssociatedObject_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //네비게이션 시작전 상황을 뷰모델에 알려주기
            if (AssociatedObject.Content is Page pageContent
                && pageContent.DataContext is INavigationAware navigationAware)
            {
                navigationAware?.OnNavigating(sender, e);
            }
        }
        protected override void OnDetaching()
        {
            AssociatedObject.Navigating -= AssociatedObject_Navigating;
            AssociatedObject.Navigated -= AssociatedObject_Navigated;
        }
        public string NavigationSource
        {
            get { return (string)GetValue(NavigationSourceProperty); }
            set { SetValue(NavigationSourceProperty, value); }
        }
        // NavigationSource DP
        public static readonly DependencyProperty NavigationSourceProperty =
            DependencyProperty.Register(nameof(NavigationSource), typeof(string), typeof(FrameBehavior), new PropertyMetadata(null, NavigationSourceChanged));
        // NavigationSource PropertyChanged
        private static void NavigationSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (FrameBehavior)d;
            if (behavior._isWork)
            {
                return;
            }
            behavior.Navigate();
        }
        // 네비게이트
        private void Navigate()
        {
            switch (NavigationSource)
            {
                case "GoBack":
                    //GoBack으로 오면 뒤로가기
                    if (AssociatedObject.CanGoBack)
                    {
                        AssociatedObject.GoBack();
                    }
                    break;
                case null:
                case "":
                    //아무것도 않함
                    return;
                default:
                    //navigate
                    AssociatedObject.Navigate(new Uri(NavigationSource, UriKind.RelativeOrAbsolute));
                    break;
            }
        }
    }
}
