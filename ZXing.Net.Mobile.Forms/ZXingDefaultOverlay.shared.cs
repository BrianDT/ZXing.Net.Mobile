﻿using System;
using System.Windows.Input;
#if !NET6_0
using Xamarin.Forms;
#endif

namespace ZXing.Net.Mobile.Forms
{
	public class ZXingDefaultOverlay : Grid
	{
		readonly Label topText;
		readonly Label botText;
		readonly Button flash;

		public delegate void FlashButtonClickedDelegate(Button sender, EventArgs e);
		public event FlashButtonClickedDelegate FlashButtonClicked;

		public ZXingDefaultOverlay()
		{
			BindingContext = this;

			VerticalOptions = LayoutOptions.FillAndExpand;
			HorizontalOptions = LayoutOptions.FillAndExpand;

			RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
			RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
			ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

#if NET6_0
			var backgroundColor = Colors.Black;
            var backgroundColor2 = Colors.Red;
            var textColor = Colors.White;
#else
			var backgroundColor = Color.Black;
			var backgroundColor2 = Color.Red;
			var textColor = Color.White;
#endif

			Children.Add(new BoxView
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = backgroundColor,
				Opacity = 0.7,
#if NET6_0
			});
#else
			}, 0, 0);
#endif

			Children.Add(new BoxView
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = backgroundColor,
				Opacity = 0.7,
#if NET6_0
			});
#else
			}, 0, 2);
#endif

			Children.Add(new BoxView
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 3,
				BackgroundColor = backgroundColor2,
				Opacity = 0.6,
#if NET6_0
            });
#else
			}, 0, 1);
#endif

            topText = new Label
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				TextColor = textColor,
				AutomationId = "zxingDefaultOverlay_TopTextLabel",
			};
			topText.SetBinding(Label.TextProperty, new Binding(nameof(TopText)));
			Children.Add(topText
#if NET6_0
				);
#else
				, 0, 0);
#endif

            botText = new Label
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				TextColor = textColor,
				AutomationId = "zxingDefaultOverlay_BottomTextLabel",
			};
			botText.SetBinding(Label.TextProperty, new Binding(nameof(BottomText)));
			Children.Add(botText
#if NET6_0
                );
#else
				, 0, 2);
#endif

            flash = new Button
			{
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Start,
				Text = "Flash",
				TextColor = textColor,
				AutomationId = "zxingDefaultOverlay_FlashButton",
			};
			flash.SetBinding(Button.IsVisibleProperty, new Binding(nameof(ShowFlashButton)));
			flash.Clicked += (sender, e) =>
			{
				FlashButtonClicked?.Invoke(flash, e);
			};

			Children.Add(flash
#if NET6_0
                );
#else
				, 0, 0);
#endif
        }

        public static readonly BindableProperty TopTextProperty =
			BindableProperty.Create(nameof(TopText), typeof(string), typeof(ZXingDefaultOverlay), string.Empty);
		public string TopText
		{
			get => (string)GetValue(TopTextProperty);
			set => SetValue(TopTextProperty, value);
		}

		public static readonly BindableProperty BottomTextProperty =
			BindableProperty.Create(nameof(BottomText), typeof(string), typeof(ZXingDefaultOverlay), string.Empty);
		public string BottomText
		{
			get => (string)GetValue(BottomTextProperty);
			set => SetValue(BottomTextProperty, value);
		}

		public static readonly BindableProperty ShowFlashButtonProperty =
			BindableProperty.Create(nameof(ShowFlashButton), typeof(bool), typeof(ZXingDefaultOverlay), false);
		public bool ShowFlashButton
		{
			get => (bool)GetValue(ShowFlashButtonProperty);
			set => SetValue(ShowFlashButtonProperty, value);
		}

		public static BindableProperty FlashCommandProperty =
			BindableProperty.Create(nameof(FlashCommand), typeof(ICommand), typeof(ZXingDefaultOverlay),
				defaultValue: default(ICommand),
				propertyChanged: OnFlashCommandChanged);

		public ICommand FlashCommand
		{
			get => (ICommand)GetValue(FlashCommandProperty);
			set => SetValue(FlashCommandProperty, value);
		}

		static void OnFlashCommandChanged(BindableObject bindable, object oldvalue, object newValue)
		{
			var overlay = bindable as ZXingDefaultOverlay;
			if (overlay?.flash == null)
				return;
			overlay.flash.Command = newValue as Command;
		}
	}
}
