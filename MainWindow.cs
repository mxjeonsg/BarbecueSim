using System;
using System.Runtime.InteropServices;
using System.IO;

using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Collections.Generic;


namespace BarbecueSim {
    static class User32 {
        [DllImport("user32.dll")]
        extern static public int MessageBoxA(nint hWnd, string lpText, string lpCaption, uint uType);
    }

    static class Leaky {
        [DllImport("../../../asset/leaky.dll")]
        extern static public nint leaky(long slots);
    }

    class MainWindow : Window {
        private string[] args;
        [UI] private Label _label1 = null;
        [UI] private Label _label2 = null;
        [UI] private Button _button1 = null;

        private bool[] switches = new bool[5] {
            false, // /disableLeak
            false, // unused
            false, // unused
            false, // unused
            false // unused
        };

        private int _counter = 0;
        private int _last_score = int.Parse(File.ReadAllText("../../../asset/best_score.txt"));

        public void cloneArgs(string[] argv) {
            if(argv != null) {
                this.args = argv;
            }
        }

        private void prepareArgs() {
            if(this.args == null) return;

            foreach(string arg in this.args) {
                if(arg.Equals("d")) {
                    User32.MessageBoxA(0, "Leaking memory was disabled.", "Barbecue Simulator: Notification", 64u);
                    this.switches[0] = true;
                } else {
                    Console.WriteLine($"Commandline argument \"{arg}\" isn't valid.\nIgnoring.");
                }
            }
        }

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow")) {
            this.prepareArgs();

            int alert = User32.MessageBoxA(0,
                "This programme performs gamified memory leaks.\n\nThe usage of this programme is responsability of the person who opened the programme.\n\nWhile this programme doesn't destroy data on disc, this can still make the system crash.\n\nClick \"No\" to close this program.\n\nAre you still sure you want to continue?\n",
                "Barbecue Simulator: ATTENTION", 32u | 4u
            );

            if(alert == 7) {
                Environment.Exit(0);
            }

            try {
                builder.Autoconnect(this);
                this.Title = "Barbecue Simulator";

                this._label1.Text = $"Your last score is {this._last_score}!";
                this._label2.Text = $"Your current score is: {this._counter}";

                DeleteEvent += Window_DeleteEvent;
                this._button1.Clicked += Button1_Clicked;
            } catch(Exception ex) {
                int result = 4;

                while(result == 4u) {
                    result = User32.MessageBoxA(
                       0,
                        $"Exception {ex.GetType().Name} occurred!\n\n{ex.Message}\n\n{ex.StackTrace}\n\n\nThe programme was halted.",
                        "Barbecue Simulator: Unhandable exception",
                        16u | 2u
                    );

                    if(result == 3u) Environment.Exit(2);
                }
                
                Application.Quit();
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a) {
            Application.Quit();
            File.WriteAllText("../../../asset/best_score.txt", $"{this._counter}");
        }

        private void Button1_Clicked(object sender, EventArgs a) {
            try {
                this._counter++;

                this._label1.Text = $"Your last score is {this._last_score}!";
                this._label2.Text = $"Your current score is: {this._counter}";

                this._button1.Label = $"Make it {this._counter + 1}!";

                if(this.switches[0] == false) {
                    Leaky.leaky(1024*1024*1024);
                }
            } catch(Exception ex) {
                int result = 4;

                while(result == 4u) {
                    result = User32.MessageBoxA(
                       0,
                        $"Exception {ex.GetType().Name} occurred!\n\n{ex.Message}\n\n{ex.StackTrace}\n\n\nThe programme was halted.",
                        "Barbecue Simulator: Unhandable exception",
                        16u | 2u
                    );

                    if(result == 3u) Environment.Exit(2);
                }
                
                Application.Quit();
            }
        }
    }
}
