import pickle
import numpy as np
import os

dir_path = os.path.dirname(os.path.realpath(__file__))
cwd = os.getcwd()

songFeatures = [[117.454,13.957,17.663,16.884,12.936,43.169,23.598]]
np_features = np.array(songFeatures)

pkl_filename = dir_path + "\\kmeans_final_model.pkl"

with open(pkl_filename, 'rb') as file:  
    pickle_model = pickle.load(file)

X_norm = (np_features - np_features.min())/(np_features.max() - np_features.min())

result = pickle_model.predict(X_norm)[0]

print(result)