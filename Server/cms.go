package main

import "net/http"

// all functions necessary for the "normal" webserver are here

func rootHandler(w http.ResponseWriter, req *http.Request) {

	setSecurityHeaders(w)

	//redirect to index page
	http.Redirect(w, req, "/page/index", http.StatusSeeOther)
}

func pageHandler(w http.ResponseWriter, req *http.Request) {

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

	setSecurityHeaders(w)

	//decode URL Path
	style := req.URL.Path[len("/style/"):]

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
		//if it is serve it
		http.ServeFile(w, req, "website/stlye/"+style+".css")
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/style/404", http.StatusSeeOther)
	}
}

func scriptHandler(w http.ResponseWriter, req *http.Request) {

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
		http.ServeFile(w, req, "website/script/"+script+".js")
	} else {
		//if not redirect to 404
		http.Redirect(w, req, "/script/404", http.StatusSeeOther)
	}
}
