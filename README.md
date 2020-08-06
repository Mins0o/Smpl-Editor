# Smpl-Editor
A tool to make smpl editing for Samsung Music Player easier

# Features
- Load playlists from .smpl files
- View playlist contents(songs)
- Add or remove songs from playlists or from all songs
- Create new playlists or delete existing playlists
- Export the playlists into .smpl format
- Sort songs according to various tags


# .smpl files json structure
.smpl  
|- members:List of jsons  
|\_\_L song  
|\_\_\_\_|- artist:string  
|\_\_\_\_|- info\:stirng  
|\_\_\_\_|- order:int  
|\_\_\_\_|- title:string  
|\_\_\_\_L type:int  
|- recentlyPlayedDate:int  
|- name:string  
|- sortBy:int  
L version:int  
Documentation in progress(2020.07.29)...  
