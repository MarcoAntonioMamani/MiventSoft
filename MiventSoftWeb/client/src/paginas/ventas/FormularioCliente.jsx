// =====================================================================
//  KAILY: Utils/FormularioCliente.vb (Efecto tipo 7 desde Tec_Ventas y Tec_Proformas)
//  Crear un cliente sin salir de la venta: Nombre, Tipo Documento (libreria 8), Nro Documento,
//  Telefono y Precios (L_prListaCategoriasPrecios). InsertarClienteFormularioExterno (MAM_Clientes @tipo 8).
// =====================================================================
import { useEffect, useState } from 'react';
import { createPortal } from 'react-dom';
import { api } from '../../api/http';
import { useToast } from '../../componentes/Toast';
import { Campo } from '../../componentes/Controles';
import Icono from '../../componentes/Icono';

export default function FormularioCliente({ url, onCreado, onCerrar }) {
  const toast = useToast();
  const [combos, setCombos] = useState(null);
  const [f, setF] = useState({ NombreCliente: '', TipoDocumento: '', NroDocumento: '', Telefono: '', PrecioCategoriaId: '' });
  const [error, setError] = useState('');
  const [grabando, setGrabando] = useState(false);
  const set = (k) => (e) => { setError(''); setF((x) => ({ ...x, [k]: e.target.value })); };

  // FormularioAyuda_Load: primer item de Tipo Documento y de Precios
  useEffect(() => {
    api.get(`${url}/combos`).then((r) => {
      setCombos(r.data);
      setF((x) => ({ ...x, TipoDocumento: r.data.tiposDocumento[0]?.cnnum ?? '', PrecioCategoriaId: r.data.categoriasPrecio[0]?.Id ?? '' }));
    }).catch((e) => toast(e.message, 'error'));
  }, [url]); // eslint-disable-line react-hooks/exhaustive-deps

  // ButtonX1_Click: _PMOValidarCampos + InsertarClienteFormularioExterno
  const grabar = async () => {
    const falta = [!f.NombreCliente.trim() && 'Nombre Cliente', !f.PrecioCategoriaId && 'Precios', !f.TipoDocumento && 'Tipo Documento'].filter(Boolean);
    if (falta.length) { setError(`Los Siguientes Campos Son Requeridos: ${falta.join(', ')}`); return; }
    setGrabando(true);
    try {
      const r = await api.post(url, { ...f, NombreCliente: f.NombreCliente.trim().toUpperCase() });
      toast(r.mensaje, 'ok');
      onCreado(r.data);
    } catch (e) { toast(e.message, 'error'); } finally { setGrabando(false); }
  };

  return createPortal(
    <div className="modal-fondo" onMouseDown={onCerrar}>
      <div className="modal" role="dialog" aria-modal="true" aria-label="Nuevo cliente" onMouseDown={(e) => e.stopPropagation()}
        onKeyDown={(e) => { if (e.key === 'Escape') onCerrar(); }}>
        <div className="modal-cab"><h3>NUEVO CLIENTE</h3><button className="btn-icono" onClick={onCerrar} aria-label="Cerrar"><Icono n="x" /></button></div>
        <div className="campos">
          <Campo etiqueta="Nombre cliente" ancho={3} error={error && !f.NombreCliente.trim() ? 'Ingrese el Nombre Cliente' : undefined} requerido>
            <input autoFocus className="control" value={f.NombreCliente} onChange={set('NombreCliente')} maxLength={200} onKeyDown={(e) => e.key === 'Enter' && grabar()} />
          </Campo>
          <Campo etiqueta="Tipo documento" requerido>
            <select className="control" value={f.TipoDocumento} onChange={set('TipoDocumento')} disabled={!combos}>
              {(combos?.tiposDocumento || []).map((t) => <option key={t.cnnum} value={t.cnnum}>{t.cndesc1}</option>)}
            </select>
          </Campo>
          <Campo etiqueta="Nro documento"><input className="control" value={f.NroDocumento} onChange={set('NroDocumento')} maxLength={50} inputMode="numeric" /></Campo>
          <Campo etiqueta="Teléfono"><input className="control" value={f.Telefono} onChange={set('Telefono')} maxLength={50} inputMode="tel" /></Campo>
          <Campo etiqueta="Precios" requerido>
            <select className="control" value={f.PrecioCategoriaId} onChange={set('PrecioCategoriaId')} disabled={!combos}>
              {(combos?.categoriasPrecio || []).map((c) => <option key={c.Id} value={c.Id}>{c.Descripcion}</option>)}
            </select>
          </Campo>
        </div>
        {error && <p className="campo-error">{error}</p>}
        <div className="modal-acciones">
          <button className="btn btn-sec" onClick={onCerrar}>Salir</button>
          <button className="btn btn-pri" onClick={grabar} disabled={grabando || !combos}>{grabando ? <span className="spinner" /> : <Icono n="save" size={15} />} Grabar</button>
        </div>
      </div>
    </div>,
    document.body,
  );
}
