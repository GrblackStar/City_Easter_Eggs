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

async function CreatePointSubmit() {
    // get the current position of the device
    var myPos = await GetUserLocation();

    document.getElementById('userLocationLongitude').value = myPos.coords.longitude;
    document.getElementById('userLocationLatitude').value = myPos.coords.latitude;
}

$(async function () {
    // Run on submit point page only.
    if ($('form#createPointForm').length == 0) return;

    var locationGotten = false;

    $("#loadingText").show();
    $("#createPointSubmit").prop('disabled', true);

    $('form#createPointForm').submit(function (e) {
        if (!locationGotten) e.preventDefault();
    })

    await CreatePointSubmit();
    locationGotten = true;

    $("#loadingText").hide();
    $("#createPointSubmit").prop('disabled', false);
});

function convertFileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = reject;
    });
}