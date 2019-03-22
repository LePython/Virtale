import os
import numpy as np
import pandas as pd
import scipy
import librosa
from tqdm import tqdm


audioDir = 'audio_files/'
wav_files = os.listdir(audioDir)

col_names = ['file_name', 'tempo', 'spectral_contrast_1_mean', 'spectral_contrast_2_mean', 'spectral_contrast_3_mean', 'mfccs_4_mean', 'mfccs_1_std', 'mfccs_5_std']
 

df = pd.DataFrame(columns=col_names)

for f in tqdm(wav_files):
    try:
        # Read wav-file
        y, sr = librosa.load(audioDir+f, sr = 22050)
        
        feature_list = [f]

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
    
    df = df.append(pd.DataFrame(feature_list, index=col_names).transpose(), ignore_index=True)

# Save file
df.to_csv('df_features.csv', index=False)