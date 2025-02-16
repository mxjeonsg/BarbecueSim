using System;
using Gtk;

namespace BarbecueSim
{
    class Program
    {
        [STAThread] public static void Main(string[] args) {
            Application.Init();

            Application app = new Application("com.divinitylabs.BarbecueSim", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            MainWindow win = new MainWindow();
            win.cloneArgs(args);
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
