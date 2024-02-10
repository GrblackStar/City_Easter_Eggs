async function GetUserLocation() {
    function error(err) {
        console.warn(`ERROR(${err.code}): ${err.message}`);
    }

    const options = {
        enableHighAccuracy: true,
        timeout: 5000,
        maximumAge: 0,
    };

    return new Promise((onDoneFunc) => {
        navigator.geolocation.getCurrentPosition(onDoneFunc, error, options);
    })
}

async function HttpGet(theUrl) {
    return new Promise((onDoneFunc) => {
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
                onDoneFunc(xmlHttp.responseText);
        }
        xmlHttp.open("GET", theUrl, true); // true for asynchronous
        xmlHttp.send(null);
    });
}