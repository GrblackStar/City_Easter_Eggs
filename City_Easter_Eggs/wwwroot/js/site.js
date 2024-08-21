function showToast(message, type) {
    // Check if a toast already exists, so it doesn't open multiple at the same time
    if (document.querySelector('.toast')) {
        return;
    }

    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toastContainer');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toastContainer';
        toastContainer.style.position = 'fixed';
        toastContainer.style.top = '20px';
        toastContainer.style.left = '50%';
        toastContainer.style.transform = 'translateX(-50%)';
        toastContainer.style.zIndex = '9999';
        toastContainer.style.display = 'flex';
        toastContainer.style.justifyContent = 'center';
        toastContainer.style.alignItems = 'center';
        toastContainer.style.pointerEvents = 'none';
        document.body.appendChild(toastContainer);
    }

    // Create toast
    const toast = document.createElement('div');
    toast.className = 'toast'; // for easy selection
    toast.style.background = type === 'success' ? '#689F38' : '#CF1A2B';
    toast.style.color = 'white';
    toast.style.padding = '5px';
    toast.style.minWidth = '300px';
    toast.style.borderRadius = '7px';
    toast.style.boxShadow = '0 0.125rem 0.25rem grey';
    toast.style.fontSize = '14px';
    toast.style.display = 'flex';
    toast.style.justifyContent = 'space-between';
    toast.style.alignItems = 'center';
    toast.style.animation = 'fadein 0.5s, fadeout 0.5s 2.5s';

    // Icon
    const icon = document.createElement('img');
    icon.src = type === 'success' ? 'CheckIcon.svg' : 'CloseIcon.svg';
    icon.style.width = '14px';
    icon.style.height = '14px';
    icon.style.marginRight = '10px';
    icon.style.marginLeft = '7px';
    icon.style.filter = 'invert(1)'; // Makes the icon white
    toast.appendChild(icon);

    // Message
    const toastMessage = document.createElement('span');
    toastMessage.textContent = message;
    toastMessage.style.whiteSpace = 'nowrap';
    toast.appendChild(toastMessage);

    // Dismiss button
    const dismissButton = document.createElement('button');
    dismissButton.textContent = 'Затвори';
    dismissButton.style.background = 'transparent';
    dismissButton.style.border = 'none';
    dismissButton.style.fontSize = '12px';
    dismissButton.style.marginRight = '7px';
    dismissButton.style.color = 'white';
    dismissButton.style.textDecoration = 'underline';
    dismissButton.style.cursor = 'pointer';
    dismissButton.style.pointerEvents = 'auto';
    dismissButton.addEventListener('click', () => {
        toastContainer.removeChild(toast);
        if (!toastContainer.firstChild) {
            toastContainer.remove();
        }
    });
    toast.appendChild(dismissButton);

    // Add toast to container
    toastContainer.appendChild(toast);
    toast.style.width = `${toastMessage.offsetWidth + 120}px`;

    // Remove toast after 2.95 seconds -> a little before it fades out
    setTimeout(() => {
        if (toast.parentNode === toastContainer) {
            toastContainer.removeChild(toast);
            if (!toastContainer.firstChild) {
                toastContainer.remove();
            }
        }
    }, 2950);
}

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
var firstPosGotten = false;

function _PositionChangedUpdate(data)
{
    var newCoords = data.coords;
    if (!newCoords || !newCoords.latitude) return;

    localStorage.setItem('coords', JSON.stringify(data.coords));
    userLocation = newCoords

    newCoords.first = !firstPosGotten;
    firstPosGotten = true;

    for (let i = 0; i < locationUpdateListeners.length; i++) {
        locationUpdateListeners[i](newCoords);
    }
}
var showtoastfirst = true;
function _PositionChangeError(err) {
    console.warn(`ERROR(${err.code}): ${err.message}`);
    if (showtoastfirst) {
        showToast('Моля, включете локацията', 'error');
        showtoastfirst = false;
    }
    document.getElementById('loadingOverlay').style.display = 'flex';
    firstPosGotten = false;

    setTimeout(() => StartWatchingUserLocation(), 1500)
}

var lastWatch = null;

function StartWatchingUserLocation() {
    if (lastWatch != null)
        navigator.geolocation.clearWatch(lastWatch)

    const options = {
        enableHighAccuracy: true,
        timeout: 5000,
        maximumAge: 0
    };

    navigator.geolocation.getCurrentPosition(_PositionChangedUpdate, () => { }, options);
    lastWatch = navigator.geolocation.watchPosition(_PositionChangedUpdate, _PositionChangeError, options)
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