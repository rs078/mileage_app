import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface MpgResult       { mpg: number; l100km: number; miles: number; gallons: number; processedBy: string; }
export interface TripCostResult  { totalCost: number; gallonsNeeded: number; costPerMile: number; miles: number; mpg: number; pricePerGallon: number; processedBy: string; }
export interface RangeResult     { rangeMiles: number; rangeKm: number; tank: number; mpg: number; processedBy: string; }
export interface EmissionsResult { co2Kg: number; co2Lbs: number; treesNeeded: number; miles: number; mpg: number; processedBy: string; }
export interface GasPriceResult  { price: number; live: boolean; source: string; }
export interface SavingsResult   { annualSavings: number; gallonsSaved: number; co2SavedKg: number; mpgImprovement: number; currentMpg: number; newMpg: number; annualMiles: number; ppg: number; processedBy: string; }
export interface DbHistoryEntry  { id: number; mode: string; summary: string; createdAt: string; }

@Injectable({ providedIn: 'root' })
export class MileageService {
  private readonly http = inject(HttpClient);
  private readonly api  = `${environment.apiUrl}/api/mileage`;

  calcMpg(miles: number, gallons: number): Observable<MpgResult> {
    return this.http.get<MpgResult>(`${this.api}/mpg`,
      { params: new HttpParams().set('miles', miles).set('gallons', gallons) });
  }

  calcTripCost(miles: number, mpg: number, ppg: number): Observable<TripCostResult> {
    return this.http.get<TripCostResult>(`${this.api}/trip-cost`,
      { params: new HttpParams().set('miles', miles).set('mpg', mpg).set('ppg', ppg) });
  }

  calcRange(tank: number, mpg: number): Observable<RangeResult> {
    return this.http.get<RangeResult>(`${this.api}/range`,
      { params: new HttpParams().set('tank', tank).set('mpg', mpg) });
  }

  calcEmissions(miles: number, mpg: number): Observable<EmissionsResult> {
    return this.http.get<EmissionsResult>(`${this.api}/emissions`,
      { params: new HttpParams().set('miles', miles).set('mpg', mpg) });
  }

  calcSavings(currentMpg: number, newMpg: number, annualMiles: number, ppg: number): Observable<SavingsResult> {
    return this.http.get<SavingsResult>(`${this.api}/savings`,
      { params: new HttpParams().set('currentMpg', currentMpg).set('newMpg', newMpg).set('annualMiles', annualMiles).set('ppg', ppg) });
  }

  getGasPrice(state?: string): Observable<GasPriceResult> {
    let params = new HttpParams();
    if (state) params = params.set('state', state);
    return this.http.get<GasPriceResult>(`${this.api}/gas-price`, { params });
  }

  getHistory(): Observable<DbHistoryEntry[]> {
    return this.http.get<DbHistoryEntry[]>(`${this.api}/history`);
  }

  clearHistory(): Observable<void> {
    return this.http.delete<void>(`${this.api}/history`);
  }
}
