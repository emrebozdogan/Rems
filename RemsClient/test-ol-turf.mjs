import WKT from 'ol/format/WKT.js';
import GeoJSON from 'ol/format/GeoJSON.js';
import * as turf from '@turf/turf';

const wkt = new WKT();
// Polygon 1
const f1 = wkt.readFeature('POLYGON((0 0, 0 10, 10 10, 10 0, 0 0))', { dataProjection: 'EPSG:4326', featureProjection: 'EPSG:3857' });
// Polygon 2 (overlapping)
const f2 = wkt.readFeature('POLYGON((5 5, 5 15, 15 15, 15 5, 5 5))', { dataProjection: 'EPSG:4326', featureProjection: 'EPSG:3857' });

const geojsonFormat = new GeoJSON();
const gj1 = geojsonFormat.writeFeatureObject(f1, { dataProjection: 'EPSG:4326', featureProjection: 'EPSG:3857' });
const gj2 = geojsonFormat.writeFeatureObject(f2, { dataProjection: 'EPSG:4326', featureProjection: 'EPSG:3857' });

try {
  const intersection = turf.intersect(turf.featureCollection([gj1, gj2]));
  console.log('Intersection:', intersection ? 'Yes' : 'No');
} catch (e) {
  console.error(e);
}
