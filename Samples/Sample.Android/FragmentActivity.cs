﻿using System;
using System.Collections.Generic;
using Android.OS;

using Android.App;
using Android.Widget;
using Android.Content.PM;
#if NET6_0
using Microsoft.Maui.ApplicationModel;
#else
using Xamarin.Essentials;
#endif
using ZXing.Mobile;

#if NET6_0
namespace Sample.Net6
#else
namespace Sample.Android
#endif
{
    [Activity(Label = "ZXing.Net.Mobile", Theme = "@style/Theme.AppCompat.Light", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
	public class FragmentActivity : AndroidX.Fragment.App.FragmentActivity
	{
		ZXingScannerFragment scanFragment;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.FragmentActivity);
		}

		protected override void OnResume()
		{
			base.OnResume();


			if (scanFragment == null)
			{
				scanFragment = new ZXingScannerFragment();

				SupportFragmentManager.BeginTransaction()
					.Replace(Resource.Id.fragment_container, scanFragment)
					.Commit();
			}

			Scan();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
			=> Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

		protected override void OnPause()
		{
			scanFragment?.StopScanning();

			base.OnPause();
		}

		void Scan()
		{
			var opts = new MobileBarcodeScanningOptions
			{
				PossibleFormats = new List<ZXing.BarcodeFormat> {
					ZXing.BarcodeFormat.QR_CODE
				},
				CameraResolutionSelector = availableResolutions =>
				{

					foreach (var ar in availableResolutions)
					{
						Console.WriteLine("Resolution: " + ar.Width + "x" + ar.Height);
					}
					return null;
				}
			};

			scanFragment.StartScanning(result =>
			{

				// Null result means scanning was cancelled
				if (result == null || string.IsNullOrEmpty(result.Text))
				{
					Toast.MakeText(this, "Scanning Cancelled", ToastLength.Long).Show();
					return;
				}

				// Otherwise, proceed with result
				RunOnUiThread(() => Toast.MakeText(this, "Scanned: " + result.Text, ToastLength.Short).Show());
			}, opts);
		}
	}
}
