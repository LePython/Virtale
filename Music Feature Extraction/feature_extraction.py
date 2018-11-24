import os
import numpy as np
import pandas as pd
import scipy
import librosa
from tqdm import tqdm


WAV_DIR = 'audio_files/'
wav_files = os.listdir(WAV_DIR)

# col_names = ['file_name', 'signal_mean', 'signal_std', 'signal_skew', 'signal_kurtosis', 
#              'zcr_mean', 'zcr_std', 'rmse_mean', 'rmse_std', 'tempo',
#              'spectral_centroid_mean', 'spectral_centroid_std',
#              'spectral_bandwidth_2_mean', 'spectral_bandwidth_2_std',
#              'spectral_bandwidth_3_mean', 'spectral_bandwidth_3_std',
#              'spectral_bandwidth_4_mean', 'spectral_bandwidth_4_std'] + \
#             ['spectral_contrast_' + str(i+1) + '_mean' for i in range(7)] + \
#             ['spectral_contrast_' + str(i+1) + '_std' for i in range(7)] + \
#             ['spectral_rolloff_mean', 'spectral_rolloff_std'] + \
#             ['mfccs_' + str(i+1) + '_mean' for i in range(20)] + \
#             ['mfccs_' + str(i+1) + '_std' for i in range(20)] + \
#             ['chroma_stft_' + str(i+1) + '_mean' for i in range(12)] + \
#             ['chroma_stft_' + str(i+1) + '_std' for i in range(12)] 

col_names = ['file_name', 'signal_mean', 'zcr_mean', 'rmse_mean', 'tempo', 'spectral_centroid_mean', 'spectral_contrast_mean', 'mfccs_mean']
            
            
df = pd.DataFrame(columns=col_names)

for f in tqdm(wav_files):
    try:
        # Read wav-file
        y, sr = librosa.load(WAV_DIR+f, sr = 22050)
        
        feature_list = [f]

        feature_list.append(np.mean(abs(y)))

        zcr = librosa.feature.zero_crossing_rate(y + 0.0001, frame_length=2048, hop_length=512)[0]
        feature_list.append(np.mean(zcr))

        rmse = librosa.feature.rmse(y + 0.0001)[0]
        feature_list.append(np.mean(rmse))
        
        tempo = librosa.beat.tempo(y, sr=sr)
        feature_list.extend(tempo)

        spectral_centroids = librosa.feature.spectral_centroid(y+0.01, sr=sr)[0]
        feature_list.append(np.mean(spectral_centroids))

        spectral_contrast = librosa.feature.spectral_contrast(y, sr=sr, n_bands = 6, fmin = 200.0)
        feature_list.append(np.mean(spectral_contrast))

        mfccs = librosa.feature.mfcc(y, sr=sr, n_mfcc=20)
        feature_list.append(np.mean(mfccs))
        
        feature_list[1:] = np.round(feature_list[1:], decimals=3)
        
    except:
        pass
    
    df = df.append(pd.DataFrame(feature_list, index=col_names).transpose(), ignore_index=True)

# Save file
df.to_csv('df_features.csv', index=False)