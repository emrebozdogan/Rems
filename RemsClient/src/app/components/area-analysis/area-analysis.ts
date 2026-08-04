import { Component, inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AreaAnalysisService } from './area-analysis.service';
import { PropertiesService } from '../properties/properties.service';
import { AreaAnalysisRequest } from '../../models/area-analysis.model';

import Map from 'ol/Map';
import View from 'ol/View';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import VectorLayer from 'ol/layer/Vector';
import VectorSource from 'ol/source/Vector';
import Draw from 'ol/interaction/Draw';
import WKT from 'ol/format/WKT';
import { Stroke, Style, Fill, Text } from 'ol/style';
import Feature from 'ol/Feature';

@Component({
  selector: 'app-area-analysis',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './area-analysis.html',
  styleUrl: './area-analysis.scss',
})
export class AreaAnalysis implements OnInit, OnDestroy {
  private analysisService = inject(AreaAnalysisService);
  private propertiesService = inject(PropertiesService);
  private cdr = inject(ChangeDetectorRef);

  map!: Map;
  vectorSource!: VectorSource;
  vectorLayer!: VectorLayer;
  drawInteraction: Draw | null = null;
  wktFormat = new WKT();

  mode: 'Manual Draw' | 'Auto-Select' | null = null;
  surfaceArea: number | null = null;
  successMessage: string | null = null;
  errorMessage: string | null = null;

  polygons: string[] = [];

  ngOnInit(): void {
    this.initMap();
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.setTarget(undefined);
    }
  }

  initMap(): void {
    this.vectorSource = new VectorSource();
    this.vectorLayer = new VectorLayer({
      source: this.vectorSource,
      style: (feature) => {
        const label = feature.get('label') || '';
        return new Style({
          fill: new Fill({ color: 'rgba(0, 153, 255, 0.4)' }),
          stroke: new Stroke({ color: '#0099ff', width: 2 }),
          text: new Text({
            text: label,
            font: '16px Calibri,sans-serif',
            fill: new Fill({ color: '#000' }),
            stroke: new Stroke({ color: '#fff', width: 3 }),
          }),
        });
      },
    });

    this.map = new Map({
      target: 'map',
      layers: [new TileLayer({ source: new OSM() }), this.vectorLayer],
      view: new View({
        center: [3900000, 4700000],
        zoom: 6,
      }),
    });
  }

  selectManualDraw(): void {
    this.clearMap();
    this.mode = 'Manual Draw';
    this.enableDrawing();
  }

  selectAutoSelect(): void {
    this.clearMap();

    this.analysisService.getSavedGeometries().subscribe({
      next: (res) => {
        this.mode = 'Auto-Select';
        this.polygons = [res.polygonA, res.polygonB, res.polygonC];
        const labels = ['A', 'B', 'C'];

        this.polygons.forEach((wkt, idx) => {
          const feature = this.wktFormat.readFeature(wkt);
          feature.set('label', labels[idx]);
          this.vectorSource.addFeature(feature);
        });

        this.showSuccess('Geometries A, B, and C loaded.');
        const extent = this.vectorSource.getExtent();
        if (extent) {
          this.map.getView().fit(extent, { padding: [50, 50, 50, 50], maxZoom: 15 });
        }
      },
      error: (err) => {
        this.mode = null;
        if (err.status === 404) {
          this.showError('No saved geometries found. Please use Manual Draw.');
        } else {
          this.showError('Failed to load saved geometries.');
        }
      },
    });
  }

  enableDrawing(): void {
    if (this.drawInteraction) {
      this.map.removeInteraction(this.drawInteraction);
    }

    this.drawInteraction = new Draw({
      source: this.vectorSource,
      type: 'Polygon',
      minPoints: 4,
      maxPoints: 4,
    });

    this.drawInteraction.on('drawend', (event) => {
      const feature = event.feature;
      const wkt = this.wktFormat.writeFeature(feature);

      const labels = ['A', 'B', 'C'];
      const label = labels[this.polygons.length];
      feature.set('label', label);

      this.polygons.push(wkt);

      if (this.polygons.length >= 3) {
        this.map.removeInteraction(this.drawInteraction!);
        this.drawInteraction = null;
        
        const req = {
          polygonA: this.polygons[0],
          polygonB: this.polygons[1],
          polygonC: this.polygons[2],
        };

        this.analysisService.saveGeometries(req).subscribe({
          next: () => this.showSuccess('Geometries A, B, and C completed and saved.'),
          error: () => this.showError('Geometries completed but failed to save.'),
        });
      }
      this.cdr.detectChanges();
    });

    this.map.addInteraction(this.drawInteraction);
  }

  computeIntersectionAB(): void {
    this.compute('IntersectAB');
  }
  computeIntersectionBA(): void {
    this.compute('DifferenceBA');
  }
  computeUnionAB(): void {
    this.compute('UnionAB');
  }
  computeUnionABC(): void {
    this.compute('UnionABC');
  }

  compute(operation: string): void {
    if (this.polygons.length < 3) {
      this.showError('Please complete geometries A, B, and C.');
      return;
    }

    const req: AreaAnalysisRequest = {
      polygonA: this.polygons[0],
      polygonB: this.polygons[1],
      polygonC: this.polygons[2],
      operationType: operation,
    };

    this.analysisService.performAnalysis(req).subscribe({
      next: (res) => {
        this.surfaceArea = res.surfaceArea;

        if (res.resultGeometry) {
          const feature = this.wktFormat.readFeature(res.resultGeometry, {
            dataProjection: 'EPSG:3857',
            featureProjection: 'EPSG:3857',
          });

          this.vectorSource.clear();
          
          let resultLabel = 'Result';
          if (operation === 'UnionAB') resultLabel = 'D';
          else if (operation === 'UnionABC') resultLabel = 'E';
          
          feature.set('label', resultLabel);
          this.vectorSource.addFeature(feature);

          const extent = this.vectorSource.getExtent();
          if (extent) {
            this.map.getView().fit(extent, { padding: [50, 50, 50, 50], maxZoom: 15 });
          }

          this.showSuccess(res.message || 'Analysis completed and saved successfully.');
        } else {
          this.showError(res.message || 'No geometry resulted from this operation.');
        }
      },
      error: (err) => {
        this.showError(err.error?.message || 'Computation failed.');
      },
    });
  }

  showOriginals(): void {
    this.vectorSource.clear();
    this.surfaceArea = null;
    this.successMessage = 'Showing original geometries.';
    this.errorMessage = null;

    const labels = ['A', 'B', 'C'];
    this.polygons.forEach((wkt, index) => {
      if (index < 3) {
        const feature = this.wktFormat.readFeature(wkt);
        feature.set('label', labels[index]);
        this.vectorSource.addFeature(feature);
      }
    });

    const extent = this.vectorSource.getExtent();
    if (extent) {
      this.map.getView().fit(extent, { padding: [50, 50, 50, 50], maxZoom: 15 });
    }
  }

  clearMap(): void {
    this.mode = null;
    this.vectorSource.clear();
    this.polygons = [];
    this.surfaceArea = null;
    this.errorMessage = null;
    this.successMessage = null;
    if (this.drawInteraction) {
      this.map.removeInteraction(this.drawInteraction);
      this.drawInteraction = null;
    }
  }

  reset(): void {
    this.mode = null;
    this.clearMap();
  }

  private showSuccess(msg: string): void {
    this.successMessage = msg;
    this.errorMessage = null;
    this.cdr.detectChanges();
  }

  private showError(msg: string): void {
    this.errorMessage = msg;
    this.successMessage = null;
    this.cdr.detectChanges();
  }
}
