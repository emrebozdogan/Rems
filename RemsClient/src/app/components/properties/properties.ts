import {
  Component,
  inject,
  OnInit,
  ChangeDetectorRef,
  ViewChild,
  ElementRef,
  signal,
  AfterViewInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertiesService } from './properties.service';
import { Property } from '../../models/property.model';
import { City } from '../../models/city.model';
import { District } from '../../models/district.model';
import { Neighborhood } from '../../models/neighborhood.model';
import { PropertyFilter } from '../../models/property-filter.model';
import { Feature, Map, Overlay, View } from 'ol';
import TileLayer from 'ol/layer/Tile';
import { OSM } from 'ol/source';
import WKT from 'ol/format/WKT';
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import Draw from 'ol/interaction/Draw';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import * as turf from '@turf/turf';
import GeoJSON from 'ol/format/GeoJSON';
import { Style, Fill, Stroke } from 'ol/style';
import { forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './properties.html',
  styleUrl: './properties.scss',
})
export class Properties implements OnInit, AfterViewInit {
  map!: Map;
  propertyVectorSource = new VectorSource();
  intersectionVectorSource = new VectorSource();
  drawSource = new VectorSource();
  drawInteraction!: Draw;
  openedImages = signal<{ [id: string]: SafeUrl }>({});
  private propertiesService = inject(PropertiesService);
  private fb = inject(FormBuilder);
  private cdr = inject(ChangeDetectorRef);
  private sanitazer = inject(DomSanitizer);

  properties: Property[] = [];
  totalCount = 0;
  totalPages = 0;
  currentPage = 1;
  pageSize = 10;

  isFilterOpen = true;

  cities: City[] = [];
  districts: District[] = [];
  neighborhoods: Neighborhood[] = [];

  filterForm: FormGroup = this.fb.group({
    cityId: [''],
    districtId: [''],
    neighborhoodId: [''],
    cityName: [''],
    districtName: [''],
    neighborhoodName: [''],
    propertyType: [''],
    parcelNumber: [''],
    lotNumber: [''],
    address: [''],
  });

  isCreateModalOpen = false;
  createDistricts: District[] = [];
  createNeighborhoods: Neighborhood[] = [];

  createForm: FormGroup = this.fb.group({
    cityId: ['', Validators.required],
    districtId: ['', Validators.required],
    neighborhoodId: ['', Validators.required],
    parcelNumber: ['', Validators.required],
    lotNumber: ['', Validators.required],
    address: ['', Validators.required],
    propertyType: ['', Validators.required],
    geometry: ['', Validators.required],
  });

  currentUpdateImage: SafeUrl | null = null;
  selectedImagePreview: SafeUrl | null = null;

  isEditModalOpen = false;
  isDeleteModalOpen = false;
  propertyToDeleteId: string | null = null;
  updateForm: FormGroup = this.fb.group({
    cityId: ['', Validators.required],
    districtId: ['', Validators.required],
    neighborhoodId: ['', Validators.required],
    id: ['', Validators.required],
    parcelNumber: ['', Validators.required],
    lotNumber: ['', Validators.required],
    address: ['', Validators.required],
    propertyType: ['', Validators.required],
    geometry: ['', Validators.required],
    imagePath: [''],
  });

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;
  selectedImageFile: File | null = null;
  isAdmin = false;

  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  closeAlert(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  ngAfterViewInit(): void {
    this.initMap();
  }

  initMap() {
    this.map = new Map({
      target: 'map',
      layers: [
        new TileLayer({
          source: new OSM(),
        }),
        new VectorLayer({
          source: this.propertyVectorSource,
        }),
        new VectorLayer({
          source: this.intersectionVectorSource,
          style: new Style({
            fill: new Fill({
              color: 'rgba(255, 0, 0, 0.5)',
            }),
            stroke: new Stroke({
              color: '#ff0000',
              width: 2,
            }),
          }),
          zIndex: 10,
        }),
        new VectorLayer({
          source: this.drawSource,
          style: {
            'fill-color': 'rgba(255,255,255, 0.2)',
            'stroke-color': '#ffcc33',
            'stroke-width': 2,
            'circle-radius': 7,
            'circle-fill-color': '#ffcc33',
          },
        }),
      ],
      view: new View({
        center: [3895000, 4770000],
        zoom: 6,
      }),
    });
    const popupElement = document.getElementById('popup');
    const popupContent = document.getElementById('popup-content');

    const popupOverlay = new Overlay({
      element: popupElement!,
      positioning: 'bottom-center',
      stopEvent: true,
      offset: [0, -10],
    });
    this.map.addOverlay(popupOverlay);

    this.map.on('singleclick', (event) => {
      const feature = this.map.forEachFeatureAtPixel(event.pixel, (feat) => feat);

      if (feature && popupElement && popupContent) {
        const propData = feature.get('propertyData');
        popupContent.innerHTML = `
          <div style="min-width: 200px;">
            <p class="mb-1"> City: <span class="text-primary">${propData.cityName}</span></p>
            <p class="mb-1"> District: <span class="text-primary">${propData.districtName}</span></p>
            <p class="mb-1"> Neighborhood: <span class="text-primary">${propData.neighborhoodName}</span></p>
            <p class="mb-1 small"><strong>Parcel/Lot:</strong> ${propData.parcelNumber} / ${propData.lotNumber}</p>
            <p class="mb-0 text-muted small" style="white-space: normal;">${propData.address}</p>
          </div>
        `;

        popupOverlay.setPosition(event.coordinate);
        popupElement.style.display = 'block';
      } else if (popupElement) {
        popupElement.style.display = 'none';
      }
    });
  }

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const tokenClaimPart = token.split('.')[1];
        const decoded = JSON.parse(atob(tokenClaimPart));
        this.isAdmin = decoded.role === 'Admin';
      } catch (error) {
        console.error('JWT decode error', error);
      }
    }
    this.loadCities();
    this.loadProperties();
  }

  toggleFilter(): void {
    this.isFilterOpen = !this.isFilterOpen;
  }

  loadCities(): void {
    this.propertiesService.getCities().subscribe({
      next: (data) => (this.cities = data),
      error: (err) => console.error('Failed to load cities', err),
    });
  }

  onCityChange(event: any): void {
    const cityId = event.target.value;
    const selectedCity = this.cities.find((c) => c.id === cityId);
    this.filterForm.patchValue({
      cityName: selectedCity ? selectedCity.name : '',
      districtId: '',
      districtName: '',
      neighborhoodId: '',
      neighborhoodName: '',
    });
    this.districts = [];
    this.neighborhoods = [];

    if (cityId) {
      this.propertiesService.getDistrictsByCityId(cityId).subscribe({
        next: (data) => (this.districts = data),
        error: (err) => console.error('Failed to load districts', err),
      });
    }
  }

  onDistrictChange(event: any): void {
    const districtId = event.target.value;
    const selectedDistrict = this.districts.find((d) => d.id === districtId);
    this.filterForm.patchValue({
      districtName: selectedDistrict ? selectedDistrict.name : '',
      neighborhoodId: '',
      neighborhoodName: '',
    });
    this.neighborhoods = [];

    if (districtId) {
      this.propertiesService.getNeighborhoodsByDistrictId(districtId).subscribe({
        next: (data) => (this.neighborhoods = data),
        error: (err) => console.error('Failed to load neighborhoods', err),
      });
    }
  }

  onNeighborhoodChange(event: any): void {
    const neighborhoodId = event.target.value;
    const selectedNeighborhood = this.neighborhoods.find((n) => n.id === neighborhoodId);
    this.filterForm.patchValue({
      neighborhoodName: selectedNeighborhood ? selectedNeighborhood.name : '',
    });
  }

  onCreateCityChange(event: any): void {
    const cityId = event.target.value;
    this.createForm.patchValue({ districtId: '', neighborhoodId: '' });
    this.createDistricts = [];
    this.createNeighborhoods = [];
    if (cityId) {
      this.propertiesService.getDistrictsByCityId(cityId).subscribe({
        next: (data) => (this.createDistricts = data),
        error: (err) => console.error('Failed to load districts', err),
      });
    }
  }

  onCreateDistrictChange(event: any): void {
    const districtId = event.target.value;
    this.createForm.patchValue({ neighborhoodId: '' });
    this.createNeighborhoods = [];
    if (districtId) {
      this.propertiesService.getNeighborhoodsByDistrictId(districtId).subscribe({
        next: (data) => (this.createNeighborhoods = data),
        error: (err) => console.error('Failed to load neighborhoods', err),
      });
    }
  }

  onUpdateCityChange(event: any): void {
    const cityId = event.target.value;
    this.updateForm.patchValue({ districtId: '', neighborhoodId: '' });
    this.createDistricts = [];
    this.createNeighborhoods = [];
    if (cityId) {
      this.propertiesService.getDistrictsByCityId(cityId).subscribe({
        next: (data) => (this.createDistricts = data),
        error: (err) => console.error('Failed to load districts', err),
      });
    }
  }

  onUpdateDistrictChange(event: any): void {
    const districtId = event.target.value;
    this.updateForm.patchValue({ neighborhoodId: '' });
    this.createNeighborhoods = [];
    if (districtId) {
      this.propertiesService.getNeighborhoodsByDistrictId(districtId).subscribe({
        next: (data) => (this.createNeighborhoods = data),
        error: (err) => console.error('Failed to load neighborhoods', err),
      });
    }
  }

  loadProperties(): void {
    const filter: PropertyFilter = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      cityName: this.filterForm.value.cityName || undefined,
      districtName: this.filterForm.value.districtName || undefined,
      neighborhoodName: this.filterForm.value.neighborhoodName || undefined,
      propertyType: this.filterForm.value.propertyType || undefined,
      parcelNumber: this.filterForm.value.parcelNumber || undefined,
      lotNumber: this.filterForm.value.lotNumber || undefined,
      address: this.filterForm.value.address || undefined,
    };

    this.propertiesService.getProperties(filter).subscribe({
      next: (response) => {
        this.properties = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.currentPage = response.pageNumber;

        this.propertyVectorSource.clear();
        const wktFormat = new WKT();
        this.properties.forEach((prop) => {
          if (prop.geometry) {
            const feature = wktFormat.readFeature(prop.geometry, {
              dataProjection: 'EPSG:4326',
              featureProjection: 'EPSG:3857',
            });

            feature.set('propertyData', prop);
            this.propertyVectorSource.addFeature(feature);
          }
        });
        this.highlightIntersections();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load properties.', err);
      },
    });
  }

  highlightIntersections(): void {
    this.intersectionVectorSource.clear();
    const features = this.propertyVectorSource.getFeatures();
    const geojsonFormat = new GeoJSON();
    const turfPolygons: any[] = [];

    features.forEach(feature => {
      try {
        const geojson = geojsonFormat.writeFeatureObject(feature, {
          featureProjection: 'EPSG:3857',
          dataProjection: 'EPSG:4326',
        });
        turfPolygons.push(geojson);
      } catch (e) {
        console.error('GeoJSON conversion error', e);
      }
    });

    for (let i = 0; i < turfPolygons.length; i++) {
      for (let j = i + 1; j < turfPolygons.length; j++) {
        try {
          const poly1 = turfPolygons[i];
          const poly2 = turfPolygons[j];
          const intersection = turf.intersect(turf.featureCollection([poly1, poly2]));
          
          if (intersection) {
            const olFeature = geojsonFormat.readFeature(intersection, {
              dataProjection: 'EPSG:4326',
              featureProjection: 'EPSG:3857',
            });
            this.intersectionVectorSource.addFeature(olFeature as Feature);
          }
        } catch (e) {
          console.error('Turf intersect error', e);
        }
      }
    }
  }

  applyFilter(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.currentPage = 1;
    this.loadProperties();
  }

  clearFilter(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.filterForm.reset({
      cityId: '',
      districtId: '',
      neighborhoodId: '',
      cityName: '',
      districtName: '',
      neighborhoodName: '',
      propertyType: '',
      parcelNumber: '',
      lotNumber: '',
      address: '',
    });
    this.districts = [];
    this.neighborhoods = [];
    this.currentPage = 1;
    this.loadProperties();
  }

  changePage(page: number | string): void {
    if (typeof page === 'number' && page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProperties();
    }
  }

  getPageNumbers(): (number | string)[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const delta = 1;
    const range: number[] = [];
    const rangeWithDots: (number | string)[] = [];
    let l: number | undefined;

    for (let i = 1; i <= total; i++) {
      if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
        range.push(i);
      }
    }

    for (const i of range) {
      if (l) {
        if (i - l === 2) {
          rangeWithDots.push(l + 1);
        } else if (i - l !== 1) {
          rangeWithDots.push('...');
        }
      }
      rangeWithDots.push(i);
      l = i;
    }

    return rangeWithDots;
  }

  get currentStartIndex(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get currentEndIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  openCreateModal(): void {
    this.createForm.reset();
    this.createDistricts = [];
    this.createNeighborhoods = [];
    this.selectedImageFile = null;
    this.isCreateModalOpen = true;
  }

  closeCreateModal(): void {
    this.isCreateModalOpen = false;
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedImageFile = input.files[0];
      const unsafeUrl = URL.createObjectURL(this.selectedImageFile);
      this.selectedImagePreview = this.sanitazer.bypassSecurityTrustUrl(unsafeUrl);
    } else {
      this.selectedImageFile = null;
      this.selectedImagePreview = null;
    }
  }

  submitCreateForm(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.propertiesService.createProperty(this.createForm.value).subscribe({
      next: (response: any) => {
        if (this.selectedImageFile && response.id) {
          this.propertiesService.uploadImage(response.id, this.selectedImageFile).subscribe({
            next: () => {
              this.successMessage.set('Property added successfully.');
              this.closeCreateModal();
              this.loadProperties();
              setTimeout(() => {
                this.map.updateSize();
              }, 100);
              this.drawSource.clear();
            },
            error: (err) => {
              console.error('Failed to upload image', err);
              this.errorMessage.set('Property created, but image upload failed.');
              this.closeCreateModal();
              this.loadProperties();
            },
          });
        } else {
          this.successMessage.set('Property added successfully.');
          this.closeCreateModal();
          this.loadProperties();
          setTimeout(() => {
            this.map.updateSize();
          }, 100);
          this.drawSource.clear();
        }
      },
      error: (err) => {
        console.error('Failed to create property', err);
        this.errorMessage.set('Failed to add property. Please check all required fields.');
      },
    });
  }

  triggerFileInput(): void {
    this.fileInput?.nativeElement.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.propertiesService.importPropertiesFromExcel(file).subscribe({
        next: () => {
          this.successMessage.set('Properties imported successfully.');
          this.loadProperties();
          input.value = '';
        },
        error: (err) => {
          console.error('Failed to import properties', err);
          this.errorMessage.set('Import failed. Please check the file format and data.');
          input.value = '';
        },
      });
    }
  }

  startDrawingMode(): void {
    this.isCreateModalOpen = false;
    this.drawSource.clear();
    this.drawInteraction = new Draw({
      source: this.drawSource,
      type: 'Polygon',
      minPoints: 4,
      maxPoints: 4,
    });

    this.map.addInteraction(this.drawInteraction);
    this.drawInteraction.on('drawend', (event) => {
      const wktFormat = new WKT();
      const wktString = wktFormat.writeFeature(event.feature, {
        dataProjection: 'EPSG:4326',
        featureProjection: 'EPSG:3857',
      });

      this.createForm.patchValue({ geometry: wktString });

      this.map.removeInteraction(this.drawInteraction);
      this.isCreateModalOpen = true;
      this.cdr.detectChanges();
    });
  }

  viewImage(id: string) {
    const currentImages = this.openedImages();
    if (currentImages[id]) {
      const updated = { ...currentImages };
      delete updated[id];
      this.openedImages.set(updated);
      return;
    }

    this.propertiesService.getPropertyImage(id).subscribe({
      next: (blob) => {
        const unsafeUrl = URL.createObjectURL(blob);
        const safeUrl = this.sanitazer.bypassSecurityTrustUrl(unsafeUrl);
        this.openedImages.update((imgs) => ({ ...imgs, [id]: safeUrl }));
      },
      error: (err) => {
        console.error('Failed to load image.', err);
        this.errorMessage.set('Could not load image for this property.');
      },
    });
  }

  openDeleteModal(id: string) {
    this.propertyToDeleteId = id;
    this.isDeleteModalOpen = true;
  }

  closeDeleteModal() {
    this.isDeleteModalOpen = false;
    this.propertyToDeleteId = null;
  }

  confirmDelete() {
    if (this.propertyToDeleteId) {
      this.propertiesService.deleteProperty(this.propertyToDeleteId).subscribe({
        next: () => {
          this.successMessage.set('Property deleted successfully.');
          this.loadProperties();
          this.closeDeleteModal();
        },
        error: (err) => {
          console.error('Failed to delete property', err);
          this.closeDeleteModal();
        },
      });
    }
  }

  openEditModal(property: any) {
    this.isEditModalOpen = true;
    this.selectedImageFile = null;
    this.selectedImagePreview = null;
    this.currentUpdateImage = null;

    if (property.imagePath) {
      this.propertiesService.getPropertyImage(property.id).subscribe({
        next: (blob) => {
          const unsafeUrl = URL.createObjectURL(blob);
          this.currentUpdateImage = this.sanitazer.bypassSecurityTrustUrl(unsafeUrl);
        },
        error: (err) => console.error('Failed to load image for edit modal', err)
      });
    }

    const district$ = property.cityId
      ? this.propertiesService.getDistrictsByCityId(property.cityId)
      : of([]);
    const neighborhood$ = property.districtId
      ? this.propertiesService.getNeighborhoodsByDistrictId(property.districtId)
      : of([]);

    forkJoin([district$, neighborhood$]).subscribe({
      next: ([districts, neighborhoods]) => {
        this.createDistricts = districts;
        this.createNeighborhoods = neighborhoods;

        setTimeout(() => {
          this.updateForm.patchValue({
            cityId: property.cityId,
            districtId: property.districtId,
            neighborhoodId: property.neighborhoodId,
            id: property.id,
            parcelNumber: property.parcelNumber,
            lotNumber: property.lotNumber,
            address: property.address,
            propertyType: property.propertyType,
            geometry: property.geometry,
            imagePath: property.imagePath,
          });
        });
      },
      error: (err) => console.error('Failed to load related data', err),
    });
  }

  submitUpdateForm(): void {
    if (this.updateForm.invalid) {
      this.updateForm.markAllAsTouched();
      return;
    }

    this.propertiesService.updateProperty(this.updateForm.value).subscribe({
      next: (response: any) => {
        if (this.selectedImageFile && response.id) {
          this.propertiesService.uploadImage(response.id, this.selectedImageFile).subscribe({
            next: () => {
              this.successMessage.set('Property updated successfully.');
              this.isEditModalOpen = false;
              this.loadProperties();
              setTimeout(() => {
                this.map.updateSize();
              }, 100);
              this.drawSource.clear();
            },
            error: (err) => {
              console.error('Failed to upload image', err);
              this.errorMessage.set('Property updated, but image upload failed.');
              this.isEditModalOpen = false;
              this.loadProperties();
            },
          });
        } else {
          this.successMessage.set('Property updated successfully.');
          this.isEditModalOpen = false;
          this.loadProperties();
          setTimeout(() => {
            this.map.updateSize();
          }, 100);
          this.drawSource.clear();
        }
      },
      error: (err) => {
        console.error('Failed to update property', err);
        this.errorMessage.set('Please enter valid property details.');
      },
    });
  }

  startDrawingModeUpdate(): void {
    this.isEditModalOpen = false;
    this.drawSource.clear();
    this.drawInteraction = new Draw({
      source: this.drawSource,
      type: 'Polygon',
      minPoints: 4,
      maxPoints: 4,
    });

    this.map.addInteraction(this.drawInteraction);
    this.drawInteraction.on('drawend', (event) => {
      const wktFormat = new WKT();
      const wktString = wktFormat.writeFeature(event.feature, {
        dataProjection: 'EPSG:4326',
        featureProjection: 'EPSG:3857',
      });

      this.updateForm.patchValue({ geometry: wktString });

      this.map.removeInteraction(this.drawInteraction);
      this.isEditModalOpen = true;
      this.cdr.detectChanges();
    });
  }
}
