package main

// All structs needed for json parsing are here

type serverConfig struct {
	Port           string `json:"port"`
	Encryption     string `json:"encryption"`
	Cert           string `json:"cert"`
	Key            string `json:"key"`
	PageConfig     string `json:"pageConfig"`
	StylesConfig   string `json:"stylesConfig"`
	ScriptsConfig  string `json:"scriptsConfig"`
	SecurityConfig string `json:"securityConfig"`
}

type contentSetting struct {
	Name    string `json:"name"`
	Allowed string `json:"allowed"`
}

type securitySetting struct {
	Header string `json:"header"`
	Option string `json:"option"`
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
