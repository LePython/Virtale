import os
import pickle
import numpy as np
import pandas as pd
import scipy
import librosa

dir_path = os.path.dirname(os.path.realpath(__file__))

audioRelativePath = input()
audioFileDirectory = dir_path + "\\" + audioRelativePath

# Extracting features from an audio file

try:
    # Read wav-file
    y, sr = librosa.load(audioFileDirectory, sr = 22050)

    feature_list = []

    tempo = librosa.beat.tempo(y, sr=sr)
    feature_list.extend(tempo)

    spectral_contrast = librosa.feature.spectral_contrast(y, sr=sr, n_bands = 6, fmin = 200.0)
    feature_list.append(np.mean(spectral_contrast[1]))
    feature_list.append(np.mean(spectral_contrast[2]))
    feature_list.append(np.mean(spectral_contrast[3]))

    mfccs = librosa.feature.mfcc(y, sr=sr, n_mfcc=20)
    feature_list.append(np.mean(mfccs[4]))
    feature_list.append(np.std(mfccs[1]))
    feature_list.append(np.std(mfccs[5]))


    feature_list[1:] = np.round(feature_list[1:], decimals=3)

except:
    pass

features = np.array([feature_list])

### Using K-means to
### analyze the features
### of the audio file


pkl_filename = dir_path + "\\kmeans_final_model.pkl"

with open(pkl_filename, 'rb') as file:  
    pickle_model = pickle.load(file)

# Die Zahlen kommen aus dem Datensatz und seinem Minimum/Maximum

X_norm = (features - (-27.943))/(172.266 - (-27.943))

result = pickle_model.predict(X_norm)[0]

print(result)