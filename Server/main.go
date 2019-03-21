package main

import (
	"crypto/tls"
	"encoding/json"
	"log"
	"net/http"
	"os"
)

// this file contains the main server seleton and configuration loaders

var allowedPages []contentSetting
var allowedStyles []contentSetting
var allowedScripts []contentSetting

var securityHeaders []securitySetting

func main() {

	var conf serverConfig
	paseJSONFile("configs/server.conf", &conf)

	allowedPages = loadConfigs(conf.PageConfig)
	allowedStyles = loadConfigs(conf.StylesConfig)
	allowedScripts = loadConfigs(conf.ScriptsConfig)

	securityHeaders = loadSecurityConfigs(conf.SecurityConfig)

	mux := http.NewServeMux()

	mux.HandleFunc("/", rootHandler)
	mux.HandleFunc("/page/", pageHandler)
	mux.HandleFunc("/style/", styleHandler)
	mux.HandleFunc("/script/", scriptHandler)
	mux.HandleFunc("/analyzeYT", analyzeYTHandler)
	mux.HandleFunc("/getAudio/", audioHandler)
	mux.HandleFunc("/getSongList", songListHandler)

	if conf.Encryption == "y" {

		cfg := &tls.Config{
			MinVersion:               tls.VersionTLS12,
			MaxVersion:               tls.VersionTLS13,
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

func loadConfigs(file string) []contentSetting {
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
	var f []contentSetting

	json.Unmarshal(data[0:count], &f)

	log.Printf("%+v", f)

	return f
}

func loadSecurityConfigs(file string) []securitySetting {
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
	var f []securitySetting

	json.Unmarshal(data[0:count], &f)

	log.Printf("%+v", f)

	return f
}

func getCiphers() []uint16 {
	return []uint16{
		tls.TLS_AES_256_GCM_SHA384,
		tls.TLS_AES_128_GCM_SHA256,

		tls.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
		tls.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,

		tls.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
		tls.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
		tls.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
		tls.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
	}
}

func setSecurityHeaders(w http.ResponseWriter) {
	for i := range securityHeaders {
		w.Header().Set(securityHeaders[i].Header, securityHeaders[i].Option)
	}
}

func paseJSONFile(file string, i interface{}) {
	// Import Configuration
	files, err := os.Open(file) // For read access.
	if err != nil {
		log.Fatal(err)
	}
	data := make([]byte, 10000)
	count, err := files.Read(data)
	if err != nil {
		log.Fatal(err)
	}

	json.Unmarshal(data[0:count], i)
}
