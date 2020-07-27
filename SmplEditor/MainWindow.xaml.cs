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
            DisplayAllSongs();
            AddingListSelector.ItemsSource = Playlists;
            AddingListSelector.SelectedIndex = 0;
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

        // UI events
        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If not de-selection
            if (PlaylistsBox.SelectedItem != null)
            {
                DisplaySelectedPlaylist();
            }
        }

        private void OnAllSongsListSelectionChanged(object sendor, RoutedEventArgs e)
        {
            if (AllSongsListBox.SelectedItem != null)
            {
                DisplayAllSongs();
            }

        }

        private void OnAddSongsClicked(object sender, RoutedEventArgs e)
        {
            Smpl targetList = Playlists[AddingListSelector.SelectedIndex];
            Song[] selectedSongs = GetSelection();
            AddSongs(targetList, selectedSongs);
        }

        private void OnDeleteSongsClicked(object sendor, RoutedEventArgs e)
        {
            Song[] selection = GetSelection();

            // When deleting from 'All Songs' list
            if (AllSongsListBox.SelectedItem != null)
            {
                DeleteSongsFromAll(selection);
                // Update the WPF
                SongsListBox.ItemsSource = null;
                SongsListBox.ItemsSource = this.AllSongs;
            }
            // When deleting from individual playlist
            else if (PlaylistsBox.SelectedItem != null)
            {
                Smpl playlist = Playlists[this.PlaylistsBox.SelectedIndex];
                DeleteSongsFromPlaylist(playlist, selection);
            }
        }

        // Internal Methods
        private void DisplaySelectedPlaylist()
        {
            AllSongsListBox.UnselectAll();
            Smpl playlist = Playlists[PlaylistsBox.SelectedIndex];
            SongsListBox.ItemsSource = playlist.members;
            SongsListBox.ScrollIntoView(SongsListBox.Items[0]);
            NameAndCountDisplay.Text = playlist.ToString() + "  " + playlist.members.Count;
        }
        
        private void DisplayAllSongs()
        {
            PlaylistsBox.UnselectAll();
            SongsListBox.ItemsSource = AllSongs;
            NameAndCountDisplay.Text = "All Songs  " + AllSongs.Count;
        }

        private Song[] GetSelection()
        {
            var selectionListInterface = SongsListBox.SelectedItems;
            Song[] selection = new Song[selectionListInterface.Count];
            selectionListInterface.CopyTo(selection, 0);
            return selection;
        }

        private void AddSongs(Smpl targetList, Song[] addition)
        {
            targetList.AddSongs(addition);
        }

        private void DeleteSongsFromAll(Song[] songsToDelete)
        {
            // Remove from playlists
            foreach (Smpl playlist in this.Playlists)
            {
                playlist.RemoveSongs(songsToDelete);
            }
            // Remove from all songs
            foreach (Song song in songsToDelete)
            {
                this.AllSongs.Remove(song);
            }
        }

        private void DeleteSongsFromPlaylist(Smpl playlist, Song[] SongsToDelete)
        {
            playlist.RemoveSongs(SongsToDelete);
            // Update the WPF
            SongsListBox.ItemsSource = null;
            SongsListBox.ItemsSource = playlist.members;
        }
    }
}
