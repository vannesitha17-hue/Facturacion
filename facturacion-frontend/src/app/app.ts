
import { Component, OnInit, signal, PLATFORM_ID, inject, ChangeDetectorRef } from '@angular/core';
import { isPlatformBrowser, CommonModule } from '@angular/common';
import { Factura, FacturaService } from './services/factura.service';

@Component({
  selector: 'app-root',
  imports: [CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('Facturación');

  private platformId = inject(PLATFORM_ID);
  private facturaService = inject(FacturaService);
  private changeDetector = inject(ChangeDetectorRef);

  facturas: Factura[] = [];
  cargando = true;
  error = '';
  procesando = false;
  mensaje = '';


  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.facturaService.obtenerFacturas().subscribe({
      next: (datos) => {
        console.log('Facturas recibidas:', datos);

        this.facturas = datos;
        this.cargando = false;

        this.changeDetector.detectChanges();
      },
      error: (err) => {
        console.error('Error al consultar facturas:', err);

        this.error = 'No fue posible cargar las facturas.';
        this.cargando = false;

        this.changeDetector.detectChanges();
      }
    });
  }

ejecutarProcesamiento(): void {
  this.procesando = true;
  this.mensaje = '';

  this.facturaService.procesarRecordatorios().subscribe({
    next: () => {
      this.facturaService.obtenerFacturas().subscribe({
        next: (datos) => {
          this.facturas = datos;
          this.procesando = false;
          this.mensaje = 'Recordatorios procesados correctamente.';
          this.changeDetector.detectChanges();
        },
        error: () => {
          this.procesando = false;
          this.mensaje = 'El proceso terminó, pero no se pudieron actualizar las facturas.';
          this.changeDetector.detectChanges();
        }
      });
    },
    error: (err) => {
      console.error('Error al procesar recordatorios:', err);
      this.procesando = false;
      this.mensaje = 'No fue posible procesar los recordatorios.';
      this.changeDetector.detectChanges();
    }
  });
}

}
