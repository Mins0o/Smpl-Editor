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
                    LoadSmpls();
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

            // If no playlists were loaded from LoadSmpls(), shutdown the app
            if (Playlists.Count == 0)
            {
                MessageBox.Show("No Samsung Music Playlist detected. Exitting the application.");
                Application.Current.Shutdown();
            }
            InitializeComponent();

            // Initialize new playlist name(to not overlap with existing playlistname)
            NewPlaylistNameBox.Text = FetchNewPlaylistName();

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
            OpenFileDialog openSmpls = new OpenFileDialog
            {
                Multiselect = true
            };
            openSmpls.ShowDialog();
            // Without full path, just names with extensions
            string[] shortNames = openSmpls.SafeFileNames;
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
                int extensionStartIndex = shortName.LastIndexOf('.');
                shortName = shortName.Substring(0,extensionStartIndex);
                //playlist.name = shortName;
                playlist.SortByArtist();

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

                // Don't add duplicate to the all songs list
                try
                {
                    List<Song> addList = new List<Song>();
                    foreach (Song member in playlist.members)
                    {
                        bool noDuplicate = true;
                        foreach (Song song in allSongs)
                        {
                            if (member.info == song.info)
                            {
                                noDuplicate = false;
                            }
                        }
                        if (noDuplicate)
                        {
                            // The songs added to the allsongs list will be a deep copy(different reference, same values) of the things in the playlists
                            addList.Add(member.DeepCopy());
                        }
                    }
                    allSongs.AddRange(addList);
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message);
                }


            }
            allSongs.Sort((Song x,Song y)=>x.artist.CompareTo(y.artist));
            this.AllSongs = allSongs;
            this.Playlists = playlists;
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
            // If not deselection,
            if (AllSongsListBox.SelectedItem != null)
            {
                DisplayAllSongs();
            }

        }

        private void OnAddSongsClicked(object sender, RoutedEventArgs e)
        {
            Smpl targetList = Playlists[AddingListSelector.SelectedIndex];
            Song[] selectedSongs = GetSelection();

            // The size of the playlist should not exceed 1,000 songs.
            if (targetList.members.Count+selectedSongs.Length > 1000)
            {
                MessageBox.Show("This playlist already has " + targetList.members.Count + "songs. Maximum number of songs per a playlist is 1000");
                return;
            }
            // Every copy of song in different lists are different instances. The deep copying is handled inside the method.
            targetList.AddSongs(selectedSongs);

            // Go to the list that the song was just added
            PlaylistsBox.SelectedIndex = AddingListSelector.SelectedIndex;
            DisplaySelectedPlaylist();
        }

        private void OnDeleteSongsClicked(object sender, RoutedEventArgs e)
        {
            Song[] selection = GetSelection();

            // When deleting from 'All Songs' list
            if (AllSongsListBox.SelectedItem != null)
            {
                // UI refresh is included
                DeleteSongsFromAll(selection);
            }
            // When deleting from individual playlist
            else if (PlaylistsBox.SelectedItem != null)
            {
                Smpl playlist = Playlists[this.PlaylistsBox.SelectedIndex];

                // UI refresh is included
                DeleteSongsFromPlaylist(playlist, selection);
            }
        }

        private void OnCreatePlaylistClicked(object sender, RoutedEventArgs e)
        {
            string name = NewPlaylistNameBox.Text;
            List<Song> songList = new List<Song>();
            if (SongsListBox.SelectedItems != null)
            {
                songList.AddRange(GetSelection());
            }
            // The new playlist is instantiated with deep copy of the songList, which means they don't share the same reference.
            // The order is also reset.
            Playlists.Add(new Smpl(name, songList));

            // Refresh UI
            PlaylistsBox.ItemsSource = null;
            PlaylistsBox.ItemsSource = Playlists;
            NewPlaylistNameBox.Text = FetchNewPlaylistName();
            // Focus the view on the new playlist
            PlaylistsBox.SelectedIndex = Playlists.Count - 1;
        }

        private void OnRemovePlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (PlaylistsBox.SelectedItem != null)
            {
                int currentSelectionIndex = PlaylistsBox.SelectedIndex;
                Smpl targetPlaylist = Playlists[currentSelectionIndex];
                DeletePlaylist(targetPlaylist.name);

                // Refresh UI
                PlaylistsBox.ItemsSource = null;
                PlaylistsBox.ItemsSource = Playlists;
                if (Playlists.Count > 0)
                {
                    PlaylistsBox.SelectedIndex = currentSelectionIndex - 1;
                }
                else
                {
                    PlaylistsBox.UnselectAll();
                }
            }
        }

        private void OnExportButtonClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description =
            "Select the directory to save the playlists",
                ShowNewFolderButton=true
            };
            folderBrowser.ShowDialog();
            foreach (Smpl playlist in Playlists)
            {
                string jsonString = JsonSerializer.Serialize(playlist);
                if (!Directory.Exists(folderBrowser.SelectedPath + "\\Exported_Smpl"))
                {
                    Directory.CreateDirectory(folderBrowser.SelectedPath + "\\Exported_Smpl");
                }
                File.WriteAllText(folderBrowser.SelectedPath + "\\Exported_Smpl\\"+playlist.name+".smpl", jsonString);
                System.Diagnostics.Process.Start(folderBrowser.SelectedPath + "\\Exported_Smpl");
            }
        }

        private void OnSortOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedOption = SortOptionComboBox.SelectedIndex;
            switch (selectedOption){
                case 0:
                    {
                        AllSongs.Sort((Song x, Song y) => {
                            if (x.UpperDirectory().CompareTo(y.UpperDirectory()) == 0) {
                                return x.order.CompareTo(y.order);
                            } else {
                                return x.UpperDirectory().CompareTo(y.UpperDirectory());
                            }
                        }
                        );
                        foreach (Smpl playlist in Playlists)
                        {
                            playlist.SortByOrder();
                        }
                        break;
                    }
                case 1:
                    {
                        AllSongs.Sort((Song x, Song y) => x.artist.CompareTo(y.artist));
                        foreach (Smpl playlist in Playlists)
                        {
                            playlist.SortByArtist();
                        }
                        break;
                    }
                case 2:
                    {
                        AllSongs.Sort((Song x, Song y) => x.title.CompareTo(y.title));
                        foreach (Smpl playlist in Playlists)
                        {
                            playlist.SortByTitle();
                        }
                        break;
                    }
                case 3:
                    {
                        AllSongs.Sort((Song x, Song y) => x.info.CompareTo(y.info));
                        foreach (Smpl playlist in Playlists)
                        {
                            playlist.SortByDirectory();
                        }
                        break;
                    }
                default:
                    return;
            }
            RefreshDisplay();
        }

        // Internal Methods
        private void DisplaySelectedPlaylist()
        {
            AllSongsListBox.UnselectAll();
            Smpl playlist = Playlists[PlaylistsBox.SelectedIndex];
            SongsListBox.ItemsSource = null;
            SongsListBox.ItemsSource = playlist.members;
            if (SongsListBox.Items.Count > 0)
            {
                SongsListBox.ScrollIntoView(SongsListBox.Items[0]);
            }
            NameAndCountDisplay.Text = playlist.ToString() + "  -  " + playlist.members.Count+" songs";
        }
        
        private void DisplayAllSongs()
        {
            PlaylistsBox.UnselectAll();
            SongsListBox.ItemsSource = null;
            SongsListBox.ItemsSource = AllSongs;
            NameAndCountDisplay.Text = "All Songs  -  " + AllSongs.Count+" songs";
        }

        private void RefreshDisplay()
        {
            if (AllSongsListBox == null || PlaylistsBox == null)
            {
                return;
            }
            if (AllSongsListBox.SelectedItem != null)
            {
                DisplayAllSongs();
            }
            else if (PlaylistsBox.SelectedItem != null)
            {
                DisplaySelectedPlaylist();
            }
        }

        private Song[] GetSelection()
        {
            var selectionListInterface = SongsListBox.SelectedItems;
            Song[] selection = new Song[selectionListInterface.Count];
            // Copies the reference of the Song objects
            selectionListInterface.CopyTo(selection, 0);
            return selection;
        }

        private void DeleteSongsFromAll(Song[] songsToDelete)
        {
            // Remove from playlists
            foreach (Smpl playlist in this.Playlists)
            {
                DeleteSongsFromPlaylist(playlist,songsToDelete);
            }
            // Remove from all songs
            foreach (Song song in songsToDelete)
            {
                this.AllSongs.Remove(song);
            }
            // UI refresh
            DisplayAllSongs();
        }

        private void DeleteSongsFromPlaylist(Smpl targetPlaylist, Song[] SongsToDelete)
        {
            targetPlaylist.RemoveSongs(SongsToDelete);
            // UI refresh
            DisplaySelectedPlaylist();
        }

        private string FetchNewPlaylistName()
        {
            string[] playlistNames = new string[Playlists.Count];
            string newPlaylistName = "New Playlist 1";
            int count = 0;
            foreach (Smpl playlist in Playlists)
            {
                playlistNames[count++] = playlist.name;
            }
            int newPlaylistIndex = 1;
            while (Array.Find(playlistNames, pn => pn == newPlaylistName) != null)
            {
                newPlaylistName = "New Playlist " + newPlaylistIndex++;
            }
            return newPlaylistName;
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
    }
}
