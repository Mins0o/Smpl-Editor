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
        // Playlists : List of playlists throughtout the application
        private List<Smpl> Playlists;
        private List<Song> AllSongs;
        public MainWindow()
        {
            while (Playlists == null)
            {
                try
                {
                    this.LoadSmpls();
                }
                catch (ArgumentException e)
                {
                    // If SMPL of same name exists, try again after a message
                    if (e.Data.Contains("shortName"))
                    {
                        string shortName=(string)e.Data["shortName"];
                        MessageBox.Show(shortName + ": " + e.Message);
                    }
                }
            }

            // If no playlists were load from LoadSmpls(), shutdown the app
            if (Playlists.Count == 0)
            {
                MessageBox.Show("No Samsung Music Playlist detected. Exitting the application.");
                System.Windows.Application.Current.Shutdown();
            }
            InitializeComponent();

            // Link the listbox contents with playlists and songs
            PlaylistsBox.ItemsSource = Playlists;
            SongsListBox.ItemsSource = AllSongs;
        }

        // This function prompts OpenFileDialog and loads file with SMPL information.
        // Playlist will be created as the file name and its members will be added into the Smpl class.
        public void LoadSmpls()
        {
            // Get the file paths to load.
            OpenFileDialog openSmpls = new OpenFileDialog();
            openSmpls.Multiselect = true;
            openSmpls.ShowDialog();
            // Without full path, just names with extensions
            string[] shortNames=openSmpls.SafeFileNames;
            // Full path
            string[] fileNames = openSmpls.FileNames;

            // List to keep newly generated Smpls
            List<Smpl> playlists = new List<Smpl>();
            List<Song> allSongs = new List<Song>();

            //For every path user selected in the OpenFileDialog
            for(int i=0;i<shortNames.Length;i++)
            {
                // The files' contents are loaded to jsonString
                string jsonString = File.ReadAllText(fileNames[i]);
                Smpl playlist = JsonSerializer.Deserialize<Smpl>(jsonString);

                // When adding to our application, we will drop the extension from the file name
                string shortName = shortNames[i];
                int extensionStartIndex = shortName.IndexOf('.',shortName.Length-5);
                shortName = shortName.Substring(0,extensionStartIndex);
                playlist.name = shortName;

                // Type of playlists is List<Smpl>. Type of playlist is Smpl, which has .name :string and .members :List<Song> properties.
                try
                {
                    playlists.Add(playlist);
                }
                catch (ArgumentException e)
                {
                    e.Data.Add("shortName", shortName);
                    throw;
                }
                try
                {
                    allSongs.AddRange(playlist.members);
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message);
                }


            }
            this.AllSongs = allSongs;
            this.Playlists = playlists;
        }

        public void CreatePlaylist(string name,List<Song> songList)
        {
            Playlists.Add(new Smpl(name,songList));
        }
        public Boolean DeletePlaylist(string name)
        {
            Boolean deleted = false;
            foreach (Smpl playlist in Playlists)
            {
                if (playlist.name == name)
                {
                    Playlists.Remove(playlist);
                    deleted = true;
                    break;
                }
            }
            if (!deleted)
            {
                System.Diagnostics.Debug.Print("No such playlist found!");
            }
            return deleted;
        }

        private void ChangeList(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistsBox.SelectedItem != null)
            {
                SongsListBox.ItemsSource = Playlists[PlaylistsBox.SelectedIndex].members;
                AllSongsListBox.UnselectAll();
            }
            SongsListBox.ScrollIntoView(SongsListBox.Items[0]);
        }

        private void DeleteSong(object sender, RoutedEventArgs e)
        {
            var selectionListInterface = SongsListBox.SelectedItems;
            Song[] selection = new Song[selectionListInterface.Count];
            selectionListInterface.CopyTo(selection, 0);
            if (AllSongsListBox.SelectedItem != null)
            {
                foreach (Smpl playlist in this.Playlists)
                {
                    playlist.RemoveSongs(selection);
                }
                foreach (Song song in selection)
                {
                    this.AllSongs.Remove(song);
                }
                SongsListBox.ItemsSource = null;
                SongsListBox.ItemsSource = this.AllSongs;
            }
            else if (PlaylistsBox.SelectedItem != null)
            {
                Smpl playlist = Playlists[this.PlaylistsBox.SelectedIndex];
                playlist.RemoveSongs(selection);
                foreach (Song song in selection)
                {
                    this.AllSongs.Remove(song);
                }
                SongsListBox.ItemsSource = null;
                SongsListBox.ItemsSource = playlist.members;
            }
        }

        private void SelectedAllSongs(object sender, SelectionChangedEventArgs e)
        {
            if (AllSongsListBox.SelectedItem != null)
            {
                PlaylistsBox.UnselectAll();
                SongsListBox.ItemsSource = AllSongs;
            }
        }
    }
}
