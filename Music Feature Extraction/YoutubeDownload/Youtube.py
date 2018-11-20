from __future__ import unicode_literals
from pydub import AudioSegment
import youtube_dl
import sys
import os

def download_wav(song_url, format):

    outtmpl = os.path.normpath(os.getcwd() + os.sep + os.pardir + "/audio_files/%(title)s.%(ext)s")
    print(outtmpl)
    ydl_opts = {
        "format": "bestaudio/best",
        "outtmpl": outtmpl,
        "postprocessors": [{
            "key": "FFmpegExtractAudio",
            "preferredcodec": format
        }],
    }

    with youtube_dl.YoutubeDL(ydl_opts) as ydl:
        ydl.download([song_url])


while True:
    try:
        print("Enter song url: ")
        url = input()
        print("Enter the audio format: ")
        aformat = input() 
        download_wav(url, aformat)
        break
    except Exception as e:
        print (e)
        os.system("cls")
        print("The url is invalid. Please enter a valind url!")



