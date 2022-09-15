const express = require("express")
const app = express();
const fs = require("fs")
const readline = require('readline');
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

let lib_location = "nan";

app.get("/mncp", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "\\index.html");
})

app.get("/min/vs/loader.js", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/min/vs/loader.js", { 
        headers:{
            "Content-Type":"text/javascript"
        }
    });
})

app.get("/min/vs/editor/editor.main.css", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/min/vs/editor/editor.main.css", {
        headers: {
            "Content-Type":"text/css"
        }
    });
})

app.get("/min/vs/editor/editor.main.nls.js", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/min/vs/editor/editor.main.nls.js", {
        headers: {
            "Content-Type":"text/javascript"
        }
    });
})

app.get("/min/vs/editor/editor.main.js", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/min/vs/editor/editor.main.js", {
        headers: {
            "Content-Type":"text/javascript"
        }
    });
})

app.get("/min/vs/base/worker/workerMain.js", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/min/vs/base/worker/workerMain.js", {
        headers: {
            "Content-Type":"text/javascript"
        }
    });
})

app.get("/min/vs/base/browser/ui/codicons/codicon/codicon.ttf", (req, res) => {
    if (lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    res.sendFile(__dirname + "/codicon.ttf", {
        headers: {
            "Content-Type": "font/ttf"
        }
    });
})

app.post("/abc", (req, res) => {
    if(lib_location == "nan") {
        res.send("server has not been prepared yet.")
        return;
    }
    console.log(req.body)
    res.send({
        intellisense: req.body.incl
    })
})

app.use((req, res, next) => {
    res.set({
        "Access-Control-Allow-Origin": "*",
        "Access-Control-Allow-Headers": "Origin, X-Requested-With, Content-Type, Accept",
        "Access-Control-Allow-Methods": "GET, POST, PATCH, DELETE, OPTIONS",
        "Content-Security-Policy": "default-src *",
        "X-Content-Security-Policy": "default-src *",
        "X-WebKit-CSP": "default-src *"
    })
    next();
});

app.listen(13005, () => 
{
    rl.question('lib folder location = ', function (location) {
        lib_location = location;
    });
})