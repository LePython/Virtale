#!/bin/bash

mv AnalyzedFeaturesList.json configs/AnalyzedFeaturesList.json

rm CategorizeSong.py
rm Download.py
rm install_linux.sh
rm su_install_linux.sh
rm kmeans_final_model.pkl
rm requirements.txt

unzip -o update.zip
