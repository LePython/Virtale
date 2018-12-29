import os
import pickle
import numpy as np
import pandas as pd
import scipy
import librosa
import glob
from tqdm import tqdm
import json

dir_path = os.path.dirname(os.path.realpath(__file__))

# Get all wav files from that folder and store them in a list
songsList = glob.glob(dir_path + "\\..\\StreamingAssets\\AudioFiles\\*.wav")
print(songsList)

# Create a list to store analyzed features afterwards
analyzedFeatureList = []

# Extracting features from each an audio file in the list
for index, f in enumerate(tqdm(songsList)):
    try:
        # Read wav-file
        y, sr = librosa.load(f, sr = 22050)

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

    # This is a one digit number between 0-9
    result = pickle_model.predict(X_norm)[0]

    audioName = os.path.basename(f)


    featureDic = {
        "Name" : audioName,
        "FeatureGroup" : int(result)
    }
    analyzedFeatureList.append(featureDic)

# Open json file to write the results to
jsonFile = open("AnalyzedFeaturesList.json", "w")

json.dump(analyzedFeatureList, jsonFile, indent=4)

jsonFile.close()

