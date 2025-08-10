import { Navigate, useLocation } from "react-router-dom";
import $api from "../services/api";

export default function DefaultRedirect() {
  const location = useLocation();
  const user = $api.useQuery("get", "/swagger/Users/me");

  // Kiểm tra nếu URL hiện tại có chứa '/external'
  if (location.pathname.includes("/external")) {
    return <Navigate replace to="/external" />;
  }

  if (user.isLoading) return <div>Loading...</div>;

  if (user.data?.isSupplier) {
    return <Navigate replace to="/qr-scanner" />;
  }

  return <Navigate replace to="/order" />;
}