import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import App from './App';
import Home from './pages/Home';
import Catalog from './pages/catalog/Catalog';
import EditGame from './pages/catalog/EditGame';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Router>
      <Routes>
        <Route path="/" element={<App />}>
          <Route index element={<Home />} />
          <Route path="catalog" element={<Catalog />} />
          <Route path="catalog/editgame" element={<EditGame />} />
          <Route path="catalog/editgame/:id" element={<EditGame />} />
        </Route>
      </Routes>
    </Router>
  </StrictMode>,
);
