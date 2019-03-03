package main

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"os/exec"
	"strings"
	"time"
)

type audioRequest struct {
	Name string `json:"name"`
}

type request struct {
	Url    string `json:"url"`
	Format string `json:"format"`
}

type response struct {
	Url          string `json:"url"`
	Name         string `json:"name"`
	Featuregroup string `json:"featuregroup"`
}

type task struct {
	Stage  string `json:"stage"`
	Url    string `json:"url"`
	Format string `json:"format"`
}

type feature struct {
	Name         string `json:"name"`
	Featuregroup string `json:"featuregroup"`
}

func main() {
	mux := http.NewServeMux()

	mux.HandleFunc("/", rootHandler)
	mux.HandleFunc("/getAudio", audioHandler)

	srv := &http.Server{
		Addr:    "0.0.0.0:8080",
		Handler: mux,
	}

	log.Fatal(srv.ListenAndServe())

}

func audioHandler(w http.ResponseWriter, req *http.Request) {
	// decode incoming request (json)
	decoder := json.NewDecoder(req.Body)
	var reg audioRequest
	err := decoder.Decode(&reg)
	if err != nil {
		panic(err)
	}

	log.Printf("incoming request: %+v", reg)

	//send the requested audio file
	http.ServeFile(w, req, "audio_files/"+reg.Name)
}

func rootHandler(w http.ResponseWriter, req *http.Request) {

	// decode incoming request (json)
	decoder := json.NewDecoder(req.Body)
	var reg request
	err := decoder.Decode(&reg)
	if err != nil {
		panic(err)
	}

	log.Printf("incoming request: %+v", reg)

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
	exec.Command("python3", "Download.py").Run()

	// Run Categorization
	log.Printf("starting cateogorization script")
	exec.Command("python3", "CategorizeSong.py").Run()

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
	log.Printf("New Feature: %+v", f[0])

	//prepare response
	var resp response

	resp.Url = reg.Url
	resp.Name = f[0].Name
	resp.Featuregroup = f[0].Featuregroup

	log.Printf("response: %+v", resp)

	//set the header and write the response
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(resp)

}
