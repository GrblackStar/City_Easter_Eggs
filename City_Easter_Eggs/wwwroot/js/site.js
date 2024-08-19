var userLocation = null;

// Try to get location from local storage
let storedCoords = localStorage.getItem('coords')
if (storedCoords) {
    storedCoords = JSON.parse(storedCoords);
    if (storedCoords.latitude)
        userLocation = storedCoords
}

// Default while loading
if (userLocation == null) userLocation = { latitude: 43, longitude: 23 }

var locationUpdateListeners = []
var firstPosGotten = false

function _PositionChangedUpdate(data)
{
    var newCoords = data.coords;
    if (!newCoords || !newCoords.latitude) return;

    localStorage.setItem('coords', JSON.stringify(data.coords));
    userLocation = newCoords

    if (!firstPosGotten) newCoords.first = true;
    firstPosGotten = true;

    for (let i = 0; i < locationUpdateListeners.length; i++) {
        locationUpdateListeners[i](newCoords);
    }
}

function _PositionChangeError(err) {
    console.warn(`ERROR(${err.code}): ${err.message}`);
    showToast('Пуснете местоположение!', 'error');
}

function StartWatchingUserLocation() {
    const options = {
        enableHighAccuracy: true,
        timeout: 5000,
        maximumAge: 0
    };

    navigator.geolocation.getCurrentPosition(_PositionChangedUpdate, _PositionChangeError, options);
    navigator.geolocation.watchPosition(_PositionChangedUpdate, _PositionChangeError, options)
}

function GetUserLocation()
{
    return userLocation;
}

function NotifyWhenLocationChanges(callback)
{
    locationUpdateListeners.push(callback);
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
    // get the current position of the device to ensure that point submission is valid
    var myPos = GetUserLocation();
    document.getElementById('userLocationLongitude').value = myPos.longitude;
    document.getElementById('userLocationLatitude').value = myPos.latitude;
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