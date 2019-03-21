package main

import (
	"crypto/tls"
	"encoding/json"
	"io"
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
	parseJSONFile("configs/server.conf", &conf)

	parseJSONFile("configs/"+conf.PageConfig, &allowedPages)
	parseJSONFile("configs/"+conf.StylesConfig, &allowedStyles)
	parseJSONFile("configs/"+conf.ScriptsConfig, &allowedScripts)

	parseJSONFile("configs/"+conf.SecurityConfig, &securityHeaders)

	mux := http.NewServeMux()

	//website
	mux.HandleFunc("/", rootHandler)
	mux.HandleFunc("/page/", pageHandler)
	mux.HandleFunc("/style/", styleHandler)
	mux.HandleFunc("/script/", scriptHandler)

	//analysis
	mux.HandleFunc("/analyzeYT", analyzeYTHandler)
	mux.HandleFunc("/getAudio/", audioHandler)
	mux.HandleFunc("/getSongList", songListHandler)

	//maintainace
	mux.HandleFunc("/reloadScripts", scriptReloadHandler)

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

func scriptReloadHandler(w http.ResponseWriter, req *http.Request) {

	var conf serverConfig
	parseJSONFile("configs/server.conf", &conf)

	parseJSONFile("configs/"+conf.PageConfig, &allowedPages)
	parseJSONFile("configs/"+conf.StylesConfig, &allowedStyles)
	parseJSONFile("configs/"+conf.ScriptsConfig, &allowedScripts)

	parseJSONFile("configs/"+conf.SecurityConfig, &securityHeaders)

	w.Header().Set("Content-Type", "text/plain")
	w.WriteHeader(http.StatusOK)

	io.WriteString(w, "Sucessfully reloaded configurations!\nTo reload the certificates and the port restart the server.")
}
func getCiphers() []uint16 {
	return []uint16{
		tls.TLS_AES_128_GCM_SHA256,
		tls.TLS_AES_256_GCM_SHA384,
		tls.TLS_CHACHA20_POLY1305_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
		tls.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
		tls.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305,
		tls.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
		tls.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
		tls.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
		tls.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
	}
}

func setSecurityHeaders(w http.ResponseWriter) {
	for i := range securityHeaders {
		w.Header().Set(securityHeaders[i].Header, securityHeaders[i].Option)
	}
}

func parseJSONFile(file string, i interface{}) {
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
