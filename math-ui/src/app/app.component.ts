import { Component, OnInit, inject }    from '@angular/core';
import { CommonModule }                  from '@angular/common';
import { FormsModule }                   from '@angular/forms';
import { MatCardModule }                 from '@angular/material/card';
import { MatButtonModule }               from '@angular/material/button';
import { MatInputModule }                from '@angular/material/input';
import { MatFormFieldModule }            from '@angular/material/form-field';
import { MatIconModule }                 from '@angular/material/icon';
import { MatDividerModule }              from '@angular/material/divider';
import { MatProgressSpinnerModule }      from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule }              from '@angular/material/tooltip';
import { MileageService, MpgResult, TripCostResult, RangeResult, EmissionsResult } from './services/math.service';

type Mode = 'mpg' | 'tripcost' | 'range' | 'emissions';

interface ModeConfig { key: Mode; label: string; icon: string; desc: string; }
interface Stat       { value: string; label: string; unit?: string; highlight?: boolean; }
interface HistoryItem{ summary: string; mode: Mode; time: Date; }

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatButtonModule, MatInputModule, MatFormFieldModule,
    MatIconModule, MatDividerModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatTooltipModule,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  private readonly svc   = inject(MileageService);
  private readonly snack = inject(MatSnackBar);

  mode: Mode = 'mpg';
  loading    = false;
  stats: Stat[]         = [];
  history: HistoryItem[] = [];

  // MPG inputs
  miles   = ''; gallons = '';
  // Trip cost inputs
  tripMiles = ''; tripMpg = ''; pricePerGallon = '';
  // Range inputs
  tankSize = ''; rangeMpg = '';
  // Emissions inputs
  emMiles = ''; emMpg = '';

  readonly modes: ModeConfig[] = [
    { key: 'mpg',       label: 'MPG',       icon: 'speed',    desc: 'Calculate fuel efficiency' },
    { key: 'tripcost',  label: 'Trip Cost',  icon: 'payments', desc: 'Estimate cost of a trip' },
    { key: 'range',     label: 'Range',      icon: 'explore',  desc: 'How far can you go?' },
    { key: 'emissions', label: 'Emissions',  icon: 'eco',      desc: 'CO₂ footprint of your drive' },
  ];

  ngOnInit() { document.body.classList.add('dark-theme'); }

  selectMode(m: Mode) { this.mode = m; this.stats = []; }

  calculate() {
    this.loading = true;
    this.stats   = [];
    switch (this.mode) {
      case 'mpg':
        this.svc.calcMpg(+this.miles, +this.gallons).subscribe({
          next: r => { this.loading = false; this.showMpg(r); },
          error: () => this.err(),
        }); break;
      case 'tripcost':
        this.svc.calcTripCost(+this.tripMiles, +this.tripMpg, +this.pricePerGallon).subscribe({
          next: r => { this.loading = false; this.showTrip(r); },
          error: () => this.err(),
        }); break;
      case 'range':
        this.svc.calcRange(+this.tankSize, +this.rangeMpg).subscribe({
          next: r => { this.loading = false; this.showRange(r); },
          error: () => this.err(),
        }); break;
      case 'emissions':
        this.svc.calcEmissions(+this.emMiles, +this.emMpg).subscribe({
          next: r => { this.loading = false; this.showEmissions(r); },
          error: () => this.err(),
        }); break;
    }
  }

  private showMpg(r: MpgResult) {
    this.stats = [
      { value: r.mpg.toFixed(1),    label: 'Miles per Gallon',    unit: 'MPG',    highlight: true },
      { value: r.l100km.toFixed(1), label: 'Liters per 100 km',   unit: 'L/100km' },
      { value: r.miles.toString(),  label: 'Miles Driven',         unit: 'mi' },
      { value: r.gallons.toString(), label: 'Gallons Used',        unit: 'gal' },
    ];
    this.push(`${r.miles} mi on ${r.gallons} gal → ${r.mpg.toFixed(1)} MPG`, 'mpg');
  }

  private showTrip(r: TripCostResult) {
    this.stats = [
      { value: `$${r.totalCost.toFixed(2)}`,    label: 'Total Trip Cost',   highlight: true },
      { value: r.gallonsNeeded.toFixed(2),       label: 'Gallons Needed',    unit: 'gal' },
      { value: `$${r.costPerMile.toFixed(3)}`,   label: 'Cost per Mile' },
      { value: `$${r.pricePerGallon}/gal`,       label: 'Gas Price' },
    ];
    this.push(`${r.miles} mi @ ${r.mpg} MPG @ $${r.pricePerGallon}/gal → $${r.totalCost.toFixed(2)}`, 'tripcost');
  }

  private showRange(r: RangeResult) {
    this.stats = [
      { value: r.rangeMiles.toFixed(0), label: 'Range',        unit: 'miles', highlight: true },
      { value: r.rangeKm.toFixed(0),    label: 'Range',        unit: 'km' },
      { value: r.tank.toString(),        label: 'Tank Size',    unit: 'gal' },
      { value: r.mpg.toString(),         label: 'Fuel Economy', unit: 'MPG' },
    ];
    this.push(`${r.tank} gal @ ${r.mpg} MPG → ${r.rangeMiles.toFixed(0)} mi range`, 'range');
  }

  private showEmissions(r: EmissionsResult) {
    this.stats = [
      { value: r.co2Kg.toFixed(1),       label: 'CO₂ Emitted',             unit: 'kg',  highlight: true },
      { value: r.co2Lbs.toFixed(1),      label: 'CO₂ Emitted',             unit: 'lbs' },
      { value: r.treesNeeded.toFixed(1), label: 'Trees to offset (1 yr)',   unit: '🌳' },
      { value: r.miles.toString(),        label: 'Miles Driven',             unit: 'mi' },
    ];
    this.push(`${r.miles} mi @ ${r.mpg} MPG → ${r.co2Kg.toFixed(1)} kg CO₂`, 'emissions');
  }

  private push(summary: string, mode: Mode) {
    this.history.unshift({ summary, mode, time: new Date() });
    if (this.history.length > 10) this.history.pop();
  }

  private err() {
    this.loading = false;
    this.snack.open('Cannot reach the API — is the .NET server running on :5000?', 'OK', { duration: 5000 });
  }

  modeIcon(m: Mode) { return this.modes.find(x => x.key === m)?.icon ?? 'help'; }
  clearHistory()    { this.history = []; }
}
