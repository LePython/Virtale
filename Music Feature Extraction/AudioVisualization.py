import librosa
import matplotlib.pyplot as plt
from matplotlib import cm
import librosa.display
import numpy as np
import pylab
import os
from tqdm import tqdm

imagePATH = "Images/"
WAV_DIR = "audio_files/"

#Get list of all files in this folder
audio_files = os.listdir(WAV_DIR)


for f in tqdm(audio_files):
    try:
        y,sr = librosa.load(WAV_DIR + f, sr = 22050)

         # Pre-emphasis filter
        pre_emphasis = 0.97
        y = np.append(y[0], y[1:] - pre_emphasis * y[:-1])

        # Compute spectrogram
        M = librosa.feature.melspectrogram(y=y,sr=sr, fmax = sr/2, n_fft=2048)

        # Power in DB
        log_power = librosa.power_to_db(M, ref=np.max)# Covert to dB (log) scale

        # Plotting the spectrogram and save as JPG without axes (just the image)
        pylab.figure(figsize=(150,40))
        pylab.axis('off') 
        pylab.axes([0., 0., 1., 1.], frameon=False, xticks=[], yticks=[]) # Remove the white edge
        librosa.display.specshow(log_power, cmap=cm.jet)

        pylab.savefig(imagePATH + f[:-4] + ".jpg", bbox_inches=None, pad_inches=0)
        pylab.close()

    except Exception as e:
        print(f, e)
        pass