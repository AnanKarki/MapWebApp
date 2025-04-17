export const getPolygonByReference = async ({
    reference,
    polygonReferenceUrl,
    datasource,
    map,
    loadingElement,
    atlas
}) => {
    if (loadingElement) loadingElement.current.style.display = 'block';

    const res = await fetch(polygonReferenceUrl.replace('{reference}', reference));
    const data = await res.json();

    datasource.setShapes(data);

    const bbox = atlas.data.BoundingBox.fromData(data);
    map.setCamera({ bounds: bbox, padding: 40 });

    if (loadingElement) loadingElement.current.style.display = 'none';
};