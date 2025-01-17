﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
#if WINDOWS_UWP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
#endif
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ZXing.Mobile
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ScanPage : Page, IMobileBarcodeScanner
	{
		ScanPageNavigationParameters Parameters { get; set; }

		public ScanPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			// If no parameters were passed, we navigated here for some other reason
			// so let's ignore it
			if (e.Parameter == null)
				return;

			Parameters = e.Parameter as ScanPageNavigationParameters;
			scannerControl.OnCameraInitialized += ScannerControl_OnCameraInitialized;
			scannerControl.OnScannerError += ScannerControl_OnScannerError;

			if (Parameters != null)
				Parameters.Scanner.ScanPage = this;

			scannerControl.TopText = Parameters?.Scanner?.TopText ?? "";
			scannerControl.BottomText = Parameters?.Scanner?.BottomText ?? "";

			scannerControl.CustomOverlay = Parameters?.Scanner?.CustomOverlay;
			scannerControl.UseCustomOverlay = Parameters?.Scanner?.UseCustomOverlay ?? false;

			scannerControl.ScanningOptions = Parameters?.Options ?? new MobileBarcodeScanningOptions();
			scannerControl.ContinuousScanning = Parameters?.ContinuousScanning ?? false;

			scannerControl.StartScanning(Parameters?.ResultHandler, Parameters?.Options);
		}

		void ScannerControl_OnCameraInitialized()
			=> Parameters.CameraInitialized?.Invoke();

		void ScannerControl_OnScannerError(IEnumerable<string> errors)
			=> Parameters.CameraError?.Invoke(errors);

		protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			try
			{
				MobileBarcodeScanner.Log("OnNavigatingFrom, stopping camera...");
				await scannerControl.StopScanningAsync();
			}
			catch (Exception ex)
			{
				MobileBarcodeScanner.Log("OnNavigatingFrom Error: {0}", ex);
			}

			base.OnNavigatingFrom(e);
		}

#region IMobileBarcodeScanner Implementation
		public bool UseCustomOverlay
			=> scannerControl.UseCustomOverlay;

		public string TopText
		{
			get => scannerControl.TopText;
			set => scannerControl.TopText = value;
		}

		public string BottomText
		{
			get => scannerControl.BottomText;
			set => scannerControl.BottomText = value;
		}

		public string CancelButtonText
		{
			get => string.Empty;
			set { }
		}

		public string FlashButtonText
		{
			get => string.Empty;
			set { }
		}

		public string CameraUnsupportedMessage
		{
			get => string.Empty;
			set { }
		}

		public bool IsTorchOn
			=> scannerControl.IsTorchOn;

		public Task<Result> Scan(MobileBarcodeScanningOptions options)
		{
			var tcsResult = new TaskCompletionSource<Result>();

			scannerControl.ContinuousScanning = false;
			scannerControl.StartScanning(r =>
			{
				scannerControl.StopScanning();

				tcsResult.SetResult(r);
			}, options ?? Parameters?.Options);

			return tcsResult.Task;
		}

		public Task<Result> Scan()
			=> Scan(new MobileBarcodeScanningOptions());

		public void ScanContinuously(MobileBarcodeScanningOptions options, Action<Result> scanHandler)
		{
			scannerControl.ContinuousScanning = true;
			scannerControl.StartScanning(scanHandler, options ?? Parameters?.Options);
		}

		public void ScanContinuously(Action<Result> scanHandler)
			=> ScanContinuously(new MobileBarcodeScanningOptions(), scanHandler);

		public void Cancel()
			=> scannerControl?.Cancel();

		public void Torch(bool on)
			=> scannerControl?.Torch(on);

		public void AutoFocus()
			=> scannerControl?.AutoFocus();

		public void ToggleTorch()
		=> scannerControl?.ToggleTorch();

		public void PauseAnalysis()
			=> scannerControl?.PauseAnalysis();

		public void ResumeAnalysis()
			=> scannerControl?.ResumeAnalysis();
#endregion
	}

	public class ScanPageNavigationParameters
	{
		public MobileBarcodeScanner Scanner { get; set; }
		public bool ContinuousScanning { get; set; }
		public MobileBarcodeScanningOptions Options { get; set; }
		public Action<ZXing.Result> ResultHandler { get; set; }

		public Action CameraInitialized { get; set; }
		public Action<IEnumerable<string>> CameraError { get; set; }
	}
}
