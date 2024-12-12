using GameLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadHighScore();

            LoadLevel level = new LoadLevel();

            level.PlayerDead += ReloadGame;

            MainGrid.Children.Add(level);   
        }

        private void ReloadGame(Object sender, EventArgs e)
        {
            MainGrid.Children.Clear();

            LoadLevel level = new LoadLevel();

            level.PlayerDead += ReloadGame;

            MainGrid.Children.Add(level);
        }

        private async void LoadHighScore()
        {
            await HighScore.ReadHighScore();
        }
    }
}
