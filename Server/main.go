package main

import (
	"crypto/tls"
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

type serverConfig struct {
	Port       string `json:"port"`
	Encryption string `json:"encryption"`
	Cert       string `json:"cert"`
	Key        string `json:"key"`
	PageConfig string `json:"pageConfig"`
}

type pageSetting struct {
	Name    string `json:"name"`
	Allowed string `json:"allowed"`
}
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
	Featuregroup int    `json:"featuregroup"`
}

type task struct {
	Stage  string `json:"stage"`
	Url    string `json:"url"`
	Format string `json:"format"`
}

type feature struct {
	Name         string `json:"name"`
	Featuregroup int    `json:"featuregroup"`
}

var allowedPages []pageSetting

func main() {

	conf := loadServerConfigs()

	allowedPages = loadPageConfigs(conf.PageConfig)

	mux := http.NewServeMux()

	mux.HandleFunc("/", rootHandler)
	mux.HandleFunc("/page/", pageHandler)
	mux.HandleFunc("/analyzeYT", analyzeYTHandler)
	mux.HandleFunc("/getAudio/", audioHandler)
	mux.HandleFunc("/getSongList", songListHandler)

	if conf.Encryption == "y" {

		cfg := &tls.Config{
			MinVersion:               tls.VersionTLS12,
			CurvePreferences:         []tls.CurveID{tls.X25519, tls.CurveP521, tls.CurveP384, tls.CurveP256},
			PreferServerCipherSuites: true,
			CipherSuites:             getCiphers(),
		}

		srv := &http.Server{
			Addr:         "0.0.0.0:" + conf.Port,
			Handler:      mux,
			TLSConfig:    cfg,
			TLSNextProto: make(map[string]func(*http.Server, *tls.Conn, http.Handler), 0),
		}

		log.Fatal(srv.ListenAndServeTLS("configs/"+conf.Cert, "configs/"+conf.Key))

	} else {
		srv := &http.Server{
			Addr:    "0.0.0.0:" + conf.Port,
			Handler: mux,
		}

		log.Fatal(srv.ListenAndServe())
	}

}

func loadServerConfigs() serverConfig {
	// Import Configuration
	files, err := os.Open("configs/server.conf") // For read access.
	if err != nil {
		log.Fatal(err)
	}
	data := make([]byte, 10000)
	count, err := files.Read(data)
	if err != nil {
		log.Fatal(err)
	}

	//decode json
	var f serverConfig

	json.Unmarshal(data[0:count], &f)

	log.Printf("%+v", f)

	return f
}

func loadPageConfigs(file string) []pageSetting {
	// Import Configuration
	files, err := os.Open("configs/" + file) // For read access.
	if err != nil {
		log.Fatal(err)
	}
	data := make([]byte, 10000)
	count, err := files.Read(data)
	if err != nil {
		log.Fatal(err)
	}

	//decode json
	var f []pageSetting

	json.Unmarshal(data[0:count], &f)

	log.Printf("%+v", f)

	return f
}

func rootHandler(w http.ResponseWriter, req *http.Request) {
	//redirect to index page
	http.Redirect(w, req, "/page/index", http.StatusSeeOther)
}

func pageHandler(w http.ResponseWriter, req *http.Request) {
	//decode URL Path
	page := req.URL.Path[len("/page/"):]

	ok := false

	//Check if requested page is allowed
	for i := range allowedPages {
		if page == allowedPages[i].Name && allowedPages[i].Allowed == "y" {
			ok = true
			break
		} else if page == allowedPages[i].Name && allowedPages[i].Allowed == "n" {
			ok = false
			break
		}
	}

	if ok {
		//if it is serve it
		http.ServeFile(w, req, "website/"+page+".html")
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/page/404", http.StatusSeeOther)
	}
}

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
	json.NewEncoder(w).Encode(f)
}

func audioHandler(w http.ResponseWriter, req *http.Request) {
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
		http.ServeFile(w, req, "website/404.html")
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

func getCiphers() []uint16 {
	return []uint16{
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
		tls.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,

		tls.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
		tls.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,

		//tls.TLS_AES_256_GCM_SHA384,
		//tls.TLS_AES_128_GCM_SHA256,
	}
}
