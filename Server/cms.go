package main

import (
	"io"
	"log"
	"net/http"
	"os"
)

// all functions necessary for the "normal" webserver are here

func rootHandler(w http.ResponseWriter, req *http.Request) {

	setSecurityHeaders(w)

	//redirect to index page
	http.Redirect(w, req, "/page/index", http.StatusSeeOther)
}

func pageHandler(w http.ResponseWriter, req *http.Request) {

	log.Printf("new request for:" + req.URL.Path)

	setSecurityHeaders(w)

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
		http.ServeFile(w, req, "website/page/"+page+".html")
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/page/404", http.StatusSeeOther)
	}
}

func styleHandler(w http.ResponseWriter, req *http.Request) {

	log.Printf("new request for:" + req.URL.Path)

	setSecurityHeaders(w)

	//decode URL Path
	style := req.URL.Path[len("/style/"):]

	log.Printf("requested style:" + style)

	ok := false

	//Check if requested style is allowed
	for i := range allowedStyles {
		if style == allowedStyles[i].Name && allowedStyles[i].Allowed == "y" {
			ok = true
			break
		} else if style == allowedStyles[i].Name && allowedStyles[i].Allowed == "n" {
			ok = false
			break
		}
	}

	if ok {
		w.Header().Set("Content-Type", "text/css; charset=utf-8")
		//if it is serve it
		//log.Printf("now serving:website/styles/" + style + ".css")

		w.WriteHeader(http.StatusOK)

		res, _ := os.Open("website/styles/" + style + ".css")
		dat := make([]byte, 10000)
		count, _ := res.Read(dat)
		//log.Printf("Containing:" + string(dat[0:count]))

		io.WriteString(w, string(dat[0:count]))
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/style/404", http.StatusSeeOther)
	}
}

func scriptHandler(w http.ResponseWriter, req *http.Request) {

	log.Printf("new request for:" + req.URL.Path)

	setSecurityHeaders(w)

	//decode URL Path
	script := req.URL.Path[len("/script/"):]

	ok := false

	//Check if requested script is allowed
	for i := range allowedScripts {
		if script == allowedScripts[i].Name && allowedScripts[i].Allowed == "y" {
			ok = true
			break
		} else if script == allowedScripts[i].Name && allowedScripts[i].Allowed == "n" {
			ok = false
			break
		}
	}

	if ok {
		//if it is serve it
		http.ServeFile(w, req, "website/scripts/"+script+".js")
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/script/404", http.StatusSeeOther)
	}
}
