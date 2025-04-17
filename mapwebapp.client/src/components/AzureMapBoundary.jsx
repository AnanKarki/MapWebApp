import React, { useEffect, useRef, useState } from 'react';
import Select from 'react-select';
import { getPolygonByReference } from '../utlis/azureMapUtils';
import { AZURE_MAPS_CONFIG, AZURE_MAPS_URLS } from '../Constant';

const AzureMapBoundary = () => {
    const mapRef = useRef(null);
    const inputRef = useRef(null);
    const resultListRef = useRef(null);
    const loadingIconRef = useRef(null);


    const [map, setMap] = useState(null);
    const [datasource, setDatasource] = useState(null);
    const [popup, setPopup] = useState(null);
    const [areaVisible, setAreaVisible] = useState(true);
    const [options, setOptions] = useState([]);
    const [selectedRef, setSelectedRef] = useState(null);
    const [labelRef, setSelectedlabelRef] = useState(null);
    const boundaryCache = useRef({});
    const [text, setText] = useState("Hide all areas");

    const togglePanel = () => setAreaVisible(prev => !prev);

    const azureMapsClientId = AZURE_MAPS_CONFIG.CLIENT_ID;
    const subscriptionKey = AZURE_MAPS_CONFIG.SUBSCRIPTION_KEY;

    const localdistrictboundariesUrl = AZURE_MAPS_URLS.LOCAL_DISTRICT_BOUNDARIES;
    const DistrictlistUrl = AZURE_MAPS_URLS.DISTRICT_LIST;
    const polygonReferenceUrl = AZURE_MAPS_URLS.POLYGON_BY_REFERENCE;
   
    useEffect(() => {
        const script = document.createElement('script');
        script.src = AZURE_MAPS_URLS.MAP_SOURCE;
        script.onload = () => {
            const m = new window.atlas.Map(mapRef.current.id, {
                style: 'grayscale_light',
                view: 'Auto',
                authOptions: {
                    authType: 'subscriptionKey',
                    subscriptionKey
                }
            });

            m.events.add('ready', async () => {
                const ds = new window.atlas.source.DataSource();
                m.sources.add(ds);
                setDatasource(ds);

                const lyr = new window.atlas.layer.PolygonLayer(ds, null, { fillColor: 'hotpink' });
                m.layers.add(lyr, 'labels');

                const pop = new window.atlas.Popup();
                setPopup(pop);

                m.events.add('click', lyr, e => handleLayerClick(e, pop, m));

                m.controls.add(new atlas.control.FullscreenControl(), { position: 'top-left' });
                m.controls.add(new atlas.control.ZoomControl(), { position: 'top-left' });
                m.controls.add(new atlas.control.PitchControl(), { position: 'top-left' });
                m.controls.add(new atlas.control.CompassControl(), { position: 'top-left' });

                m.setCamera({ pitch: 60, bearing: 0 });

                try {
                    loadingIconRef.current.style.display = 'block';
                    const response = await fetch(localdistrictboundariesUrl);
                    if (response.ok) {
                        const geojson = await response.json();
                        ds.setShapes(geojson);

                        const bbox = window.atlas.data.BoundingBox.fromData(geojson);
                        m.setCamera({ bounds: bbox, padding: 40 });

                    }
                } catch (err) {
                    console.error("Failed to load UK boundaries:", err);
                }
                loadingIconRef.current.style.display = 'none';
                //await showallareas(ds);

            });

            setMap(m);
        };
        document.body.appendChild(script);
        Dropdown();
    }, []);

    const Dropdown = () => {

        fetch(DistrictlistUrl)
            .then(res => res.json())
            .then(data => {
                if (!Array.isArray(data)) throw new Error('Invalid district data');
                const formatted = data.map(d => ({
                    value: d.reference,
                    label: d.name,
                }));
                setOptions(formatted);
            })
            .catch(err => {
                console.error("Failed to load districts", err);
                setOptions([]); // fallback to prevent .map crash
            });

    };

    const handleChange = async (e) => {
        const selectedReference = e.value;
        setSelectedRef(selectedReference);
        setSelectedlabelRef(e.label);

        await getPolygonByReference({
            reference: selectedReference,
            polygonReferenceUrl: polygonReferenceUrl,
            datasource: datasource,
            map : map,
            loadingElement : loadingIconRef,
            atlas : window.atlas
        });
    };
    const showallareas = async () => {
        try {
            loadingIconRef.current.style.display = 'block';
            const response = await fetch(localdistrictboundariesUrl);
            if (response.ok) {
                const geojson = await response.json();
                datasource.setShapes(geojson);

                const bbox = window.atlas.data.BoundingBox.fromData(geojson);
                map.setCamera({ bounds: bbox, padding: 40 });
                setAreaVisible(true);
                setText(prevText => (prevText === "Hide all areas" ? "Show all areas" : "Hide all areas"));
                loadingIconRef.current.style.display = 'none';
            }
        } catch (err) {
            console.error("Failed to load UK boundaries:", err);
        }
    }
    const hideallareas = () => {
        datasource.clear();
        setAreaVisible(false);
        setText(prevText => (prevText === "Hide all areas" ? "Show all areas" : "Hide all areas"));
    }
    const clear = () => {
        datasource.clear();
        setSelectedRef(null);
        setSelectedlabelRef(null);
    }

    const handleLayerClick = (e, popup, map) => {
        const shape = e.shapes[0].toJson();
        let numPos = 0;
        let numPolygons = 0;

        if (shape.geometry.type === 'Polygon') {
            numPolygons = 1;
            shape.geometry.coordinates.forEach(p => (numPos += p.length));
        } else {
            numPolygons = shape.geometry.coordinates.length;
            shape.geometry.coordinates.forEach(p => {
                p.forEach(r => (numPos += r.length));
            });
        }

        popup.setOptions({
            content: `<div style='padding:15px;'>Type: ${shape.geometry.type}<br/># polygons: ${numPolygons}<br/># positions: ${numPos}</div>`,
            position: e.position
        });
        popup.open(map);
    };

    return (
        <div>
            <div
                id="myMap"
                data-testid="myMap-testid"
                ref={mapRef}
                style={{
                    width: '100vw',
                    height: '100vh',
                    position: 'fixed',
                    top: 0,
                    left: 0,
                }}
            />

            <div
                style={{
                    position: 'absolute',
                    top: 15,
                    left: 60,
                    backgroundColor: 'white',
                    padding: 10,
                    borderRadius: 10,
                    zIndex: 999,
                }}
            >
                <div style={{ width: '300px', marginBottom: '1rem' }}>
                    <Select
                        options={options}
                        value={selectedRef}
                        onChange={handleChange}
                        placeholder={selectedRef == null ? 'Search district...' : labelRef}
                        isClearable
                        isSearchable
                    />
                </div>

                <input type="button" onClick={clear} value="Clear" />
                
                <input type="button" data-testid="eventButton" onClick={areaVisible ? hideallareas : showallareas} value={text} />
               


                {areaVisible && (
                    <div id="resultList" ref={resultListRef}></div>
                )}
            </div>

            <img
                ref={loadingIconRef}
                src="/src/assets/loadingIcon.gif"
                alt="Loading"
                style={{
                    position: 'absolute',
                    left: 'calc(50% - 25px)',
                    top: 250,
                    display: 'block',
                }}
            />
        </div>
    );
};

export default AzureMapBoundary;

