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
        const feature = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([location.lon, location.lat]))
        });

        const style = getMarkerStyle(`${location.city}, ${location.country}`);
        feature.setStyle(style);

        features.push(feature);
    }

    if (iconLayer) {
        map.removeLayer(iconLayer);
    }

    iconLayer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: features
        })
    });

    map.addLayer(iconLayer);
}

function getMarkerStyle(label) {
    return new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 42],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            src: 'images/marker.svg'
        }),
        text: new ol.style.Text({
            font: "12px sans-serif",
            fill: new ol.style.Fill({ color: '#343434' }),
            stroke: new ol.style.Stroke({
                color: '#fff',
                width: 2
            }),
            backgroundFill: new ol.style.Fill({ color: "#fffffa" }),
            backgroundStroke: new ol.style.Stroke({
                color: '#2181ff',
                width: 1
            }),
            padding: [3, 7, 1, 7],
            offsetY: 18,
            text: label
        })
    })
}
