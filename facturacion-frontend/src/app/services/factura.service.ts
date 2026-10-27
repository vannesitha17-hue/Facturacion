
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Factura {
  id?: string;
  cliente: string;
  correo: string;
  numeroFactura: string;
  valor: number;
  fechaVencimiento: string;
  estado: string;
}

@Injectable({
  providedIn: 'root'
})
export class FacturaService {
  private http = inject(HttpClient);

  private apiUrl = 'http://localhost:5460/api/facturas';

  obtenerFacturas(): Observable<Factura[]> {
    return this.http.get<Factura[]>(this.apiUrl);
  }

procesarRecordatorios(): Observable<unknown> {
  return this.http.post(
    `${this.apiUrl}/procesar-recordatorios`,
    {}
  );
}

}