﻿/* Got inspiration from https://www.wintellect.com/creating-a-secondary-bottom-ios-toolbar-in-xamarin-forms/ */
using System;
using System.Linq;
using UIKit;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using Travlendar.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomToolbarItemsRenderer))]
namespace Travlendar.iOS.Renderers
{
    public class CustomToolbarItemsRenderer : NavigationRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            var badNavigationBar = View.Subviews.OfType<UIToolbar>().FirstOrDefault(v => v.GetType() != typeof(UIToolbar));
            if (badNavigationBar != null)
            {
                badNavigationBar.RemoveFromSuperview();
            }
            base.ViewWillAppear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            UIView[] subviews = View.Subviews.Where(v => v != NavigationBar).ToArray();
            var toolBarViews = subviews.Where(v => v is UIToolbar).ToArray();
            var otherViews = subviews.Where(v => !(v is UIToolbar)).ToArray();

            nfloat toolbarHeight = 0;

            foreach (var uIView in toolBarViews)
            {
                uIView.SizeToFit();
                uIView.Frame = new CGRect
                {
                    X = 0,
                    Y = View.Bounds.Height - uIView.Frame.Height,
                    Width = View.Bounds.Width,
                    Height = uIView.Frame.Height,
                };
                var thisToolbarHeight = uIView.Frame.Height;
                if (toolbarHeight < thisToolbarHeight)
                {
                    toolbarHeight = thisToolbarHeight;
                }
            }

            var othersHeight = View.Bounds.Height - toolbarHeight;
            var othersFrame = new CGRect(View.Bounds.X, View.Bounds.Y, View.Bounds.Width, othersHeight);

            foreach (var uIView in otherViews)
            {
                uIView.Frame = othersFrame;
            }
        }
    }
}
