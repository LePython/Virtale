package main

import (
	"encoding/json"
	"fmt"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"os/exec"
	"strings"
	"time"
)

func songListHandler(w http.ResponseWriter, req *http.Request) {
	// Import AnalyzedFeaturesList.json
	file, err := os.Open("configs/AnalyzedFeaturesList.json") // For read access.
	if err != nil {
		log.Fatal(err)
	}
	data := make([]byte, 10000)
	count, err := file.Read(data)
	if err != nil {
		log.Fatal(err)
	}

	//decode json
	var f []feature

	json.Unmarshal(data[0:count], &f)

	// Set the content type to json and send response
	w.Header().Set("Content-Type", "application/json")
	setSecurityHeaders(w)
	json.NewEncoder(w).Encode(f)
}

func audioHandler(w http.ResponseWriter, req *http.Request) {

	setSecurityHeaders(w)

	//decode the URL Path
	song := req.URL.Path[len("/getAudio/"):]

	ok := false

	// Import AnalyzedFeaturesList.json
	file, err := os.Open("configs/AnalyzedFeaturesList.json") // For read access.
	if err != nil {
		log.Fatal(err)
	}
	data := make([]byte, 10000)
	count, err := file.Read(data)
	if err != nil {
		log.Fatal(err)
	}

	//decode json
	var f []feature

	json.Unmarshal(data[0:count], &f)

	// Check if requested song is analyzed
	for i := range f {
		if song == f[i].Name {
			ok = true
			break
		}
	}

	if ok {
		//send the requested audio file if existing
		http.ServeFile(w, req, "audio_files/"+song)
	} else {
		//if not send 404 site
		http.ServeFile(w, req, "website/page/404.html")
	}

}

func analyzeYTHandler(w http.ResponseWriter, req *http.Request) {

	// decode incoming request (json)
	decoder := json.NewDecoder(req.Body)
	var reg request
	err := decoder.Decode(&reg)
	if err != nil {
		panic(err)
	}

	log.Printf("incoming request: %+v", reg)

	keyok := false

	for i := range keys {
		if keys[i].Key == reg.Key {
			keyok = true
			break
		}
	}

	if keyok {

		// create new download task
		var t task

		for {
			ftask, err := os.Open("configs/task.json") // For read access.
			if err != nil {
				log.Printf(err.Error())
				if strings.Contains(err.Error(), "no such file or directory") {
					break
				} else {
					panic(err)
				}
			}
			taskdata := make([]byte, 1000)
			taskcount, err := ftask.Read(taskdata)
			if err != nil {
				log.Fatal(err)
			}

			json.Unmarshal(taskdata[0:taskcount], &t)
			log.Printf("currently in the taskfile: %+v", t)

			if t.Stage == "download" {
				fmt.Println("A file is being dowloaded trying again in 1 second!")
				time.Sleep(time.Second)
			} else {
				break
			}
		}

		t.Stage = "download"
		t.Url = reg.Url
		t.Format = reg.Format

		mars, _ := json.Marshal(t)
		ioutil.WriteFile("configs/task.json", mars, 0644)

		// Run Download
		log.Printf("starting dowload script")
		dl := exec.Command("python3", "scripts/Download.py")
		dl.Stdout = os.Stdout
		dl.Stderr = os.Stderr
		_ = dl.Run()

		// Run Categorization
		log.Printf("starting cateogorization script")
		ana := exec.Command("python3", "scripts/CategorizeSong.py")
		ana.Stdout = os.Stdout
		ana.Stderr = os.Stderr
		_ = ana.Run()

		// Import NewFeatures.json
		file, err := os.Open("configs/NewFeatures.json") // For read access.
		if err != nil {
			log.Fatal(err)
		}
		data := make([]byte, 1000)
		count, err := file.Read(data)
		if err != nil {
			log.Fatal(err)
		}
		log.Printf(string(data[0:count]))

		//decode json
		var f []feature

		json.Unmarshal(data[0:count], &f)
		log.Printf("New Features: %+v", f)

		//set the header and write the response
		w.Header().Set("Content-Type", "application/json")
		setSecurityHeaders(w)

		json.NewEncoder(w).Encode(f)

	} else {
		//set the header and write the response
		w.Header().Set("Content-Type", "application/json")
		setSecurityHeaders(w)

		w.WriteHeader(http.StatusOK)

		io.WriteString(w, "{\"info\":\"Not a valid apiKey\"}")
	}

}
