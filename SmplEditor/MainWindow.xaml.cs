using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
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

        private List<string> importedFileNames = new List<string>();
        public MainWindow()
        {
            // initializing
            while (this.playlistLibrary.Count == 0)
            {
                ImportPlaylist();
            }

            if (this.playlistLibrary.Count == 0)
            {
                MessageBox.Show("No Music Playlist detected. Exitting the application.");
                Application.Current.Shutdown();
            }
            InitializeComponent();

            // Initialize new playlist name(to not overlap with existing playlistname)
            NewPlaylistNameBox.Text = FetchNewPlaylistName();

            // Link the listbox contents with playlists and songs
            PlaylistsBox.ItemsSource = this.playlistLibrary;
            DisplayAllSongs();
            AddingListSelector.ItemsSource = this.playlistLibrary;
            AddingListSelector.SelectedIndex = 0;
        }

        // UI events
        private void OnPlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If not de-selection
            if (PlaylistsBox.SelectedItem != null)
            {
                this.DisplaySelectedPlaylist();
            }
        }

        private void OnAllSongsListSelectionChanged(object sendor, RoutedEventArgs e)
        {
            // If not deselection,
            if (AllSongsListBox.SelectedItem != null)
            {
                this.DisplayAllSongs();
            }

        }
        
        private void OnImportPlaylistClicked(object sender, RoutedEventArgs e)
        {
            ImportPlaylist();
            // Somehow, without setting it to null at first,
            // there will be an "inconsistency exception" when displaying the list
            // System.InvalidOperationException: An ItemsControl is inconsistent with its items source.
            PlaylistsBox.ItemsSource = null;
            PlaylistsBox.ItemsSource = this.playlistLibrary;
        }

        /// <summary> This function takes in a list of newly selected files and 
        /// separates them into `already imported` ones and `to be imported` ones. </summary>
        private List<int> getNewFileIndices(string[] selectedFileNames, List<string> importedFiles){
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

        /// <summary> 
        /// For each track in the imported list, search if the track already exists in the library.<br/>
        /// If any song was not found in the existing library, convert it to a &lt;Song&gt;.<br/>
        /// Return the importedPlaylist after converting it to a &lt;Playlist&gt; and connecting each track to the library.
        /// </summary>
        private List<Song> linkTracksToLibrary(Smpl importePlaylist, List<Song> library, List<Song> newSongs){
            List<Song> remappedPlaylist = new List<Song>();
            List<SmplSong> playlistSongs = importePlaylist.members;

            foreach (SmplSong targetSong in playlistSongs){
                Song matched = library.Find(libSong => targetSong.IsEqualTo(libSong));
                if(matched == default(Song)){ // not found
                    // convert the SmplSong to a Song and add to the list
                    matched = new Song(targetSong);
                    newSongs.Add(matched);
                    library.Add(matched);
                }
                else{ // existing song found on library
                    if(!matched.HasSmplSong()){ // if the song doesn't have SMPLSong, add the new information
                        matched.SmplMusic = targetSong;
                    }
                }
                remappedPlaylist.Add(matched);
            }
            return remappedPlaylist;
        }

        private Dictionary<ITunesLibraryParser.Track, Song> linkTracksToLibrary(List<ITunesLibraryParser.Track> importedTracks, List<Song> library, List<Song> newSongs){
            var linkedTracks = new Dictionary<ITunesLibraryParser.Track, Song>();
            foreach(var targetSong in importedTracks){
                Song matched = library.Find(libSong => libSong.IsEqualTo(targetSong));
                if(matched == default(Song)){
                    matched = new Song(targetSong);
                    newSongs.Add(matched);
                    library.Add(matched);
                }
                else{ // existing song found on library
                    if(!matched.HasITunesSong()){ // if the song doesn't have SMPLSong, add the new information
                        matched.ITunesSong = targetSong;
                    }
                }
                linkedTracks.Add(targetSong, matched);
            }
            return linkedTracks;
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

            List<int> newFileIndices = getNewFileIndices(fileNames, this.importedFileNames);
            foreach (int fileIdx in newFileIndices)
            {
                string safeName = safeNames[fileIdx];
                string fileName = fileNames[fileIdx];
                string extension = this.GetExtensionFromSafeName(safeName);
                if (extension == ".xml")
                {
                    // This file is iTunes file
                    var itunes = new ITunesLibraryParser.ITunesLibrary(fileName);
                    List<ITunesLibraryParser.Playlist> iPlaylists = itunes.Playlists.ToList();
                    List<ITunesLibraryParser.Track> iTracks = itunes.Tracks.ToList();
                    // Unlike smpl files, multiple playlists can come in at once.
                    // And all duplicateless track library comes separate to the playlists.
                    // Does iTunes library share the reference with the tracks in the playlist?
                    // No

                    // Registereing and linking can be done in the scale of the whole library
                    // While registering, create a map of iTunes.Track --> registered_Song
                    // There is a bit concern since the tracks from iTracks and iPlaylists
                    //    only equals in value but not in references.

                    // After registering, just go through the playlists
                    // Per playlist, map the iTunes.Tracks --> Registered_Song
                    List<Song> newSongs = new List<Song>();
                    var trackToSongLookup = this.linkTracksToLibrary(iTracks, this.songLibrary, newSongs);
                    foreach (var iPlaylist in iPlaylists)
                    {
                        Playlist playlist = new Playlist(iPlaylist, trackToSongLookup);
                        this.playlistLibrary.Add(playlist);
                    }
                }
                else if(extension == ".smpl")
                {
                    string jsonString = File.ReadAllText(fileNames[fileIdx]);
                    Smpl importingPlaylist = JsonSerializer.Deserialize<Smpl>(jsonString);

                    // If the playlist has no songs, skip the playlist
                    if (importingPlaylist.members.Count == 0) continue; 

                    // Separate new songs with the existing ones
                    // Remap the playlist to the library songs
                    // Add new songs to the library
                    List<Song> newSongs = new List<Song>();
                    List<Song> existingSameTypeSongs = new List<Song>();
                    List<Song> existingDiffTypeSongs = new List<Song>();
                    List<Song> linkedPlaylist = this.linkTracksToLibrary(importingPlaylist, this.songLibrary, newSongs);

                    Playlist playlist = new Playlist(importingPlaylist, linkedPlaylist);
                    this.playlistLibrary.Add(playlist);
                }
                else // Other than iTunes or SMPL
                {
                    System.Diagnostics.Trace.WriteLine("This file format not supported");
                }
                int breakHere = 1;
            }
            return;
        }

        private void OnAddSongsClicked(object sender, RoutedEventArgs e)
        {
            Playlist targetList = this.playlistLibrary[AddingListSelector.SelectedIndex];
            List<Song> selectedSongs = this.GetSongSelections();

            // The size of the playlist should not exceed 1,000 songs.
            // This is a restriction from the mobile application
            if (targetList.ListOfTracks.Count+selectedSongs.Count > 1000)
            {
                MessageBox.Show("This playlist already has " + targetList.ListOfTracks.Count + " tracks. Maximum number of tracks per playlist is 1000");
                return;
            }
            targetList.AddSongs(selectedSongs);

            // Go to the list that the song was just added
            PlaylistsBox.SelectedIndex = AddingListSelector.SelectedIndex;
            DisplaySelectedPlaylist();
        }

        private void OnDeleteSongsClicked(object sender, RoutedEventArgs e)
        {
            List<Song> selection = GetSongSelections();

            // When deleting from 'All Songs' list
            if (AllSongsListBox.SelectedItem != null)
            {
                // UI refresh is included
                DeleteSongsFromAll(selection);
            }
            // When deleting from individual playlist
            else if (PlaylistsBox.SelectedItem != null)
            {
                Playlist playlist = this.playlistLibrary[this.PlaylistsBox.SelectedIndex];

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
                songList.AddRange(GetSongSelections());
            }
            // The new playlist is instantiated with deep copy of the songList, which means they don't share the same reference.
            // The order is also reset.
            this.playlistLibrary.Add(new Playlist(name, songList));

            // Refresh UI
            PlaylistsBox.ItemsSource = null;
            PlaylistsBox.ItemsSource = this.playlistLibrary;
            NewPlaylistNameBox.Text = FetchNewPlaylistName();
            // Focus the view on the new playlist
            PlaylistsBox.SelectedIndex = this.playlistLibrary.Count - 1;
        }

        private void OnRemovePlaylistClicked(object sender, RoutedEventArgs e)
        {
            if (PlaylistsBox.SelectedItem != null)
            {
                int currentSelectionIndex = PlaylistsBox.SelectedIndex;
                Playlist targetPlaylist = this.playlistLibrary[currentSelectionIndex];
                DeletePlaylist(targetPlaylist.Name);

                // Refresh UI
                PlaylistsBox.ItemsSource = null;
                PlaylistsBox.ItemsSource = this.playlistLibrary;
                if (this.playlistLibrary.Count > 0)
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
            foreach (Playlist playlist in this.playlistLibrary)
            {
                string jsonString = "";
                if (playlist.IsSmpl)
                {
                    var jsonSerializerOption = new JsonSerializerOptions{
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    Smpl smplExport = new Smpl(playlist);
                    jsonString = JsonSerializer.Serialize(smplExport, jsonSerializerOption);
                }
                if (!Directory.Exists(folderBrowser.SelectedPath + "\\Exported_Smpl"))
                {
                    Directory.CreateDirectory(folderBrowser.SelectedPath + "\\Exported_Smpl");
                }
                File.WriteAllText(folderBrowser.SelectedPath + "\\Exported_Smpl\\"+playlist.Name+".smpl", jsonString);
                System.Diagnostics.Process.Start(folderBrowser.SelectedPath + "\\Exported_Smpl");
            }
        }

        private void OnSortOptionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedOption = SortOptionComboBox.SelectedIndex;
            switch (selectedOption){
                case 0: // by Order, directory
                // current structure of <Song> has a problem in this
                // Turns out, each song has its own "order" field per playlist
                // Currently, the "order" is considered as a <Song> object's property.
                // It's actually a property of each playlist with mapping to the songs.
                    {
                        // A song library does not have duplicates nor its own ordering
                        // (since ordering is an attribute of a playlist)
                        // There needs to be an alternative way to sort.
                        // The best I can think of now is by artist-title.
                        // Another Idea is Dir-title. Or jsut full path.
                        songLibrary.Sort((Song x, Song y) => x.CompareByOrder(y));
                        foreach (Playlist playlist in this.playlistLibrary)
                        {
                            playlist.SortByOrder();
                        }
                        break;
                    }
                case 1: // by artists
                    {
                        songLibrary.Sort((Song x, Song y) => x.CompareByArtist(y));
                        foreach (Playlist playlist in this.playlistLibrary)
                        {
                            playlist.SortByArtist();
                        }
                        break;
                    }
                case 2: // by title
                    {
                        songLibrary.Sort((Song x, Song y) => x.CompareByTitle(y));
                        foreach (Playlist playlist in this.playlistLibrary)
                        {
                            playlist.SortByTitle();
                        }
                        break;
                    }
                case 3: // by folder directory?
                    {
                        System.Diagnostics.Trace.WriteLine("CompareByDir: Not implemented. Cloning ByTitle behavior.");
                        songLibrary.Sort((Song x, Song y) => x.CompareByDir(y));
                        foreach (Playlist playlist in this.playlistLibrary)
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

        private void DisplaySelectedPlaylist()
        {
            AllSongsListBox.UnselectAll();
            Playlist playlist = this.playlistLibrary[PlaylistsBox.SelectedIndex]; //!!exception occurs when trying to delete from allsongs lists
            SongsListBox.ItemsSource = null;
            SongsListBox.ItemsSource = playlist.ListOfTracks;
            if (SongsListBox.Items.Count > 0)
            {
                SongsListBox.ScrollIntoView(SongsListBox.Items[0]);
            }
            NameAndCountDisplay.Text = playlist.ToString() + "  -  " + playlist.ListOfTracks.Count+" songs";
        }
        
        private void DisplayAllSongs()
        {
            PlaylistsBox.UnselectAll();
            SongsListBox.ItemsSource = null;
            SongsListBox.ItemsSource = songLibrary;
            NameAndCountDisplay.Text = "All Songs  -  " + songLibrary.Count+" songs";
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

        private List<Song> GetSongSelections()
        {
            var selectionListInterface = SongsListBox.SelectedItems;
            // Copies the reference of the Song objects
            List<Song> selections = selectionListInterface.OfType<Song>().ToList();
            return selections;
        }

        private void DeleteSongsFromAll(List<Song> songsToDelete)
        {
            // Remove from playlists
            foreach (Playlist playlist in this.playlistLibrary)
            {
                DeleteSongsFromPlaylist(playlist,songsToDelete);
            }
            // Remove from all songs
            foreach (Song track in songsToDelete)
            {
                this.songLibrary.Remove(track);
            }
            // UI refresh
            DisplayAllSongs();
        }

        private void DeleteSongsFromPlaylist(Playlist targetPlaylist, List<Song> SongsToDelete)
        {
            targetPlaylist.RemoveSongs(SongsToDelete);
            // UI refresh
            DisplaySelectedPlaylist();
        }

        private string FetchNewPlaylistName()
        {
            List<string> playlistNames = new List<string>();
            string newPlaylistName = "New Playlist 1";
            int count = 0;
            foreach (Playlist playlist in this.playlistLibrary)
            {
                playlistNames.Add(playlist.Name);
            }
            int newPlaylistIndex = 1;
            while (playlistNames.Find(playlistName => playlistName == newPlaylistName) != null)
            {
                newPlaylistName = "New Playlist " + newPlaylistIndex++;
            }
            return newPlaylistName;
        }

        public Boolean DeletePlaylist(string name)
        {
            Boolean deleted = false;
            foreach (Playlist playlist in this.playlistLibrary)
            {
                if (playlist.Name == name)
                {
                    this.playlistLibrary.Remove(playlist);
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
