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


# Currently existings songs in the songs folder
existingSongsNames = []

# Get all wav files from that folder and store them in a list
songsList = glob.glob(os.path.normpath(dir_path + "/../audio_files/*.mp3"))
for i in songsList:
    existingSongsNames.append(os.path.basename(i))


# A list of audio files that have been already analyzed
analyzedDataListNames = []

# Open json file to get already analyzed files
with open(os.path.normpath(dir_path + "/../configs/AnalyzedFeaturesList.json"),"r") as aFile:
    loadedData = json.load(aFile)
    for p in loadedData:
        analyzedDataListNames.append(p["name"])

# Check if existing files have been analyzed
def IsAnalysisRequired():
    for obj in existingSongsNames:
        if not obj in analyzedDataListNames:
            return True

# Check if there are files for analysis
# if there are none, exit the script
if IsAnalysisRequired() != True:
    print("There is nothing to analyze!")
    exit()

print("Analysis is required. Initiating analysis...")
print("This might take a while. It depends on how many songs have to be analyzed. Be Patient.")
# Create a list to store analyzed features afterwards
analyzedFeatureList = []

# Extracting features from each audio file in the list
for index, f in enumerate(songsList):
    # audio file name
    audioName = os.path.basename(f)

    if audioName in analyzedDataListNames:
        continue

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

        print(index + "Song" + feature_list[0] + " was analyzed succesfully.")

    except:
        pass
    
    features = np.array([feature_list])

    ### Using K-means to
    ### analyze the features
    ### of the audio file


    pkl_filename = os.path.normpath(dir_path + "/../configs/kmeans_final_model.pkl")

    with open(pkl_filename, 'rb') as file:  
        pickle_model = pickle.load(file)

    # Die Zahlen kommen aus dem Datensatz und seinem Minimum/Maximum

    X_norm = (features - (-27.943))/(172.266 - (-27.943))

    # This is a one digit number between 0-9
    result = pickle_model.predict(X_norm)[0]


    featureDic = {
        "name" : audioName,
        "featuregroup" : int(result)
    }
    analyzedFeatureList.append(featureDic)

# Open the json file and copy its content
with open(os.path.normpath(dir_path + "/../configs/AnalyzedFeaturesList.json"),"r") as f:
    data = json.load(f)

# Add new analyzed songs into the list of previously analyzed ones
finalList = data + analyzedFeatureList

# Open the json file and rewrite its content to update the list
with open(os.path.normpath(dir_path + "/../configs/AnalyzedFeaturesList.json"), "w+") as f:
    json.dump(finalList, f, indent=4)

with open(os.path.normpath(dir_path + "/../configs/NewFeatures.json"),"w+") as f:
    json.dump(analyzedFeatureList,f)
