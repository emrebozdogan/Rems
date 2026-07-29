import WKT from 'ol/format/WKT.js';
import GeoJSON from 'ol/format/GeoJSON.js';
import * as turf from '@turf/turf';

const wkt = new WKT();
const f1 = wkt.readFeature('POLYGON((0 0, 0 10, 10 10, 10 0, 0 0))');
const f2 = wkt.readFeature('POLYGON((5 5, 5 15, 15 15, 15 5, 5 5))');

const features = [f1, f2];
const geojsonFormat = new GeoJSON();
const turfPolygons = [];

features.forEach(feature => {
  const geojson = geojsonFormat.writeFeatureObject(feature);
  turfPolygons.push(geojson);
});

let intersectionsCount = 0;
for (let i = 0; i < turfPolygons.length; i++) {
  for (let j = i + 1; j < turfPolygons.length; j++) {
    const poly1 = turfPolygons[i];
    const poly2 = turfPolygons[j];
    const intersection = turf.intersect(turf.featureCollection([poly1, poly2]));
    
    if (intersection) {
      intersectionsCount++;
    }
  }
}
console.log('Intersections found:', intersectionsCount);
