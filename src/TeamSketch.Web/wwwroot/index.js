const centerOfWorld = ol.proj.fromLonLat([40.86666, 34.56666]);
const map = new ol.Map({
    target: 'map',
    layers: [
        new ol.layer.Tile({
            source: new ol.source.OSM()
        })
    ],
    view: new ol.View({
        center: centerOfWorld,
        zoom: 3
    })
});

let iconLayer;

loadLocationMarkers();
setInterval(() => {
    loadLocationMarkers();
}, 5000);

function loadLocationMarkers() {
    fetch(`${baseUrl}/api/liveview`, {
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(addLocationMarkers);
}

function addLocationMarkers(locations) {
    const features = [];
    for (let location of locations) {
        features.push(new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([location.lon, location.lat])),
            name: `${location.city}, ${location.country}`
        }));
    }

    if (iconLayer) {
        map.removeLayer(iconLayer);
    }

    iconLayer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: features
        }),
        style: new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 42],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                src: 'images/marker.svg'
            })
        })
    });

    map.addLayer(iconLayer);
}
