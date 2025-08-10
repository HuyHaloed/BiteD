import { withAuthenticationRequired } from "react-oidc-context";
import { ReactNode } from "react";
import FullscreenLoading from "./FullscreenLoading";

const ProtectedRoute = ({ children }: { children: ReactNode }) => children;

const AuthenticatedProtectedRoute = withAuthenticationRequired(ProtectedRoute, {
  OnRedirecting: () => (
    <>
      <FullscreenLoading message="Redirecting to the login page..." />
    </>
  ),
});

export default AuthenticatedProtectedRoute;
