const turf = require('@turf/turf');

const poly1 = turf.polygon([[[0, 0], [0, 10], [10, 10], [10, 0], [0, 0]]]);
const poly2 = turf.polygon([[[5, 5], [5, 15], [15, 15], [15, 5], [5, 5]]]);

try {
  const result1 = turf.intersect(turf.featureCollection([poly1, poly2]));
  console.log('With FeatureCollection:', result1 ? 'Success' : 'Null');
} catch (e) {
  console.log('Error with FeatureCollection:', e.message);
}

try {
  const result2 = turf.intersect(poly1, poly2);
  console.log('With 2 args:', result2 ? 'Success' : 'Null');
} catch (e) {
  console.log('Error with 2 args:', e.message);
}
