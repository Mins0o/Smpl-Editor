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
        private List<Playlist> playlistLibrary = new List<Playlist>();
        private List<Song> songLibrary = new List<Song>();

        private Song matchSong(Song lookingFor, List<Song> list) {
            Song match = new Song();
            if (true) {
                return null;
            }
            return match;
        }

        // Playlists : List of playlists throughtout the application
        private List<Smpl> playlists;// = new List<Smpl>();
        private List<SmplSong> allSongs = new List<SmplSong>();
        private List<string> prevImportFileName = new List<string>();
        public MainWindow()
        {
            ImportPlaylist();
            // initializing
            while (playlists == null)
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
            if (playlists.Count == 0)
            {
                MessageBox.Show("No Samsung Music Playlist detected. Exitting the application.");
                Application.Current.Shutdown();
            }
            InitializeComponent();

            // Initialize new playlist name(to not overlap with existing playlistname)
            NewPlaylistNameBox.Text = FetchNewPlaylistName();

            // Link the listbox contents with playlists and songs
            PlaylistsBox.ItemsSource = playlists;
            DisplayAllSongs();
            AddingListSelector.ItemsSource = playlists;
            AddingListSelector.SelectedIndex = 0;
        }

        /// <summary>This function prompts OpenFileDialog and loads file with SMPL information.<br/>
        /// Playlist will be created as the file name and its members will be added into the Smpl class.</summary>
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
            List<SmplSong> allSongs = new List<SmplSong>();

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
                    List<SmplSong> addList = new List<SmplSong>();
                    foreach (SmplSong member in playlist.members)
                    {
                        bool noDuplicate = true;
                        foreach (SmplSong song in allSongs)
                        {
                            if (member.info == song.info)
                            {
                                noDuplicate = false;
                            }
                        }
                        if (noDuplicate)
                        {
                            // The songs added to the allsongs list will be a deep copy(different reference, same values) of the things in the playlists
                            addList.Add(member.DeepCopy()); //!!why?
                        }
                    }
                    allSongs.AddRange(addList);
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message);
                }


            }
            // sort by artist
            allSongs.Sort((SmplSong x,SmplSong y)=>x.artist.CompareTo(y.artist));
            this.allSongs = allSongs;
            this.playlists = playlists;
        }

        // UI events
        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If not de-selection //!! ?? why do I need this?
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
        private void OnImportPlaylistClicked(object sender, RoutedEventArgs e)
        {
            ImportPlaylist();
        }
        /// <summary> This function takes in a list of newly selected files and 
        /// separates them into `already imported` ones and `to be imported` ones. </summary>
        private List<int> FilterNewFileIndices(string[] selectedFileNames, List<string> importedFiles){
            // name? extension? check dups
            List<int> availableIndices = new List<int>();
            int listSize = selectedFileNames.Length;
            for(int ii = 0; ii < listSize; ++ii){
                string fileName = selectedFileNames[ii];
                if(importedFiles.IndexOf(fileName) >= 0){
                    continue;
                }
                else{
                    availableIndices.Add(ii);
                }
            }
            return availableIndices;
        }
        
        private void AddToImportedFiles(string[] selectedFileNames, List<int> processedIndices){
            foreach(int fileIndex in processedIndices){
                this.prevImportFileName.Add(selectedFileNames[fileIndex]);
            }
        }

        private string GetExtensionFromSafeName(string safeName){
            int extStartIndex = safeName.LastIndexOf(".");
            string extension = safeName.Substring(extStartIndex);
            return extension;
        }
        
        private string GetPlaylistNameFromSafeName(string safeName){
            int extStartIndex = safeName.LastIndexOf(".");
            string playlistName = safeName.Substring(0, extStartIndex);
            return playlistName;
        }

        // private List<T> addNewSongsToLibrary<T,P>(P palylist, List<T> library)
        // where T:Song
        // where P:Playlist
        // {
        //     return new List<T>();
        // }
        private List<Song> addNewSongsToLibrary(Smpl playlist, List<Song> library){
            List<Song> newSongsList = new List<Song>();
            List<Song> playlistSongs = playlist.members;
            foreach (Song song in playlistSongs){
                Song matched = this.matchSong(song, library);
                if(matched != null){ // More like, if it is matched
                    ;
                }
                else{
                    library.Add(song);
                    newSongsList.Add(song);
                }
            }
            return newSongsList;
        }
        private List<Song> addNewSongsToLibrary(ITunesPlaylist playlist, List<Song> library){
            List<Song> newSongsList = new List<Song>();
            return newSongsList;
        }
        private Playlist getPlaylist(Smpl playlist, List<Song> library){
            Playlist newPlaylist = new Playlist();
            return newPlaylist;
        }
        private Playlist getPlaylist(ITunesPlaylist playlist){
            Playlist newPlaylist = new Playlist();
            return newPlaylist;
        }
        private void ImportPlaylist()
        {
            {/* Pseudo code (all comments)
             * 1. Select files
             * 2. Check if file has been imported already
             *    If imported before, skip that file
             * 3. If file should be imported, parse the file
             *    During this procedure both iTunes library and .smpl will create playlist(s) and songs.
             *      Their formats differ by file type (iTunes/smpl).
             * 4. If a playlist with same name and file type exists, skip
             * 5. If the playlist is new, add the playlist into the playlist library.
             *     Playlist from smpl and iTunes are different.
             *     --- How should a playlist from iTunes work??
             *       -- can be edited
             *       -- can be created anew
             *       -- can be removed (easy)
             *       -- can be exported into .xml
             *       -- can be converted to smpl (same for smpl --> iTunes)
             *          -- only if all the songs in the playlist has its mapping back and forth.
             *     --- For the requirements, should a new class be generated? 
             *          or the existing automatic data structure support these features?
             *          If it does not support this features, for every iTunes playlist, create a custom playlist object.
             *     --- What about folders in iTunes?
             *       -- For now, folders will be ignored. Only the leaf playlist will be exportable.
             *     Do this for all the playlists, accumulate all the songs that are not duplicates.
             *     if song is duplicate among import, don't add to the cumulative list.
             * 6. For all the songs in the new cumulative list, check if they already exist in the song library.
             *     song for different file type should be comparable with:
             *       1) Title and Artist
             *       2) Filename (not full path, but file name)
             *     If same song exist,
             *       If different file format, add the new format entry to the libraries existing song
             *        -- this is the mapping. mark the song as mapped.
             *       If same file format, discard (or maybe collectively notify the user about it)
             * 7. If not, 
             *    For smpl songs, just store the automatically generated objects references.
             *    For iTuens songs, create a new song object and put its reference 
            */
            }

            // Get the file paths to load.
            OpenFileDialog openFiles = new OpenFileDialog
            {
                Multiselect = true
            };
            openFiles.ShowDialog();
            // Without full path, just names with extensions
            string[] safeNames = openFiles.SafeFileNames;
            // Full path
            string[] fileNames = openFiles.FileNames;

            List<int> newFileIndices = FilterNewFileIndices(fileNames, prevImportFileName);
            List<Song> newSongToLibrary = new List<Song>();
            foreach (int fileIdx in newFileIndices)
            {
                string safeName = safeNames[fileIdx];
                string fileName = fileNames[fileIdx];
                string extension = this.GetExtensionFromSafeName(safeName);
                if (extension == ".xml"){
                    // this file is iTunes file
                }
                else if(extension == ".smpl"){
                    string jsonString = File.ReadAllText(fileNames[fileIdx]);
                    // This playlist object will be just temporary.
                    Smpl importingPlaylist = JsonSerializer.Deserialize<Smpl>(jsonString);
                    List<Song> newSongs = addNewSongsToLibrary(importingPlaylist, songLibrary);
                    Playlist playlist = getPlaylist(importingPlaylist, songLibrary);
                    // add the playlist to the playlist library
                    // If it already exist, just do the latter part
                    // There must a temporary song class only for getting the raw formats, for each
                    // and there should be a universal one, that is actually stored in the library.
                    // playlist also.
                }
                else // Other than iTunes or SMPL
                {
                    System.Diagnostics.Trace.WriteLine("This file format not supported");
                }
                int break_here = 1;
            }
            return;
        }

        private void OnAddSongsClicked(object sender, RoutedEventArgs e)
        {
            Smpl targetList = playlists[AddingListSelector.SelectedIndex];
            SmplSong[] selectedSongs = GetSelection();

            // The size of the playlist should not exceed 1,000 songs.
            // This is a restriction from the mobile application
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
            SmplSong[] selection = GetSelection();

            // When deleting from 'All Songs' list
            if (AllSongsListBox.SelectedItem != null)
            {
                // UI refresh is included
                DeleteSongsFromAll(selection);
            }
            // When deleting from individual playlist
            else if (PlaylistsBox.SelectedItem != null)
            {
                Smpl playlist = playlists[this.PlaylistsBox.SelectedIndex];

                // UI refresh is included
                DeleteSongsFromPlaylist(playlist, selection);
            }
        }

        private void OnCreatePlaylistClicked(object sender, RoutedEventArgs e)
        {
            string name = NewPlaylistNameBox.Text;
            List<SmplSong> songList = new List<SmplSong>();
            if (SongsListBox.SelectedItems != null)
            {
                songList.AddRange(GetSelection());
            }
            // The new playlist is instantiated with deep copy of the songList, which means they don't share the same reference.
            // The order is also reset.
            playlists.Add(new Smpl(name, songList));

            // Refresh UI
            PlaylistsBox.ItemsSource = null;
            PlaylistsBox.ItemsSource = playlists;
            NewPlaylistNameBox.Text = FetchNewPlaylistName();
            // Focus the view on the new playlist
            PlaylistsBox.SelectedIndex = playlists.Count - 1;
        }

        private void OnRemovePlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (PlaylistsBox.SelectedItem != null)
            {
                int currentSelectionIndex = PlaylistsBox.SelectedIndex;
                Smpl targetPlaylist = playlists[currentSelectionIndex];
                DeletePlaylist(targetPlaylist.name);

                // Refresh UI
                PlaylistsBox.ItemsSource = null;
                PlaylistsBox.ItemsSource = playlists;
                if (playlists.Count > 0)
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
            foreach (Smpl playlist in playlists)
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
                        allSongs.Sort((SmplSong x, SmplSong y) => {
                            if (x.UpperDirectory().CompareTo(y.UpperDirectory()) == 0) {
                                return x.order.CompareTo(y.order);
                            } else {
                                return x.UpperDirectory().CompareTo(y.UpperDirectory());
                            }
                        }
                        );
                        foreach (Smpl playlist in playlists)
                        {
                            playlist.SortByOrder();
                        }
                        break;
                    }
                case 1:
                    {
                        allSongs.Sort((SmplSong x, SmplSong y) => x.artist.CompareTo(y.artist));
                        foreach (Smpl playlist in playlists)
                        {
                            playlist.SortByArtist();
                        }
                        break;
                    }
                case 2:
                    {
                        allSongs.Sort((SmplSong x, SmplSong y) => x.title.CompareTo(y.title));
                        foreach (Smpl playlist in playlists)
                        {
                            playlist.SortByTitle();
                        }
                        break;
                    }
                case 3:
                    {
                        allSongs.Sort((SmplSong x, SmplSong y) => x.info.CompareTo(y.info));
                        foreach (Smpl playlist in playlists)
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
            Smpl playlist = playlists[PlaylistsBox.SelectedIndex]; //!!exception occurs when trying to delete from allsongs lists
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
            SongsListBox.ItemsSource = allSongs;
            NameAndCountDisplay.Text = "All Songs  -  " + allSongs.Count+" songs";
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

        private SmplSong[] GetSelection()
        {
            var selectionListInterface = SongsListBox.SelectedItems;
            SmplSong[] selection = new SmplSong[selectionListInterface.Count];
            // Copies the reference of the Song objects
            selectionListInterface.CopyTo(selection, 0);
            return selection;
        }

        private void DeleteSongsFromAll(SmplSong[] songsToDelete)
        {
            // Remove from playlists
            foreach (Smpl playlist in this.playlists)
            {
                DeleteSongsFromPlaylist(playlist,songsToDelete);
            }
            // Remove from all songs
            foreach (SmplSong song in songsToDelete)
            {
                this.allSongs.Remove(song);
            }
            // UI refresh
            DisplayAllSongs();
        }

        private void DeleteSongsFromPlaylist(Smpl targetPlaylist, SmplSong[] SongsToDelete)
        {
            targetPlaylist.RemoveSongs(SongsToDelete);
            // UI refresh
            DisplaySelectedPlaylist();
        }

        private string FetchNewPlaylistName()
        {
            string[] playlistNames = new string[playlists.Count];
            string newPlaylistName = "New Playlist 1";
            int count = 0;
            foreach (Smpl playlist in playlists)
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
            foreach (Smpl playlist in playlists)
            {
                if (playlist.name == name)
                {
                    playlists.Remove(playlist);
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
