using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Background;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.System.RemoteDesktop;
using Windows.UI.Composition.Interactions;

namespace GameLibrary
{
    public static class HighScore
    {
        public static int Highscore { get; set; } = 0;

        // Set new Highscore
        public static async Task SaveHighScore(int newScore)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile storageFile =
                await storageFolder.CreateFileAsync("GhostBusterHighscore.txt", CreationCollisionOption.ReplaceExisting);

            if (newScore > Highscore)
            {
                Highscore = newScore;
                await FileIO.WriteTextAsync(storageFile, newScore.ToString());
            }
        }

        public static async Task ReadHighScore()
        {
            int temp;

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile storageFile = await storageFolder.GetFileAsync("GhostBusterHighscore.txt");
                string line = await FileIO.ReadTextAsync(storageFile);
                temp = int.Parse(line);
                if (temp > Highscore)
                    Highscore = temp;
            }
            catch (Exception ex)
            {  }
        }
    }
}
