using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmplEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string,Smpl> Playlists;
        public MainWindow()
        {
            while (Playlists == null){
                try
                {
                    Playlists = this.LoadSmpls();
                }
                catch (ArgumentException e)
                {
                    if (e.Data.Contains("shortName"))
                    {
                        string shortName=(string)e.Data["shortName"];
                        MessageBox.Show(shortName + ": " + e.Message);
                    }
                }
            }
            InitializeComponent();
            PlaylistsBox.ItemsSource = Playlists.Keys;
            SongsListBox.ItemsSource = AllSongs();
        }

        public Dictionary<string,Smpl> LoadSmpls()
        {
            OpenFileDialog openSmpls = new OpenFileDialog();
            openSmpls.Multiselect = true;
            openSmpls.ShowDialog();
            string[] shortNames=openSmpls.SafeFileNames;

            Dictionary<string,Smpl> playlists = new Dictionary<string,Smpl>();
            for(int i=0;i<shortNames.Length;i++)
            {
                string shortName = shortNames[i];
                string jsonString = File.ReadAllText(openSmpls.FileNames[i]);
                Smpl playlist = JsonSerializer.Deserialize<Smpl>(jsonString);
                int extensionStartIndex = shortName.IndexOf('.',shortName.Length-5);
                shortName = shortName.Substring(0,extensionStartIndex);
                playlist.name = shortName;
                try
                {
                    playlists.Add(shortName, playlist);
                }
                catch (ArgumentException e)
                {
                    e.Data.Add("shortName", shortName);
                    throw;
                }


            }
            return playlists;
        }

        public Song[] AllSongs()
        {
            List<Song> allSongs = new List<Song>();
            foreach (Smpl playlist in Playlists.Values)
            {
                allSongs.AddRange(playlist.members);
            }
            return allSongs.ToArray();
        }

        public void CreatePlaylist(string name)
        {
            Playlists.Add(name, new Smpl());
        }
        public void DeletePlaylist(string name)
        {
            Playlists.Remove(name);
        }

        private void ChangeList(object sender, SelectionChangedEventArgs e)
        {
            SongsListBox.ItemsSource = Playlists[SongsListBox.Items[SongsListBox.SelectedIndex].ToString()];
        }
    }
}
