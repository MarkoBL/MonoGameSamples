using System;

namespace MBL
{
	internal class Application 
	{
		#if MONOTOUCH
		[Register ("AppDelegate")]
		private class AppDelegate : UIApplicationDelegate
		{
			private Game game;

			public override void FinishedLaunching (UIApplication app)
			{
				game = Instance.CreateGame ();
				game.Run ();
			}
		}

		private static Program Instance;

		protected void Run (string[] args)
		{
			Instance = this;
			UIApplication.Main (args, null ,"AppDelegate");
		}
		#endif

		#if WINDOWS
		protected void Run(string[] args)
		{
			Game game = new Game();
			game.Run();
		}
		#endif

		#if MONOMAC
		protected void Run (string[] args)
		{
			MonoMac.AppKit.NSApplication.Init ();

			using (var p = new MonoMac.Foundation.NSAutoreleasePool ()) 
			{
				MonoMac.AppKit.NSApplication.SharedApplication.Delegate = new AppDelegate (this);
				MonoMac.AppKit.NSApplication.Main (args);
			}
		}

		class AppDelegate : MonoMac.AppKit.NSApplicationDelegate
		{
			private Application application;
			private Game game;

			public AppDelegate(Application application)
			{
				this.application = application;
			}

			public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
			{
				game = new Game ();
				game.Run();
			}

			public override bool ApplicationShouldTerminateAfterLastWindowClosed (MonoMac.AppKit.NSApplication sender)
			{
				return true;
			}
		}
		#endif

		#if !(WINDOWS_PHONE || WINRT || ANDROID)
		public static void Main (string[] args)
		{
			Application application = new Application ();
			application.Run (args);
		}
		#endif
	}
}

