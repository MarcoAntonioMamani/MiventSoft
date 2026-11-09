import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import '@fontsource/roboto/latin-400.css';
import '@fontsource/roboto/latin-500.css';
import '@fontsource/roboto/latin-700.css';
import App from './App';
import { SesionProvider } from './global/Sesion';
import { ToastProvider } from './componentes/Toast';
import { activarRipple } from './global/ripple';
import './estilos/app.css';

activarRipple();

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <BrowserRouter>
      <ToastProvider>
        <SesionProvider>
          <App />
        </SesionProvider>
      </ToastProvider>
    </BrowserRouter>
  </React.StrictMode>
);
