// Tec_Login.vb
import { useRef, useState } from 'react';
import { useSesion } from '../global/Sesion';
import Icono from '../componentes/Icono';
import { MarcaSistema, NOMBRE_SISTEMA, LEMA_SISTEMA } from '../componentes/Logo';
import { esApp, servidorApp, configurarServidorApp } from '../global/appMovil';

export default function Tec_Login() {
  const { ingresar, aviso } = useSesion();
  const [usuario, setUsuario] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(false);
  const pass = useRef(null);

  const btnIngresar = async (e) => {
    e.preventDefault();
    if (!usuario) return setError('Error: Debe ingresar un usuario correcto!!!');
    if (!password) return setError('Error: Debe ingresar una contraseña correctamente..!!!');
    setCargando(true);
    setError('');
    try {
      await ingresar(usuario, password);
    } catch (err) {
      setError(err.message);
      setPassword('');
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="login">
      <aside className="login-lado">
        <div className="marca grande"><MarcaSistema /></div>
        <div className="login-texto">
          <span className="login-etiqueta"><Icono n="store" size={14} /> {LEMA_SISTEMA}</span>
          <h1>Sistema de Punto de Ventas</h1>
          <p>Ventas, caja, inventario y clientes en un solo lugar.</p>
          <ul className="login-puntos">
            <li><Icono n="cart" size={18} /> Ventas y cobro en mostrador</li>
            <li><Icono n="wallet" size={18} /> Caja: ingresos, egresos y cierre</li>
            <li><Icono n="box" size={18} /> Inventario por almacén</li>
          </ul>
        </div>
      </aside>
      <main className="login-panel">
      <div className="marca grande login-marca-movil"><MarcaSistema /></div>
      <form className="login-form" onSubmit={btnIngresar} noValidate>
        <span className="login-saludo">Bienvenido</span>
        <h2>Iniciar sesión</h2>
        <p className="sub">Ingrese con su usuario del sistema</p>
        {(error || aviso) && <div className="alerta" role="alert"><Icono n="alert" size={16} /> {error || aviso}</div>}
        <label className="campo">
          <span className="campo-etq">Usuario</span>
          <div className="control-icono"><Icono n="user" size={16} />
            <input className="control mayus" autoFocus value={usuario} autoComplete="username"
              onChange={(e) => setUsuario(e.target.value.toUpperCase())}
              onKeyDown={(e) => { if (e.key === 'Enter') { e.preventDefault(); pass.current?.focus(); } }} />
          </div>
        </label>
        <label className="campo">
          <span className="campo-etq">Contraseña</span>
          <div className="control-icono"><Icono n="lock" size={16} />
            <input ref={pass} className="control" type="password" value={password} autoComplete="current-password" onChange={(e) => setPassword(e.target.value)} />
          </div>
        </label>
        <button className="btn btn-pri btn-bloque" disabled={cargando}>{cargando ? 'Validando...' : <>Ingresar <Icono n="next" size={16} /></>}</button>
        {esApp && (
          <button type="button" className="btn btn-sec btn-bloque" onClick={configurarServidorApp} disabled={cargando}>
            <Icono n="settings" size={16} /> Servidor: {servidorApp() || 'sin configurar'}
          </button>
        )}
        <p className="login-pie">© {new Date().getFullYear()} {NOMBRE_SISTEMA} · {LEMA_SISTEMA}</p>
      </form>
      </main>
    </div>
  );
}
