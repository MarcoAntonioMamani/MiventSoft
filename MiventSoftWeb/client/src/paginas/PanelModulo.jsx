// Panel de un modulo (/configuracion, /ventas, ...): sus programas como tarjetas
import { Link, Navigate, useParams } from 'react-router-dom';
import { useSesion } from '../global/Sesion';
import { PROGRAMAS_WEB, iconoModulo, imgPrograma, slug, titulo } from '../global/programas';
import Icono from '../componentes/Icono';
import { usePestanas } from './Tec_Principal';

// Paletas de las tarjetas (como el menu del escritorio: violeta / turquesa / azul, mejorado)
const PALETAS = 8;
export function TarjetasProgramas({ programas }) {
  const { urlDe } = usePestanas(); // si el programa ya esta abierto, vuelve a como estaba
  return (
    <div className="mosaico">
      {programas.map((p, i) => {
        const w = PROGRAMAS_WEB[p.IdPrograma];
        const estilo = { '--i': i };
        const cont = (
          <>
            <span className="mos-deco" aria-hidden="true" />
            <span className={`mos-ic ${imgPrograma(p.IdPrograma) ? 'con-img' : ''}`}>
              {imgPrograma(p.IdPrograma) ? <img src={imgPrograma(p.IdPrograma)} alt="" loading="lazy" /> : <Icono n={w ? w.icono : 'grid'} size={30} />}
            </span>
            <b className="mos-tit">{titulo(p.DescripcionPrograma)}</b>
            <span className="mos-pie">{w ? <>Abrir <Icono n="next" size={14} /></> : 'Próximamente'}</span>
          </>
        );
        return w
          ? <Link key={p.IdPrograma} to={urlDe(w.ruta)} className={`mos-tile pal-${i % PALETAS}`} style={estilo}>{cont}</Link>
          : <div key={p.IdPrograma} className="mos-tile pendiente" style={estilo} title="Aún no migrado a la web">{cont}</div>;
      })}
    </div>
  );
}

export default function PanelModulo() {
  const { modulo: m } = useParams();
  const { sesion } = useSesion();
  const modulo = sesion.modulos.find((x) => slug(x.cndesc1) === m);
  if (!modulo) return <Navigate to="/" replace />;
  const listos = modulo.programas.filter((p) => PROGRAMAS_WEB[p.IdPrograma]).length;
  return (
    <div className="panel-modulo entrada">
      <header className="mos-banner">
        <span className="mos-banner-ic"><Icono n={iconoModulo(modulo.cndesc1)} size={28} /></span>
        <div>
          <h1>{titulo(modulo.cndesc1)}</h1>
          <p>{modulo.programas.length} programas · {listos} disponibles en la web</p>
        </div>
      </header>
      <TarjetasProgramas programas={modulo.programas} />
    </div>
  );
}
