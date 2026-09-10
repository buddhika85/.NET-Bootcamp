import React, { useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';

interface PrivateRouteProps {
  children: React.ReactNode;
  requiredRole?: string;
}

const PrivateRoute: React.FC<PrivateRouteProps> = ({ children, requiredRole }) => {
  const auth = useAuth();
  const location = useLocation();

  const isAuthenticated = auth.isAuthenticated;
  const role = auth.user?.profile?.role;
  const roles = Array.isArray(role) ? role : role ? [role as string] : [];
  const hasRequiredRole = requiredRole ? roles.includes(requiredRole) : true;

  useEffect(() => {
    if (!isAuthenticated && !auth.isLoading) {
      auth.signinRedirect({ state: { from: location } });
    }
  }, [isAuthenticated, auth, location]);

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  if (isAuthenticated && !hasRequiredRole) {
    return <Navigate to="/accessDenied" state={{ from: location }} />;
  }

  if (!isAuthenticated) {
    return null; // Return null while redirecting to login
  }

  return <>{children}</>;
};

export default PrivateRoute;