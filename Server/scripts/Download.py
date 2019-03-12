from __future__ import unicode_literals
from pydub import AudioSegment
import youtube_dl
import sys
import os
import json
import time

dir_path = os.path.dirname(os.path.realpath(__file__))

def download_wav(song_url, format):

    outtmpl = os.path.normpath(dir_path + "/../audio_files/%(title)s.%(ext)s")
    print(outtmpl)
    ydl_opts = {
        "format": "bestaudio/best",
        "outtmpl": outtmpl,
        "postprocessors": [{
            "key": "FFmpegExtractAudio",
            "preferredcodec": format
        }],
        "ignoreerrors":True
    }

    with youtube_dl.YoutubeDL(ydl_opts) as ydl:
        ydl.download([song_url])


# opening the task file
with open(os.path.normpath(dir_path + "/../configs/task.json")) as aFile:
    task = json.load(aFile)
    print(task)
    if task["stage"] == 'download':
        print("starting download")
        try:
            download_wav(task["url"],task["format"])   
        except Exception as e:
            print("The url is invalid. Please enter a valind url!", e)
        else:

            try:
                # overwriting current taskfile
                print("updating current task")
                task["stage"] = 'finished'
                f = open(os.path.normpath(dir_path + "/../configs/task.json"), "w")
                f.write(json.dumps(task))
            except Exception as e:
                print("Something went wrong", e)    
            else:
                print("finished without errors")
                        
    else:
        print("invalid type!")
        

