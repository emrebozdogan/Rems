import WKT from 'ol/format/WKT.js';
import GeoJSON from 'ol/format/GeoJSON.js';
import * as turf from '@turf/turf';

const wkt = new WKT();
const f1 = wkt.readFeature('POLYGON((0 0, 0 10, 10 10, 10 0, 0 0))');
const f2 = wkt.readFeature('POLYGON((5 5, 5 15, 15 15, 15 5, 5 5))');

const geojsonFormat = new GeoJSON();
const gj1 = geojsonFormat.writeFeatureObject(f1);
const gj2 = geojsonFormat.writeFeatureObject(f2);

const intersection = turf.intersect(turf.featureCollection([gj1, gj2]));
try {
  const olFeature = geojsonFormat.readFeature(intersection);
  console.log('Read Feature Success:', !!olFeature);
} catch(e) {
  console.error(e);
}
