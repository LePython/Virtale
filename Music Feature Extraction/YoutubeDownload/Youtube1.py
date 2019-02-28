from __future__ import unicode_literals
from pydub import AudioSegment
import youtube_dl
import sys
import os
import json
import time

dir_path = os.path.dirname(os.path.realpath(__file__))

def download_wav(song_url, format):

    outtmpl = os.path.normpath(dir_path + "/audio_files/%(title)s.%(ext)s")
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

        info_dict = ydl.extract_info(song_url, download=False)
        return os.path.normpath(dir_path + "/audio_files/" + info_dict.get('title', None) + "." + format)


# opening the task file
with open(os.path.normpath(dir_path + "/configs/task.json")) as aFile:
    task = json.load(aFile)
    print(task)
    if task["stage"] == 'download':
        print("starting download")
        try:
            file = download_wav(task["url"],task["format"])   
        except Exception as e:
            print("The url is invalid. Please enter a valind url!", e)
        else:  
            next_file = { 
                "stage" : "analysis"
            }

            try:
                # openening task file to check if not overwriting something
                print("checking for running analysis")
                while next_file["stage"] != 'finished':
                    time.sleep(1)
                    f = open(os.path.normpath(dir_path + "/configs/task1.json"), "r")
                    next_file = json.load(f)

            except Exception as e:
                print("Something went wrong", e)

            else:

                try:
                    # overwriting current taskfile
                    print("updating current task")
                    task["stage"] = 'finished'
                    f = open(os.path.normpath(dir_path + "/configs/task.json"), "w")
                    f.write(json.dumps(task))
                except Exception as e:
                    print("Something went wrong", e)    

                else:

                    try:
                        # updating properties of json file
                        task["stage"] = 'analysis'
                        task["location"] = str(file)

                        # writing to new properties file for next step
                        print("updating next task")
                        f = open(os.path.normpath(dir_path + "/configs/task1.json"), "w")
                        f.write(json.dumps(task))

                    except Exception as e:
                        print("Something went wrong", e) 
                    else:
                        print("finished without errors")
                        
    else:
        print("invalid type!")
        

