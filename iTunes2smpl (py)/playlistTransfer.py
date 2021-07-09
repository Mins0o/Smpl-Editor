import plistlib
import urllib.parse
import os.path
import os
import json
import Levenshtein
import re

def iget_relevant_plists(xml_path):
    with open(xml_path, "rb") as xml_file:
        iLibrary =plistlib.load(xml_file)
    plentries = iLibrary["Playlists"]

    playlists=[]
    if_has_this_attribute_exclude = ["Folder", "Master", "Music", "Movies", "TV Shows", "Podcasts", "Audiobooks"]
    for plist in plentries:
        
        excluded_or_not = not plist["Name"]=="Downloaded"
        for i in if_has_this_attribute_exclude:
            excluded_or_not = excluded_or_not and (i not in plist)
        excluded_or_not = excluded_or_not and "Playlist Items" in plist
        
        if excluded_or_not:
            playlists.append(plist)
    return playlists

def iplaylist_items(playlist):
    return [i["Track ID"] for i in playlist["Playlist Items"]]

def iget_Track_Info(xml_path):
    with open(xml_path,"rb") as xmlfile:
        iLibrary = plistlib.load(xmlfile)
    return(iLibrary["Tracks"])

def smload_files(base_path, N = 5):
    smpl=[]
    for i in range(N):
        with open(base_path+"All{:02d}.smpl".format(i+1),"rb") as smpl_file:
            smpl+=json.load(smpl_file)["members"]
    return(smpl)

def match_title_and_artist_to_path(title, artist, smpl):
    best_match = None
    best_result = 0
    for i in smpl:
        curr_result = string_compare(title,i["title"]) * string_compare(artist,i["artist"])
        if  curr_result > best_result: 
            if curr_result > 0.8:
                best_match = i
            best_result = curr_result
    return best_match, best_result

def string_compare(str_a, str_b):
    return Levenshtein.ratio(str_a,str_b)
            

xml_path = "C:/Users/steng/OneDrive/Desktop/New folder/Library.xml"
base_path = "C:/Users/steng/OneDrive/Desktop/New folder/"

# list of dictionaries
# each dictionary is a playlist with
# {Name: , Description: , (Master: ,) Playlist ID: , Visible: , Playlist Items: , etc.}
iplaylists = iget_relevant_plists(xml_path)

# Each tracks have
# {Track ID: , Name: , Artist: , Album: , Location: , etc.} (may or may not have some fields)
itracks = iget_Track_Info(xml_path)

# list of songs from Samsung music playlist format
# each song has 
# {artist: , info: , order: , title: , type}
smpl = smload_files(base_path, 5)


for iplist in iplaylists:
    # per playlist
    if not os.path.isdir("./output_smpl"):
        os.mkdir("./output_smpl")
    smpl_name = "./output_smpl/"+re.sub(r'[^\w\s-]','_',iplist["Name"]) + ".smpl"
    with open(smpl_name,"w") as output_smpl:
        sm_members = []
        print(str(len(iplist["Playlist Items"])) + "\t{}".format(iplist["Name"]))
        
        # list of track ID (int) numbers in the playlist plist
        itracks_in_plist = iplaylist_items(iplist)

        # go through all the tracks in a playlist
        order = 1
        for track_num in itracks_in_plist:

            # get title and artist name
            itrack_title = itracks[str(track_num)]["Name"]
            try:
                itrack_artist = itracks[str(track_num)]["Artist"]
            except:
                itrack_artist = "\u003cunknown\u003e"

            # Get the best match of the two libraries file.
            # With this step, the necessary information of file path in Samsung phone is obtained.
            match, score = match_title_and_artist_to_path(itrack_title, itrack_artist, smpl)

            # debug purpose print. May require manual manipulation to fix smoothly
            sm_phone_path = None
            if match:
                sm_phone_path = match["info"]
            if score < 1:
                print("\t",track_num,"\t{:0.2f}".format(score), itrack_title," "*(110-len(itrack_title)),"\t", sm_phone_path)
            
            # if the match is not None, write it to the list
            if match:
                match["order"] = order
                order += 1
                sm_members.append(match)
        json.dump({"members":sm_members, "name":smpl_name, "recentlyPlayedDate": 0, "sortBy":4, "version": 1},output_smpl)