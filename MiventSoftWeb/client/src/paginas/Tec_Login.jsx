// Tec_Login.vb
import { useRef, useState } from 'react';
import { useSesion } from '../global/Sesion';
import Icono from '../componentes/Icono';

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
      <div className="login-lado">
        <div className="marca grande"><span className="marca-logo">S</span><span className="marca-txt">Santit<small>Distribución</small></span></div>
        <h1>Sistema de Distribución</h1>
        <p>Pedidos, despachos, inventario y cobranza en un solo lugar.</p>
        <ul className="login-puntos">
          <li><Icono n="truck" size={18} /> Control de despachos y rutas</li>
          <li><Icono n="box" size={18} /> Inventario por almacén</li>
          <li><Icono n="cart" size={18} /> Ventas y conciliación</li>
        </ul>
      </div>
      <form className="login-form" onSubmit={btnIngresar} noValidate>
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
        <button className="btn btn-pri btn-bloque" disabled={cargando}>{cargando ? 'Validando...' : 'Ingresar'}</button>
        <p className="login-pie">© {new Date().getFullYear()} Santit</p>
      </form>
    </div>
  );
}
